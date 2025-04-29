using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Jelly
{
    public class JellyContext : LifetimeScope
    {
        #region Properties

        public JellyModel Model => _model;
        public Animator Animator => GetComponent<Animator>();

        #endregion


        #region Fields

        [SerializeField] private JellyModel _model;

        #endregion


        #region Override Mothods

        protected override void Configure(IContainerBuilder builder)
        {
            // System Register
            builder.Register<ClickerSystem>(Lifetime.Singleton);
            builder.Register<GrowUpSystem>(Lifetime.Singleton);

            // Model Regisrer
            builder.RegisterInstance(Model);

            // View Register
            builder.RegisterComponent(gameObject);
            builder.RegisterComponent(Animator);

            // Presenter Register
            builder.RegisterEntryPoint<JellyAIPresenter>();
            builder.RegisterEntryPoint<JellyClickPresenter>();
            builder.RegisterEntryPoint<JellyDragPresenter>();
            builder.RegisterEntryPoint<JellyGrowUpPresenter>();
        }

        #endregion
    }
}
