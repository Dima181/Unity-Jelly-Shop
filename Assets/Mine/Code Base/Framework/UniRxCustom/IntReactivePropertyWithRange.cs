using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Mine.CodeBase.Framework.UniRxCustom
{
    [Serializable]
    public class IntReactivePropertyWithRange : IntReactiveProperty
    {
        #region Preperties

        public int Min { get; }
        public int Max { get; }


        #endregion


        #region Constructors

        public IntReactivePropertyWithRange(int min, int max) : base()
        {
            Min = min;
            Max = max;
        }

        public IntReactivePropertyWithRange(int initialValue, int min, int max) 
            : base(Clamp(initialValue, min, max))
        {
            Min = min;
            Max = max;
        }

        #endregion


        #region Public Methods

        public new int Value
        {
            get => base.Value;
            set => base.Value = Clamp(value, Min, Max);
        }

        #endregion


        #region Private Methods

        private static int Clamp(int value, int min, int max)
        {
            return Mathf.Clamp(value, min, max);
        }

        #endregion
    }
}
