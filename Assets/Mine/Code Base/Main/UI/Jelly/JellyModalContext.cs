using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Modal;
using Mine.CodeBase.Framework.UniRxCustom;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Main.UI.Jelly
{
    public class JellyModalContext : Modal
    {
        #region Inner Classes

        [global::System.Serializable]
        public class UIView
        {
            [field: SerializeField] public TextMeshProUGUI JellyIndexText { get; private set; }
            [field: SerializeField] public Button LeftButton { get; private set; }
            [field: SerializeField] public Button RightButton { get; private set; }

            [field: Header("Unlock")]
            [field: SerializeField] public GameObject UnlockFolder { get; private set; }
            [field: SerializeField] public Image UnlockJellyImage { get; private set; }
            [field: SerializeField] public TextMeshProUGUI UnlockJellyNameText { get; private set; }
            [field: SerializeField] public TextMeshProUGUI UnlockJellyCostText { get; private set; }
            [field: SerializeField] public Button BuyButton { get; private set; }

            [field: Header("Lock")]
            [field: SerializeField] public GameObject LockFolder { get; private set; }
            [field: SerializeField] public Image LockJellyImage { get; private set; }
            [field: SerializeField] public TextMeshProUGUI LockJellyCostText { get; private set; }
            [field: SerializeField] public Button UnlockButton { get; private set; }
        }

        [global::System.Serializable]
        public class UIModel
        {
            public IntReactivePropertyWithRange CurrentPage { get; } = new(0, 11);

        }

        #endregion

        #region Properties

        [field: SerializeField] public UIView View { get; private set; }
        public UIModel Model => Container.Resolve<UIModel>();

        #endregion

        #region Override Methods

        protected override void Configure(IContainerBuilder builder)
        {
            // Model Register
            builder.Register<UIModel>(Lifetime.Scoped);

            // View Register
            builder.RegisterInstance(View);

            // Presenter Register
            builder.RegisterEntryPoint<JellyModalPresenter>();
        }

        #endregion
    }
}
