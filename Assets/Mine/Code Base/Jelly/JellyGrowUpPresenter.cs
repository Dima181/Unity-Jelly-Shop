using Mine.CodeBase.Framework.Template;
using System;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Jelly
{
    public class JellyGrowUpPresenter : VObject<JellyContext>, IStartable
    {
        #region Fields

        [Inject] private readonly JellyModel _model;
        [Inject] private readonly GrowUpSystem _growUpSystem;
        private readonly int _maxLevel = 3;
        private readonly int _maxExp = 50;

        #endregion

        #region Entry Point

        void IStartable.Start()
        {
            InitializeModel();
            InitializeScheduler();
        }

        void InitializeModel()
        {
            _model.Exp
                .Where(exp => exp >= _maxExp)
                .Subscribe(exp =>
                {
                    _growUpSystem.LevelUp(Context);
                })
                .AddTo(Context.gameObject);

            _model.Level
                .Subscribe(_ =>
                {
                    _growUpSystem.LevelUpEvent(Context);
                })
                .AddTo(Context.gameObject);
        }

        void InitializeScheduler()
        {
            // Gain experience every second
            Observable.Interval(TimeSpan.FromSeconds(1))
                .TakeWhile(_ => _model.Level.Value < _maxLevel)
                .Where(_ => Context.gameObject.activeInHierarchy)
                .Subscribe(_ => _growUpSystem.GetExpByTime(Context))
                .AddTo(Context.gameObject);

            // Get Gelatin every 3 seconds
            Observable.Interval(TimeSpan.FromSeconds(3))
                .Where(_ => Context.gameObject.activeInHierarchy)
                .Subscribe(_ => _growUpSystem.AutoGetGelatin(Context))
                .AddTo(Context.gameObject);
        }

        #endregion
    }
}
