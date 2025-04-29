#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Mine.CodeBase.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Sheet
{
    public sealed class SheetContainer : UIContainer<SheetContainer>, ISerializationCallbackReceiver
    {
        #region Properties

        [field: SerializeField] public List<Sheet> RegisterSheetsByPrefab { get; private set; } = new();

#if ADDRESSABLE_SUPPORT
        [field: SerializeField] public List<ComponentReference<Sheet>> RegisterSheetsByAddressable { get; private set; } = new();

#endif

        [field: SerializeField] public bool HasDefault { get; private set; }

        public Dictionary<Type, Sheet> Sheets { get; set; }
        public Sheet CurrentView => _currentView;


        #endregion


        #region Fields

        /// <summary>
        /// Current Sheet of Current Container
        /// </summary>
        private Sheet _currentView;

        #endregion


        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

#if ADDRESSABLE_SUPPORT
            if(InstantiateType == InstantiateType.InstantiateByAddressable) RegisterSheetsByPrefab = RegisterSheetsByAddressable.Select(x => x.LoadAssetAsync<GameObject>().WaitForCompletion().GetComponent<Sheet>()).ToList();

#endif

            // Register all sheets registered in sheets in the form of a dictionary with Type as the key value.
            RegisterSheetsByPrefab = RegisterSheetsByPrefab
                .Select(x => x.IsRecycle ? Instantiate(x, transform) : x)
                .GroupBy(x => x.GetType())
                .Select(x => x.FirstOrDefault())
                .ToList();

            Sheets = RegisterSheetsByPrefab
                .ToDictionary(sheet => sheet.GetType(), sheet => sheet);

            RegisterSheetsByPrefab
                .Where(x => x.IsRecycle)
                .ForEach(x =>
                {
                    x.UIContainer = this;
                    x.gameObject.SetActive(false);
                });
        }

        private void OnEnable()
        {
            // Activate the initial sheet
            if(HasDefault && RegisterSheetsByPrefab.Any())
            {
                var nextSheet = Sheets[RegisterSheetsByPrefab.First().GetType()];

                if (_currentView)
                {
                    _currentView.HideAsync(false).Forget();
                    if(!_currentView.IsRecycle)
                        Destroy(_currentView.gameObject);
                }

                _currentView = nextSheet.IsRecycle ? nextSheet : Instantiate(nextSheet, transform);
                _currentView.ShowAsync(false).Forget();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable) RegisterSheetsByAddressable.ForEach(x => x.ReleaseAsset());

#endif
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// /// Activates the specified sub-Sheet. <br/>
        /// <br/>
        /// If there is a currently activated Sheet, it deactivates the previous Sheet and activates the new Sheet, and updates the FocusView. <br/>
        /// </summary>
        /// <typeparam name="T">Type of Sheet to be activated</typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>(
            Action<T> onPreInitialize = null,
            Action<T> onPostInitialize = null)
            where T : Sheet
        {
            if (Sheets.TryGetValue(typeof(T), out var sheet))
                return await NextAsync(sheet as T, onPreInitialize, onPostInitialize);

            Debug.LogError($"Sheed not found : {typeof(T)}");
            return null;
        }

        #endregion


        #region Private Methods

        private async UniTask<T> NextAsync<T>(
            T nextSheet,
            Action<T> onPreInitialize = null,
            Action<T> onPostInitialize = null)
            where T : Sheet
        {
            if (_currentView != null && _currentView.VisibleState is EVisibleState.Appearing or EVisibleState.Disappearing)
                return null;

            if (_currentView != null && _currentView == nextSheet)
                return null;

            var prevSheet = _currentView;

            nextSheet.gameObject.SetActive(false);
            nextSheet = nextSheet.IsRecycle
                ? nextSheet
                :
#if VCONTAINER_SUPPORT
                VContainerSettings.Instance.RootLifetimeScope.Container.Instantiate(nextSheet, transform);

#else
                Instantiate(nextSheet, transform);

#endif

            nextSheet.UIContainer = this;

            nextSheet.OnPreInitialize
                .FirstOrDefault()
                .Subscribe(_ =>
                {
                    onPreInitialize?.Invoke(nextSheet);
                })
                .AddTo(nextSheet);

            nextSheet.OnPostInitialize
                .FirstOrDefault()
                .Subscribe(_ =>
                {
                    onPostInitialize?.Invoke(nextSheet);
                })
                .AddTo(nextSheet);

            _currentView = nextSheet;

            if (prevSheet != null)
            {
                prevSheet.HideAsync().Forget();
                if (!prevSheet.IsRecycle)
                    Destroy(prevSheet.gameObject);
            }

            await _currentView.ShowAsync();

            return _currentView as T;
        }

        #endregion


        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable)
                RegisterSheetsByPrefab.Clear();
            else
                RegisterSheetsByAddressable.Clear();
#endif

        }

        #endregion
    }
}
#endif