#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Animation;
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Runtime
{
    public enum EAnimationSetting
    {
        Container = 1,
        Custom = 2
    }

    // Animation states of UI Views
    public enum EVisibleState
    {
        Appearing = 1,
        Appeared = 2,
        Disappearing = 3,
        Disappeared = 4
    }

    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]

#if VCONTAINER_SUPPORT
    public abstract class UIContext : LifetimeScope
#else
    public abstract class UIContext : MonoBehaviour
#endif
    {
        #region Fields

        [SerializeField] private EAnimationSetting _animationSetting = EAnimationSetting.Container;
        [SerializeField] private ViewShowAnimation _showAnimation = new();
        [SerializeField] private ViewHideAnimation _hideAnimation = new();

        private CanvasGroup _canvasGroup;

        private readonly Subject<Unit> _preInitializeEvent = new();
        private readonly Subject<Unit> _postInitializeEvent = new();
        private readonly Subject<Unit> _appearEvent = new();
        private readonly Subject<Unit> _appearedEvent = new();
        private readonly Subject<Unit> _disappearEvent = new();
        private readonly Subject<Unit> _disappearedEvent = new();

        #endregion


        #region Properties

        public static List<UIContext> ActiveViews { get; } = new();
        public float LastShowTime { get; private set; }
        public static UIContext FocusContext
        {
            get
            {
                var activeViews = ActiveViews
                    .Where(view => view.gameObject.activeInHierarchy)
                    .Where(view => view is not Sheet.Sheet)
                    .Where(view =>
                    {
                        if (view is not Page.Page page)
                            return true;

                        if (page.UIContainer is PageContainer pageContainer)
                            return pageContainer.DefaultPage != page;

                        return true;
                    });

                if (activeViews.Any())
                    return activeViews
                        .Aggregate((prev, current) =>
                        {
                            return prev.LastShowTime > current.LastShowTime ? prev : current;
                        });

                return null;
            }
        }

        public UIContainer UIContainer { get; set; }
        public CanvasGroup CanvasGroup => _canvasGroup ? _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();
        public EVisibleState VisibleState { get; private set; } = EVisibleState.Disappeared;

        /// <summary>
        /// Event called before Awake
        /// </summary>
        public IObservable<Unit> OnPreInitialize => _preInitializeEvent.Share();

        /// <summary>
        /// Event called immediately after Awake
        /// </summary>
        public IObservable<Unit> OnPostInitialize => _postInitializeEvent.Share();

        /// <summary>
        /// Event that occurs when a UI View starts becoming active.
        /// </summary>
        public IObservable<Unit> OnAppear => _appearEvent.Share();

        /// <summary>
        /// An event that occurs every frame when a UI View is in the process of being activated.
        /// </summary>
        public IObservable<Unit> OnAppearing => OnChangingVisibleState(OnAppear, OnAppeared);

        /// <summary>
        /// Event that occurs when the UI View is completely activated.
        /// </summary>
        public IObservable<Unit> OnAppeared => _appearedEvent.Share();

        /// <summary>
        /// An event that occurs every frame while the UI View is active.
        /// </summary>
        public IObservable<Unit> OnUpdate => OnChangingVisibleState(OnAppeared, OnDisappear);

        /// <summary>
        /// Event fired when a UI View starts to become inactive.
        /// </summary>
        public IObservable<Unit> OnDisappear => _disappearEvent.Share();

        /// <summary>
        /// Event that fires every frame when a UI View is in the process of deactivating.
        /// </summary>
        public IObservable<Unit> OnDisappearing => OnChangingVisibleState(OnDisappear, OnDisappeared);

        /// <summary>
        /// Event that occurs when the UI View is completely deactivated.
        /// </summary>
        public IObservable<Unit> OnDisappeared => _disappearedEvent.Share();

        #endregion


        #region Unity Lifecycle

#if VCONTAINER_SUPPORT
        protected override void OnDestroy()
        {
            base.OnDestroy();
#else
        protected virtual void OnDestroy()
        {
#endif
            _preInitializeEvent.Dispose();
            _postInitializeEvent.Dispose();
            _appearEvent.Dispose();
            _appearedEvent.Dispose();
            _disappearEvent.Dispose();
            _disappearedEvent.Dispose();

            ActiveViews.Remove(this);
        }

        #endregion


        #region Public Methods

        public async UniTask ShowAsync(bool useAnimation = true)
        {
            // Records the last time the View was activated.
            // This is used when finding the FocusView.
            LastShowTime = Time.time;

            // Add the View to the list of active Views.
            ActiveViews.Add(this);

            // Initialize the RectTransform and CanvasGroup.
            // This takes 1 frame.
            var rectTransform = (RectTransform)transform;
            await InitializeRectTransformAsync(rectTransform);
            CanvasGroup.alpha = 1;

            await WhenPreAppearAsync();
            _preInitializeEvent.OnNext(Unit.Default);
            gameObject.SetActive(true);
            _postInitializeEvent.OnNext(Unit.Default);
            VisibleState = EVisibleState.Appearing;
            _appearEvent.OnNext(Unit.Default);

            if (useAnimation)
            {
                if (_animationSetting == EAnimationSetting.Custom)
                    await _showAnimation.AnimateAsync(rectTransform, CanvasGroup);
                else
                    await UIContainer.ShowAnimation.AnimateAsync(transform, CanvasGroup);
            }

            await WhenPostAppearAsync();

            VisibleState = EVisibleState.Appeared;
            _appearedEvent.OnNext(Unit.Default);
        }

        public async UniTask HideAsync(bool useAnimation = true)
        {
            ActiveViews.Remove(this);

            VisibleState = EVisibleState.Disappearing;
            _disappearEvent.OnNext(Unit.Default);

            await UniTask.Yield(cancellationToken: this.GetCancellationTokenOnDestroy());

            await WhenPreDisappearAsync();

            if (useAnimation)
            {
                if (_animationSetting == EAnimationSetting.Custom)
                    await _hideAnimation.AnimateAsync(transform, CanvasGroup);
                else 
                    await UIContainer.HideAnimation.AnimateAsync(transform, CanvasGroup);
            }

            gameObject.SetActive(false);
            await WhenPostDisappearAsync();

            VisibleState = EVisibleState.Disappeared;
            _disappearedEvent.OnNext(Unit.Default);
        }

        #endregion


        #region Virtual Methods

        protected virtual UniTask WhenPreAppearAsync() => UniTask.CompletedTask;
        protected virtual UniTask WhenPostAppearAsync() => UniTask.CompletedTask;

        protected virtual UniTask WhenPreDisappearAsync() => UniTask.CompletedTask;
        protected virtual UniTask WhenPostDisappearAsync() => UniTask.CompletedTask;

        #endregion


        #region Private Methods

        private IObservable<Unit> OnChangingVisibleState(
        IObservable<Unit> begin,
        IObservable<Unit> end)
        {
            return this.UpdateAsObservable()
                       .SkipUntil(begin)
                       .TakeUntil(end)
                       .RepeatUntilDestroy(gameObject)
                       .Share();
        }

        private async UniTask InitializeRectTransformAsync(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            await UniTask.Yield();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
        }

        #endregion
    }
}
#endif