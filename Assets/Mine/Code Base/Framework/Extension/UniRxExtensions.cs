using Mine.CodeBase.Framework.UniRxCustom;
using System;
using UniRx;
using UnityEngine;

namespace Mine.CodeBase.Framework.Extension
{
    public static class UniRxExtensions
    {
        #region For UniRx

#if UNIRX_SUPPORT
        public static IObservable<Unit> OnMouseDownAsObservable(this Component component)
        {
            if(!component || !component.gameObject)
                return Observable.Empty<Unit>();

            return component.GetOrAddComponent<ObservableOnMouseDownTrigger>().OnMouseDownAsObservable();
        }

        public static IObservable<Unit> OnMouseDragAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();

            return component.GetOrAddComponent<ObservableOnMouseDragTrigger>().OnMouseDragAsObservable();
        }

        public static IObservable<Unit> OnMouseUpAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();

            return component.GetOrAddComponent<ObservableOnMouseUpTrigger>().OnMouseUpAsObservable();
        }

#endif

        #endregion
    }
}
