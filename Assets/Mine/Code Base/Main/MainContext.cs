using Mine.CodeBase.Framework.Presenter;
using Mine.CodeBase.Main.Model;
using Mine.CodeBase.Main.System;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Main
{
    public class MainContext : LifetimeScope
    {
        #region Properties

        [field: SerializeField] public MainFolderModel MainFolderModel { get; private set; }

        #endregion

        #region Override Methods

        protected override void Configure(IContainerBuilder builder)
        {
            // System Register
            builder.Register<ShopSystem>(Lifetime.Singleton);
            builder.Register<UpgradeSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SaveSystem>().AsSelf();

            // Model Register
            builder.RegisterInstance(MainFolderModel);
            builder.Register<CurrencyModel>(Lifetime.Singleton);
            builder.Register<FieldModel>(Lifetime.Singleton);
            builder.Register<UpgradeModel>(Lifetime.Singleton);
            builder.Register<MainFactoryModel>(Lifetime.Singleton);

            // Presenter Register
            builder.RegisterEntryPoint<BackPresenter>();
        }

        #endregion
    }
}
