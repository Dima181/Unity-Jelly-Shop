using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.CodeBase.Framework.UniRxCustom
{
    /// <summary>
    /// Add UniRx Trigger for OnMouseDrag
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnMouseDragTrigger : ObservableTriggerBase
    {
        #region Variable

        Subject<Unit> onMouseDrag;

        #endregion

        #region Properties

        public IObservable<Unit> OnMouseDragAsObservable()
        {
            return onMouseDrag ??= new Subject<Unit>();
        }

        #endregion

        #region Unity Event

        void OnMouseDrag() => 
            onMouseDrag?.OnNext(default);

        #endregion

        #region Methods

        protected override void RaiseOnCompletedOnDestroy() => 
            onMouseDrag?.OnCompleted();

        #endregion
    }
}
