#if ADDRESSABLE_SUPPORT
using UnityEngine.AddressableAssets;
#endif
#if VCONTAINER_SUPPORT
using VContainer.Unity;
#endif
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mine.CodeBase.Framework.Manager.ResourceFactory
{
    public class ResourceFactory<T>
        where T : Component
    {
        #region Inner Classes

        public class ResourceFactoryBuilder : ResourceFactoryBuilderBase<ResourceFactoryBuilder>
        {
            public ResourceFactory<T> Build(string path)
            {
                var resourceFactory = new ResourceFactory<T>(
                    path: path,
                    isAddressable: IsAddressable,
                    isInject: IsInject
                    );

                return resourceFactory;
            }
        }

        #endregion


        #region Fields

        protected readonly string path;
        private readonly bool isAddressable;
        private readonly bool isInject;

        #endregion


        #region Constructor

        public ResourceFactory(string path, bool isAddressable, bool isInject)
        {
            this.path = path;
            this.isAddressable = isAddressable;
            this.isInject = isInject;
        }

        #endregion

        
        #region Static Methods

        public static ResourceFactoryBuilder Builder => new();

        #endregion


        #region Virtual Methods

        public virtual T Load(Transform parent = null)
        {
            T resource = null;

            if (isAddressable)
            {
#if ADDRESSABLE_SUPPORT

                resource = typeof(Component).IsAssignableFrom(typeof(T)) ? Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion().GetComponent<T>() : Addressables.LoadAssetAsync<T>(path).WaitForCompletion();

#endif
            }
            else 
                resource = Resources.Load<T>(path);

            if (!resource)
                return null;

            if(TryInstantiate(parent, resource, out var instance))
                return instance;

            return resource;
        }

        public virtual async UniTask<T> LoadAsync(Transform parent = null)
        {
            T resource = null;

            if (isAddressable)
            {
#if ADDRESSABLE_SUPPORT
                if (typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
                    resource = (await Addressables.LoadAssetAsync<GameObject>(path)).GetComponent<T>();
                else resource = await Addressables.LoadAssetAsync<T>(path);
#endif
            }
            else 
                resource = await Resources.LoadAsync<T>(path) as T;

            if (!resource) 
                return null;

            await WaitForRootLifetimeScopeAsync();

            if (TryInstantiate(parent, resource, out var instance))
                return instance;

            Debug.Log($"[LoadAsync] Instantiation {(instance != null ? "successful" : "failed")}");


            return resource;
        }

        public virtual void Release(T resource)
        {
            if (resource is Component component)
                Object.Destroy(component.gameObject);
            else if(resource is GameObject gameObject)
                Object.Destroy(gameObject);
            else
            {
#if ADDRESSABLE_SUPPORT
                if (isAddressable) Addressables.Release(resource);
#endif
            }
        }

        #endregion


        #region Private Methods

        private bool TryInstantiate(Transform parent, T resource, out T result) 
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
            {
#if ADDRESSABLE_SUPPORT

                GameObject handle = null;
                if (resource is Component resourceComponent) handle = resourceComponent.gameObject;
                else if (resource is GameObject resourceGameObject) handle = resourceGameObject;

#endif

                T instance = null;
                if (isInject)
                {
#if VCONTAINER_SUPPORT

                    instance = VContainerSettings.Instance.GetOrCreateRootLifetimeScopeInstance().Container.Instantiate(resource);

#endif
                }
                else
                    instance = Object.Instantiate(resource);

                instance.name = instance.name.Replace("(Clone)", string.Empty);
#if ADDRESSABLE_SUPPORT

                if (instance is Component instanceComponent)
                    instanceComponent.OnDestroyAsObservable().Where(_ => isAddressable).Subscribe(_ => Addressables.Release(handle));
                else if (instance is GameObject instanceGameObject)
                    instanceGameObject.OnDestroyAsObservable().Where(_ => isAddressable).Subscribe(_ => Addressables.Release(handle));

#endif

                result = instance;
                return true;
            }

            result = null;
            return false;
        }

        private async UniTask WaitForRootLifetimeScopeAsync()
        {
            await UniTask.WaitUntil(() =>
                VContainerSettings.Instance != null &&
                VContainerSettings.Instance.GetOrCreateRootLifetimeScopeInstance() != null &&
                VContainerSettings.Instance.GetOrCreateRootLifetimeScopeInstance().Container != null);
        }

        #endregion
    }
}
