#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer.Unity;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Modal
{
    public sealed class ModalContainer : UIContainer<ModalContainer>, IHasHistory, ISerializationCallbackReceiver
    {
        #region Properties

        [field: SerializeField] public List<Modal> RegisterModalsByPrefab { get; private set; } = new();
#if ADDRESSABLE_SUPPORT
        [field: SerializeField] public List<ComponentReference<Modal>> RegisterModalsByAddressable { get; private set; } = new();

#endif

        public Modal CurrentView => _history.TryPeek(out var currentView) ? currentView : null;

        private Dictionary<Type, Modal> Modals { get; set; }

        #endregion


        #region Fields

        [SerializeField] private Backdrop _modalBackdrop; // Layer to be placed behind the generated modal

        /// <summary>
        /// This is a list of History of Page UI Views. <br/>
        /// History is managed in each Container. <br/>
        /// </summary>
        private Stack<Modal> _history = new();

        #endregion


        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

#if ADDRESSABLE_SUPPORT
            if(InstantiateType == InstantiateType.InstantiateByAddressable) RegisterModalsByPrefab = _registerModalsByAddressable.Select(x => x.LoadAssetAsync<GameObject>().WaitForCompletion().GetComponent<Modal>()).ToList();

#endif

            // All modals registered in modals are registered in the form of a dictionary with Type as the key value.
            Modals = RegisterModalsByPrefab
                .GroupBy(x => x.GetType())
                .Select(x => x.FirstOrDefault())
                .ToDictionary(modal => modal.GetType(), modal => modal);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable) _registerModalsByAddressable.ForEach(x => x.ReleaseAsset());

#endif
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// /// Activate the specified sub-Page and store it in History. <br/>
        /// The method for specifying a Page is to pass the type of the desired Page as a generic type. <br/>
        /// <br/>
        /// If there is a currently activated Page, it deactivates the previous Page and activates the new Page, and updates the FocusView. <br/>
        /// </summary>
        /// <typeparam name="T">Type of Page to be Activated</typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>(
            Action<T> onPreInitialize = null,
            Action<T> onPostInitialize = null) where T : Modal
        {
            if (Modals.TryGetValue(typeof(T), out var modal))
                return await NextAsync(modal as T, onPreInitialize, onPostInitialize);

                Debug.LogError($"Modal not found : {typeof(T)}");
            return null;
        }

        public async UniTask PrevAsync(int count = 1)
        {
            count = Mathf.Clamp(count, 1, _history.Count);

            if (!CurrentView) return;
            if (CurrentView.VisibleState is EVisibleState.Appearing or EVisibleState.Disappearing) return;

            await UniTask.WhenAll(Enumerable.Range(0, count).Select(_ => HideViewAsync()));
        }

        #endregion


        #region Private Methods

        private async UniTask HideViewAsync()
        {
            var currentView = _history.Pop();
            if (currentView.BackDrop)
            {
                await UniTask.WhenAll
                (
                    currentView.BackDrop.DOFade(0, 0.2f).ToUniTask(),
                    currentView.HideAsync()
                );
            }
            else await currentView.HideAsync();

            if (currentView.BackDrop)
                Destroy(currentView.BackDrop.gameObject);

            Destroy(currentView.gameObject);
        }

        private async UniTask<T> NextAsync<T>(
            T nextModal,
            Action<T> onPreInitialize,
            Action<T> onPostInitialize)
            where T : Modal
        {
            if (CurrentView != null
               && CurrentView.VisibleState is EVisibleState.Appearing or EVisibleState.Disappearing)
                return null;

            var backdrop = await ShowBackdrop();

            nextModal.gameObject.SetActive(false);
            nextModal =
#if VCONTAINER_SUPPORT
                VContainerSettings.Instance.GetOrCreateRootLifetimeScopeInstance().Container.Instantiate(nextModal, transform);

#else
                Instantiate(nextModal, transform);

#endif

            nextModal.UIContainer = this;

            nextModal.OnPreInitialize
                .FirstOrDefault()
                .Subscribe(_ =>
                {
                    onPreInitialize?.Invoke(nextModal);
                })
                .AddTo(nextModal);

            nextModal.OnPostInitialize
                .FirstOrDefault()
                .Subscribe(_ =>
                {
                    onPostInitialize?.Invoke(nextModal);
                })
                .AddTo(nextModal);

            if (backdrop)
            {
                nextModal.BackDrop = backdrop;
                if (!nextModal.BackDrop.TryGetComponent<Button>(out var button))
                    button = nextModal.BackDrop.gameObject.AddComponent<Button>();

                button.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        PrevAsync().Forget();
                    });
            }

            _history.Push(nextModal);

#pragma warning disable 4014
            if (nextModal.BackDrop) CurrentView.BackDrop.DOFade(1, 0.2f);
#pragma warning restore 4014

            await CurrentView.ShowAsync();

            return CurrentView as T;
        }

        private async UniTask<CanvasGroup> ShowBackdrop()
        {
            if (!_modalBackdrop)
                return null;

            var backdrop = Instantiate(_modalBackdrop.gameObject, transform, true);
            if (!backdrop.TryGetComponent<CanvasGroup>(out var canvasGroup))
                canvasGroup = backdrop.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            var rectTransform = (RectTransform)backdrop.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localScale = Vector3.one; 
            await UniTask.Yield();
            rectTransform.anchoredPosition = Vector2.zero;
            return canvasGroup;
        }

        #endregion


        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable)
                RegisterModalsByPrefab.Clear();
            else
                RegisterModalsByAddressable.Clear();
#endif
        }

        #endregion
    }
}
#endif