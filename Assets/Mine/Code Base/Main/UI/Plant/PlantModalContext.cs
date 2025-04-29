using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Modal;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Main.UI.Plant
{
    public class PlantModalContext : Modal
    {
        #region Inner Classes

        [Serializable]
        public class UIView
        {
            [field: SerializeField] public TextMeshProUGUI ApartmentSubText { get; private set; }
            [field: SerializeField] public TextMeshProUGUI ApartmentCostText { get; private set; }
            [field: SerializeField] public Button ApartmentUpgradeButton { get; private set; }
            [field: SerializeField] public TextMeshProUGUI ClickSubText { get; private set; }
            [field: SerializeField] public TextMeshProUGUI ClickCostText { get; private set; }
            [field: SerializeField] public Button ClickUpgradeButton { get; private set; }
        }

        #endregion

        #region Properties

        [field: SerializeField] public UIView View { get; private set; }

        #endregion

        #region Override Methods

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(View);

            builder.RegisterEntryPoint<PlantModalPresenter>();
        }

        #endregion
    }
}
