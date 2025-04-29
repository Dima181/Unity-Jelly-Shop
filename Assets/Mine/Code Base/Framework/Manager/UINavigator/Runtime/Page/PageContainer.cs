#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using Mine.CodeBase.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Page
{
    public sealed class PageContainer : UIContainer<PageContainer>, IHasHistory, ISerializationCallbackReceiver
    {
        #region Properties

        public List<Page> RegisterPagesByPrefab => _registerPagesByPrefab;

#if ADDRESSABLE_SUPPORT
        public List<ComponentReference<Page>> RegisterPagesByAddressable => _registerPagesByAddressable;

#endif

        public bool HasDefault => _hasDefault;

        public Dictionary<Type, Page> Pages { get; set; }
        public Page DefaultPage => _defaultPage;

        public Page CurrentView => _history.TryPeek(out var currentView) ? currentView : null;
        private bool IsRemainHistory => _defaultPage ? _history.Count > 1 : _history.Count > 0;

        #endregion


        #region Fields

        [SerializeField] private List<Page> _registerPagesByPrefab = new(); // List of pages that can be created in the container

#if ADDRESSABLE_SUPPORT
        [SerializeField] private List<ComponentReference<Page>> _registerPagesByAddressable = new(); // List of pages that can be created in the container

#endif
        [SerializeField] private bool _hasDefault; // Whether to activate the initial sheet when starting

        private Page _defaultPage;

        /// <summary>
        /// This is a list of the History of Page UI Views. <br/>
        /// History is managed in each Container. <br/>
        /// </summary>
        private Stack<Page> _history = new();

        #endregion


        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

#if ADDRESSABLE_SUPPORT
            if(InstantiateType == InstantiateType.InstantiateByAddressable) RegisterPagesByPrefab = RegisterPagesByAddressable.Select(x => x.LoadAssetAsync<GameObject>().WaitForCompletion().GetComponent<Page>()).ToList();

#endif

            _registerPagesByPrefab = _registerPagesByPrefab
                .Select(x => x.IsRecycle ? UnityEngine.Object.Instantiate(x, transform) : x)
                .GroupBy(x => x.GetType())
                .Select(x => x.FirstOrDefault())
                .ToList();

            Pages = _registerPagesByPrefab
                .ToDictionary(page => page.GetType(), page => page);

            _registerPagesByPrefab
                .Where(x => x.IsRecycle)
                .ForEach(x =>
                {
                    x.UIContainer = this;
                    x.gameObject.SetActive(false);
                });

            if (_hasDefault && _registerPagesByPrefab.Any())
                _defaultPage = Pages[_registerPagesByPrefab.First().GetType()];
        }

        private void OnEnable()
        {
            if (_defaultPage && Pages.TryGetValue(_defaultPage.GetType(), out var nextPage))
            {
                if (CurrentView)
                {
                    CurrentView.HideAsync(false).Forget();
                    if (!CurrentView.IsRecycle)
                        Destroy(CurrentView.gameObject);
                }

                nextPage = nextPage.IsRecycle ? nextPage : Instantiate(nextPage, transform);
                nextPage.ShowAsync(false).Forget();
                _history.Push(nextPage);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
#if ADDRESSABLE_SUPPORT
            if(InstantiateType == InstantiateType.InstantiateByAddressable) RegisterPagesByAddressable.ForEach(x => x.ReleaseAsset());

#endif
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Activates the specified sub-Page and stores it in History. <br/>
        /// The method for specifying a Page is to pass the type of the desired Page as a generic type. <br/>
        /// <br/>
        /// If there is a currently activated Page, it deactivates the previous Page and activates the new Page, and updates the FocusView. <br/>
        /// </summary>
        /// <typeparam name="T">Type of Page to be Activated</typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>(
            Action<T> onPreInitialize = null,
            Action<T> onPostInitialize = null) where T : Page
        {
            if (Pages.TryGetValue(typeof(T), out var page))
                return await NextAsync(page as T, onPreInitialize, onPostInitialize);

            Debug.LogError($"Page not found : {typeof(T)}");
            return null;
        }

        public async UniTask PrevAsync(int count = 1)
        {
            count = Mathf.Clamp(count, 1, _history.Count);

            if (!IsRemainHistory)
                return;

            if (CurrentView.VisibleState is EVisibleState.Appearing or EVisibleState.Disappearing)
                return;

            CurrentView.HideAsync().Forget();

            for (int i = 0; i < count; i++)
            {
                if (!CurrentView.IsRecycle)
                    Destroy(CurrentView.gameObject);

                _history.Pop();
            }

            if (!CurrentView)
                return;

            await CurrentView.ShowAsync();
        }

        public async UniTask ResetAsync()
        {
            await PrevAsync(_history.Count);
        }

        #endregion


        #region Private Methods

        private async UniTask<T> NextAsync<T>(
            T nextPage,
            Action<T> onPreInitialize = null,
            Action<T> onPostInitialize = null)
            where T : Page
        {
            if (CurrentView
               && (CurrentView.VisibleState is EVisibleState.Appearing or EVisibleState.Disappearing || CurrentView == nextPage))
                return null;

            // Disable and Instantiate the prefab to do some pre-processing before calling Awake on the created object.
            // Awake is called when the object is first activated after being created while disabled.
            // Also, the creation method is different depending on whether the VContainer library is used.
            nextPage.gameObject.SetActive(false);

            nextPage = nextPage.IsRecycle
                ? nextPage
                :
#if VCONTAINER_SUPPORT
                VContainerSettings.Instance.RootLifetimeScope.Container.Instantiate(nextPage, transform);

#else
                Instantiate(nextPage, transform);

#endif

            nextPage.UIContainer = this;

            // Register events to be processed before and after calling Awake.
            nextPage.OnPreInitialize
                .FirstOrDefault()
                .Subscribe(_ =>
                {
                    onPreInitialize?.Invoke(nextPage);
                })
                .AddTo(nextPage);

            nextPage.OnPostInitialize
                .FirstOrDefault()
                .Subscribe(_ =>
                {
                    onPostInitialize?.Invoke(nextPage);
                })
                .AddTo(nextPage);

            // If there is a current Page, deactivate the current Page and activate a new Page.
            // At this time, the newly activated Page is saved in History.
            if (CurrentView)
                CurrentView.HideAsync().Forget();

            _history.Push(nextPage);
            await CurrentView.ShowAsync();

            return CurrentView as T;
        }

        #endregion


        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable)
                RegisterPagesByPrefab.Clear();
            else
                RegisterPagesByAddressable.Clear();
#endif
        }

        #endregion
    }
}
#endif