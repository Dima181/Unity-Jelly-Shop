using Cysharp.Threading.Tasks;
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Animation;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Runtime
{
    public enum EInstantiateType
    {
        InstantiateByPrefab,
#if ADDRESSABLE_SUPPORT
        InstantiateByAddressable
#endif
    }

    [DefaultExecutionOrder(-1)]
    public abstract class UIContainer : MonoBehaviour
    {
        #region Properties

        [field: SerializeField] public string ContainerName { get; private set; }
        [field: SerializeField] public EInstantiateType InstantiateType { get; private set; } = EInstantiateType.InstantiateByPrefab;
        [field: SerializeField] public ViewShowAnimation ShowAnimation { get; private set; } = new();

        [field: SerializeField] public ViewHideAnimation HideAnimation { get; private set; } = new();

        #endregion


        #region Fields

        private static readonly Dictionary<int, UIContainer> _cached = new();

        [SerializeField] private bool _isDontDestroyOnLoad;

        #endregion


        #region Unity Lifecycle

        protected virtual void Awake()
        {
            if (_isDontDestroyOnLoad)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Returns the most adjacent Container of the Transform received as an argument value.
        /// By default, it is cached and used, and if there is no cached value, it is created and returned.
        /// Whether to cache or not can be determined depending on the settings.
        /// </summary>
        /// <param name="transform">Transform the criteria for finding a container</param>
        /// <param name="useCache">Whether to use caching</param>
        /// <returns></returns>
        public static UIContainer Of(Transform transform, bool useCache = true) =>
            Of((RectTransform)transform, useCache);


        /// <summary>
        /// Returns the RectTransform closest to the Container received as an argument value.
        /// By default, it is cached and used, and if there is no cached value, it is created and returned.
        /// Whether to cache or not can be determined depending on the settings.
        /// </summary>
        /// <param name="rectTransform">RectTransform to find the container</param>
        /// <param name="useCache">Whether to use caching</param>
        /// <returns></returns>
        public static UIContainer Of(RectTransform rectTransform, bool useCache = true)
        {
            var hashCode = rectTransform.GetInstanceID();

            if(useCache && _cached.TryGetValue(hashCode, out var container))
            {
                return container;
            }

            container = rectTransform.GetComponentInParent<UIContainer>();
            if(container != null)
            {
                _cached.Add(hashCode, container);
                return container;
            }

            return null;
        }

        public static async UniTask<bool> BackAsync()
        {
            if (UIContext.FocusContext)
            {
                await ((IHasHistory)UIContext.FocusContext.UIContainer).PrevAsync();
                return true;
            }

            return false;
        }

        #endregion
    }

    public abstract class UIContainer<T> : UIContainer 
        where T : UIContainer<T>
    {
        #region Properties

        public static T Main
        {
            get
            {
                T main = global::Mine.CodeBase.Framework.Manager.UINavigator.Runtime.MainUIContainers.In.GetMain<T>();

                if(main)
                    return main;

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if(scene.isLoaded)
                    {
                        main = 
                            scene.GetRootGameObjects()
                                .Select(root =>
                                {
                                    return root.GetComponentInChildren<T>();
                                })
                                .FirstOrDefault(x => x);

                        MainUIContainers.In.SetMain(main);

                        if(main)
                            return main;
                    }
                }

                return null;
            }
        }

        private static readonly Dictionary<int, T> _cached = new();

        #endregion


        #region Fields

        private static readonly Dictionary<string, T> _containers = new();

        #endregion


        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            if (!string.IsNullOrEmpty(ContainerName))
            {
                _containers[ContainerName] = (T)this;
            }
        }

        protected virtual void OnDestroy()
        {
            _containers.Remove(ContainerName);
        }

        #endregion
    }
}
