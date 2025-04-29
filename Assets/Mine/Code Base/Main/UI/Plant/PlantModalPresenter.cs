using Mine.CodeBase.Framework.Manager.Sound;
using Mine.CodeBase.Framework.Template;
using Mine.CodeBase.Main.Model;
using Mine.CodeBase.Main.Setting;
using Mine.CodeBase.Main.System;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Main.UI.Plant
{
    public class PlantModalPresenter : VObject<PlantModalContext>, IStartable
    {
        #region Fields

        [Inject] private readonly UpgradeModel _upgradeModel;
        [Inject] private readonly PlantModalContext.UIView _view;
        [Inject] private readonly SoundManager _soundManager;
        [Inject] private readonly UISetting _uISetting;
        [Inject] private readonly UpgradeSystem _upgradeSystem;

        #endregion


        #region Entry Point

        void IStartable.Start()
        {
            InitializeModel();
            InitializeView();
        }

        #endregion


        #region Private Methods

        private void InitializeModel()
        {
            _upgradeModel.ApartmentLevel
                .Subscribe(level =>
                {
                    _view.ApartmentSubText.text = $"Jelly Capacity {(level + 1) * 2}";
                    if (level < _upgradeModel.ApartmentMaxLevel)
                        _view.ApartmentCostText.text = $"{_upgradeModel.ApartmentUpgradeCost}";
                    else
                        _view.ApartmentUpgradeButton.gameObject.SetActive(false);
                })
                .AddTo(Context);

            _upgradeModel.ClickLevel
                .Subscribe(level =>
                {
                    _view.ClickSubText.text = $"Click Production {level + 1}";
                    if(level <  _upgradeModel.ClickMaxLevel)
                        _view.ClickCostText.text = $"{_upgradeModel.ClickUpgradeCost}";
                    else 
                        _view.ClickUpgradeButton.gameObject.SetActive(false);
                })
                .AddTo(Context);
        }

        private void InitializeView()
        {
            _view.ApartmentUpgradeButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _soundManager.PlaySfx(_uISetting.Button);
                    _upgradeSystem.ApartmentUpgrade();
                })
                .AddTo(Context);

            _view.ClickUpgradeButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _soundManager.PlaySfx(_uISetting.Button);
                    _upgradeSystem.ClickUpgrade();
                })
                .AddTo(Context);
        }

        #endregion
    }
}
