using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.CodeBase.Framework.UniRxCustom
{
    /// <summary>
    /// Add UniRx Trigger for OnMouseDown
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnMouseDownTrigger : ObservableTriggerBase
    {
        #region Variable

        private Subject<Unit> _onMouseDown;

        #endregion


        #region Properties

        public IObservable<Unit> OnMouseDownAsObservable()
        {
            return _onMouseDown ??= new Subject<Unit>();
        }

        #endregion


        #region Unity Event

        private void OnMouseDown() => 
            _onMouseDown?.OnNext(default);

        #endregion


        #region Methods

        protected override void RaiseOnCompletedOnDestroy() => 
            _onMouseDown?.OnCompleted();

        #endregion
    }
}
