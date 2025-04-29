using Cysharp.Threading.Tasks;
using Mine.CodeBase.Framework.Extension;
using Mine.CodeBase.Framework.Template;
using Mine.CodeBase.Main.Setting;
using Mine.CodeBase.Main.System;
using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Jelly
{
    public class JellyDragPresenter : VObject<JellyContext>, IStartable
    {
        #region Fields

        [Inject] readonly JellyModel _model;
        [Inject] readonly ShopSystem _shopSystem;
        [Inject] readonly MainSetting _mainSetting;

        #endregion

        #region Entry Point

        void IStartable.Start()
        {
            InitializeView();
        }

        void InitializeView()
        {
            Vector3 delta = Vector2.zero;

            Context.OnMouseDownAsObservable()
                .Subscribe(_ =>
                {
                    delta = Camera.main.ScreenToWorldPoint(Input.mousePosition).DropZ() - Context.transform.position.DropZ();
                    Context.GetComponent<SpriteRenderer>().sortingOrder = 11;
                })
                .AddTo(Context.gameObject);

            Context.OnMouseDragAsObservable()
                .Subscribe(_ =>
                {
                    Context.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).DropZ() - delta;
                    _model.AI.Transition(JellyModel.EJellyState.Idle);
                })
                .AddTo(Context.gameObject);

            Context.OnMouseUpAsObservable()
                .DelayFrame(1)
                .Subscribe(_ =>
                {
                    if (_shopSystem.IsActiveSell)
                        _shopSystem.Sell((JellyContext)Context);
                    else if(!Context.transform.position.IsInRange(_mainSetting.MinRange, _mainSetting.MaxRange))
                        Context.transform.position = _mainSetting.RandomPositionInField;

                    Context.GetComponent<SpriteRenderer>().sortingOrder = 0;
                })
                .AddTo(Context.gameObject);

        }

        #endregion
    }
}
