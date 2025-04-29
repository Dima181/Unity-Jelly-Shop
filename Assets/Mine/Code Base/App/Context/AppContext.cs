using Mine.CodeBase.App.Model;
using Mine.CodeBase.Framework.Manager.Sound;
using Mine.CodeBase.Main.Setting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.App.Context
{
    public class AppContext : LifetimeScope
    {
        #region Properties

        [field: SerializeField] public JellyFarmJsonDBModel JellyFarmDBModel { get; private set; }
        [field: SerializeField] public UISetting UISetting { get; private set; }
        [field: SerializeField] public MainSetting MainSetting { get; private set; }

        #endregion


        #region Override Methods

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(UISetting);
            builder.RegisterInstance(MainSetting);

            builder.RegisterEntryPoint<SoundManager>().AsSelf();

            JellyFarmDBModel.LoadDB("JellyPreset", "Currency", "Field", "Upgrade", "Plant");
            builder.RegisterInstance(JellyFarmDBModel);
        }

        #endregion
    }
}
