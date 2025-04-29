using Mine.CodeBase.Framework.Extension;
using Mine.CodeBase.Framework.Template;
using System;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Jelly
{
    public class JellyClickPresenter : VObject<JellyContext>, IStartable
    {
        #region Fields

        [Inject] private readonly JellyModel _model;
        [Inject] private readonly ClickerSystem _clickerSystem;

        #endregion


        #region Entry Point

        void IStartable.Start()
        {
            InitializeView();
        }

        private void InitializeView()
        {
            Context.OnMouseDownAsObservable()
                .Subscribe(_ =>
                {
                    _clickerSystem.Click(Context);
                })
                .AddTo(Context.gameObject);
        }

        #endregion
    }
}
