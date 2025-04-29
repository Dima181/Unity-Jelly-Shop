using Mine.CodeBase.App.Model;
using Mine.CodeBase.Framework.Manager.Sound;
using Mine.CodeBase.Framework.Template;
using Mine.CodeBase.Jelly;
using Mine.CodeBase.Main.Model;
using Mine.CodeBase.Main.Setting;
using UnityEngine;
using VContainer;

namespace Mine.CodeBase.Main.System
{
    public class ShopSystem : VObject<MainContext>
    {
        #region Properties

        public bool IsActiveSell { get; set; }

        #endregion


        #region Fields

        [Inject] private readonly JellyFarmJsonDBModel _jellyFarmDBModel;
        [Inject] private readonly MainFactoryModel _factoryModel;
        [Inject] private readonly CurrencyModel _currencyModel;
        [Inject] private readonly FieldModel _fieldModel;
        [Inject] private readonly UpgradeModel _upgradeModel;
        [Inject] private readonly MainFolderModel _mainFolderModel;
        [Inject] private readonly SaveSystem _saveSystem;
        [Inject] private readonly SoundManager _soundManager;
        [Inject] private readonly UISetting _uiSetting;
        [Inject] private readonly MainSetting _mainSetting;

        #endregion


        #region Public Methods

        public void Sell(JellyContext jellyContext)
        {
            if (jellyContext == null)
                return;

            _soundManager.PlaySfx(_uiSetting.Sell);
            _currencyModel.Gold.Value += jellyContext.Model.JellyPrice;
            jellyContext.Model.Despawn();
            Object.Destroy(jellyContext.gameObject);

            _saveSystem.Save();
        }

        public async void Buy(int index)
        {
            if (_fieldModel.Jellies.Count >= (_upgradeModel.ApartmentLevel.Value + 1) * 2)
                return;

            var jellyCost = (int)_jellyFarmDBModel.JellyPresets[index]["jellyCost"];

            if (_currencyModel.Gold.Value >= jellyCost)
            {
                _soundManager.PlaySfx(_uiSetting.Buy);

                _currencyModel.Gold.Value -= jellyCost;

                var jellyContext = await _factoryModel.JellyFactory[index].LoadAsync();
                var jellyTransform = jellyContext.transform;
                var jellyModel = jellyContext.GetComponent<JellyContext>().Model;
                Context.Container.Inject(jellyModel);
                jellyTransform.SetParent(_mainFolderModel.JellyFolder);
                jellyTransform.position = _mainSetting.RandomPositionInField;
                jellyModel.Respawn();

                _saveSystem.Save();
            }
            else
                _soundManager.PlaySfx(_uiSetting.Fail);
        }

        public bool Unlock(int index)
        {
            var jellyPreset = _jellyFarmDBModel.JellyPresets[index];
            var jellyCost = (int)jellyPreset["jellyCost"];

            if(_currencyModel.Gelatin.Value >= jellyCost)
            {
                _soundManager.PlaySfx(_uiSetting.UpgradeOrUnlock);

                _currencyModel.Gelatin.Value -= jellyCost;
                jellyPreset["isUnlocked"] = true;

                _saveSystem.Save();
                return true;
            }

            _soundManager.PlaySfx(_uiSetting.Fail);

            return false;
        }

        #endregion
    }
}
