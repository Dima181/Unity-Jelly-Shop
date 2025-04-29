#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Modal;
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Page;
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Sheet;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Runtime
{
    public class MainUIContainers : MonoBehaviour
    {
        #region Fields

        private static MainUIContainers _instance;
        [SerializeField] private SheetContainer _mainSheetContainer;
        [SerializeField] private PageContainer _mainPageContainer;
        [SerializeField] private ModalContainer _mainModalContainer;

        #endregion

        #region Preperties

        public static MainUIContainers In => _instance = _instance ? _instance : FindObjectOfType<MainUIContainers>() ?? new GameObject(nameof(MainUIContainers)).AddComponent<MainUIContainers>();

        #endregion


        #region Unity Lifecycle

        private void Awake()
        {
            if (_instance)
                Destroy(gameObject);

            if(!_mainSheetContainer && !_mainPageContainer && !_mainModalContainer)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (scene.isLoaded)
                    {
                        if (!_mainSheetContainer)
                            _mainSheetContainer = 
                                scene.GetRootGameObjects()
                                    .Select(root =>
                                    {
                                        return root.GetComponentInChildren<SheetContainer>();
                                    })
                                    .FirstOrDefault(x => x);

                        if (!_mainPageContainer)
                            _mainPageContainer = 
                                scene.GetRootGameObjects()
                                    .Select(root =>
                                    {
                                        return root.GetComponentInChildren<PageContainer>();
                                    })
                                    .FirstOrDefault(x => x);

                        if (!_mainModalContainer)
                            _mainModalContainer =
                                scene.GetRootGameObjects()
                                    .Select(root =>
                                    {
                                        return root.GetComponentInChildren<ModalContainer>();
                                    })
                                    .FirstOrDefault(x => x);
                    }
                }
            }
        }

        #endregion


        #region Public Methods

        public T GetMain<T>()
            where T : UIContainer<T>
        {
            if (typeof(T) == typeof(SheetContainer))
                return _mainSheetContainer as T;
            
            if (typeof(T) == typeof(PageContainer))
                return _mainPageContainer as T;
            
            if (typeof(T) == typeof(ModalContainer))
                return _mainModalContainer as T;

            return null;
        }

        public void SetMain<T>(T container)
            where T : UIContainer<T>
        {
            if (typeof(T) == typeof(SheetContainer))
                _mainSheetContainer = container as SheetContainer;

            if (typeof(T) == typeof(PageContainer))
                _mainPageContainer = container as PageContainer;

            if (typeof(T) == typeof(ModalContainer))
                _mainModalContainer  = container as ModalContainer;
        }


        #endregion
    }
}
#endif