using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.CodeBase.Framework.UniRxCustom
{
    /// <summary>
    /// Add UniRx Trigger for OnMouseUp
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnMouseUpTrigger : ObservableTriggerBase
    {
        #region Variable

        Subject<Unit> onMouseUp;

        #endregion

        #region Properties

        public IObservable<Unit> OnMouseUpAsObservable()
        {
            return onMouseUp ??= new Subject<Unit>();
        }

        #endregion

        #region Unity Event

        void OnMouseUp() => 
            onMouseUp?.OnNext(default);

        #endregion

        #region Methods

        protected override void RaiseOnCompletedOnDestroy() => 
            onMouseUp?.OnCompleted();

        #endregion
    }
}
