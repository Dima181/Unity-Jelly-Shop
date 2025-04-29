using Mine.CodeBase.Framework.Manager.Sound;
using Mine.CodeBase.Main.Model;
using Mine.CodeBase.Main.Setting;
using VContainer;

namespace Mine.CodeBase.Main.System
{
    public class UpgradeSystem
    {
        #region Fields

        [Inject] private readonly CurrencyModel _currencyModel;
        [Inject] private readonly UpgradeModel _upgradeModel;
        [Inject] private readonly SoundManager _soundManager;
        [Inject] private readonly UISetting _uiSetting;

        #endregion


        #region Public Methods

        public void ApartmentUpgrade()
        {
            if (_upgradeModel.ApartmentLevel.Value >= _upgradeModel.ApartmentMaxLevel)
                return;

            if (_currencyModel.Gold.Value >= _upgradeModel.ApartmentUpgradeCost)
            {
                _currencyModel.Gold.Value -= _upgradeModel.ApartmentUpgradeCost;
                _upgradeModel.ApartmentLevel.Value++;
            }
            else
                _soundManager.PlaySfx(_uiSetting.Fail);
        }

        public void ClickUpgrade()
        {
            if (_upgradeModel.ClickLevel.Value >= _upgradeModel.ClickMaxLevel)
                return;

            if (_currencyModel.Gold.Value >= _upgradeModel.ClickUpgradeCost)
            {
                _currencyModel.Gold.Value -= _upgradeModel.ClickUpgradeCost;
                _upgradeModel.ClickLevel.Value++;
            }
            else
                _soundManager.PlaySfx(_uiSetting.Fail);
        }

        #endregion
    }
}
