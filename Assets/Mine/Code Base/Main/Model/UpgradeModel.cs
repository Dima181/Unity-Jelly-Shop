using JetBrains.Annotations;
using Mine.CodeBase.App.Model;
using Mine.CodeBase.Framework.UniRxCustom;
using System;
using System.Linq;
using VContainer;

namespace Mine.CodeBase.Main.Model
{
    public class UpgradeModel
    {
        #region Inner Structs

        [Serializable]
        public struct SaveData
        {
            public int apartmentLevel;
            public int clickLevel;
        }

        #endregion


        #region Properties

        public IntReactivePropertyWithRange ApartmentLevel
        {
            get
            {
                if(_apartmentLevel is null)
                    _apartmentLevel = new IntReactivePropertyWithRange((int)_jellyFarmDBModel.ApartmentLevel, 0, ApartmentMaxLevel);
                return _apartmentLevel;
            }
        }

        public IntReactivePropertyWithRange ClickLevel
        {
            get
            {
                if(_clickLevel is null)
                    _clickLevel = new IntReactivePropertyWithRange((int)_jellyFarmDBModel.ClickLevel, 0, ClickMaxLevel);
                return _clickLevel;
            }
        }

        public int ApartmentMaxLevel => _jellyFarmDBModel.Apartment.Count();
        public int ClickMaxLevel => _jellyFarmDBModel.Click.Count();
        public int ApartmentUpgradeCost => (int)_jellyFarmDBModel.Apartment[ApartmentLevel.Value]["cost"];
        public int ClickUpgradeCost => (int)_jellyFarmDBModel.Click[ClickLevel.Value]["cost"];
        public SaveData Data => new() { apartmentLevel = ApartmentLevel.Value, clickLevel = ClickLevel.Value };


        #endregion


        #region Fields

        [Inject] readonly JellyFarmJsonDBModel _jellyFarmDBModel;
        private IntReactivePropertyWithRange _apartmentLevel;
        private IntReactivePropertyWithRange _clickLevel;

        #endregion
    }
}
