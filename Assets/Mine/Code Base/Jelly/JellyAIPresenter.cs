using DG.Tweening;
using Mine.CodeBase.App.Common;
using Mine.CodeBase.Framework.Template;
using Mine.CodeBase.Main.Setting;
using System;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Mine.CodeBase.Jelly
{
    public class JellyAIPresenter : VObject<JellyContext>, IStartable
    {
        #region Fields

        [Inject] private readonly JellyModel _model;
        [Inject] private readonly MainSetting _mainSetting;
        private IDisposable _disposable;

        #endregion


        #region Entry Point

        void IStartable.Start()
        {
            InitializeModel();
        }

        #endregion


        #region Private Methods

        private void InitializeModel()
        {
            // AI State Definition
            InitializeIdleState();
            InitializeMoveState();

            // AI starts working
            _model.AI.StartFSM(JellyModel.EJellyState.Idle);
        }

        private void InitializeIdleState()
        {
            _model.AI.OnBeginState(JellyModel.EJellyState.Idle)
                .Subscribe(_ =>
                {
                    Context.Animator.SetBool(Constants.IsWalk, false);
                    _disposable = Observable.Timer(TimeSpan.FromSeconds(Random.Range(0.5f, 3f)))
                        .Subscribe(_ =>
                        {
                            _model.AI.Transition(JellyModel.EJellyState.Move);
                        })
                        .AddTo(Context.gameObject);
                })
                .AddTo(Context.gameObject);


            _model.AI.OnEndState(JellyModel.EJellyState.Idle)
                .Subscribe(_ =>
                {
                    _disposable?.Dispose();
                })
                .AddTo(Context.gameObject);
        }

        private void InitializeMoveState()
        {
            _model.AI.OnBeginState(JellyModel.EJellyState.Move)
                .Subscribe(_ =>
                {
                    Context.GetComponent<Animator>().SetBool(Constants.IsWalk, true);

                    // Move to a random location at a constant speed
                    var randomPosition = _mainSetting.RandomPositionInField;
                    Context.GetComponent<SpriteRenderer>().flipX = Context.transform.position.x > randomPosition.x;
                    Context.transform.DOMove(randomPosition, 1f)
                        .SetSpeedBased()
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            _model.AI.Transition(JellyModel.EJellyState.Idle);
                        });
                })
                .AddTo(Context.gameObject);


            _model.AI.OnEndState(JellyModel.EJellyState.Move)
                .Subscribe(_ =>
                {
                    Context.transform.DOKill();
                })
                .AddTo(Context.gameObject);
        }

        #endregion
    }
}
