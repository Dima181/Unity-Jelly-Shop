using Mine.CodeBase.App.Model;
using Mine.CodeBase.Framework.UniRxCustom;
using System;
using VContainer;

namespace Mine.CodeBase.Main.Model
{
    [Serializable]
    public class CurrencyModel
    {
        #region Inner Structs

        [Serializable]
        public struct SaveData
        {
            public int gelatin;
            public int gold;
        }

        #endregion


        #region Proreties

        public IntReactivePropertyWithRange Gelatin
        {
            get
            {
                if (_gelatin is null)
                    _gelatin = new IntReactivePropertyWithRange((int)_jellyFarmDBModel.Gelatin, 0, 999999999);
                return _gelatin;
            }
        }

        public IntReactivePropertyWithRange Gold
        {
            get
            {
                if (_gold is null)
                    _gold = new IntReactivePropertyWithRange((int)_jellyFarmDBModel.Gold, 0, 999999999);
                return _gold;
            }
        }

        public SaveData Data => new() { gelatin = Gelatin.Value, gold = Gold.Value };

        #endregion


        #region Fields

        [Inject] private JellyFarmJsonDBModel _jellyFarmDBModel;
        private IntReactivePropertyWithRange _gelatin;
        private IntReactivePropertyWithRange _gold;

        #endregion
    }
}
