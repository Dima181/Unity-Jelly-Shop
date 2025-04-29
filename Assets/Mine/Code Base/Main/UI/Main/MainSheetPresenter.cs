using Cysharp.Threading.Tasks;
using Mine.CodeBase.Framework.Manager.Sound;
using Mine.CodeBase.Framework.Template;
using Mine.CodeBase.Main.Model;
using Mine.CodeBase.Main.Setting;
using Mine.CodeBase.Main.System;
using UniRx;
using VContainer;
using VContainer.Unity;
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Modal;
using Mine.CodeBase.Main.UI.Jelly;
using Mine.CodeBase.Main.UI.Plant;
using UniRx.Triggers;

namespace Mine.CodeBase.Main.UI.Main
{
    public class MainSheetPresenter : VObject<MainSheetContext>, IStartable
    {
        #region Fields

        [Inject] private readonly CurrencyModel _currencyModel;
        [Inject] private readonly ShopSystem _shopSystem;
        [Inject] private readonly MainSheetContext.UIView _view;
        [Inject] private readonly SoundManager _soundManager;
        [Inject] private readonly UISetting _uISetting;

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
            _currencyModel.Gelatin
                .Subscribe(gelatin =>
                {
                    _view.GelatinText.text = gelatin.ToString("N0");
                })
                .AddTo(Context);

            _currencyModel.Gold
                .Subscribe(gold =>
                {
                    _view.GoldText.text = gold.ToString("N0");
                })
                .AddTo(Context);
        }

        private void InitializeView()
        {
            _view.JellyButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ModalContainer.Main.NextAsync<JellyModalContext>().Forget();
                    _soundManager.PlaySfx(_uISetting.Button);
                })
                .AddTo(Context);

            _view.PlantButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ModalContainer.Main.NextAsync<PlantModalContext>().Forget();
                    _soundManager.PlaySfx(_uISetting.Button);
                })
                .AddTo(Context);


            _view.SellButton
                .OnPointerEnterAsObservable()
                .Subscribe(_ =>
                {
                    _shopSystem.IsActiveSell = true;
                })
                .AddTo(Context);

            _view.SellButton
                .OnPointerExitAsObservable()
                .Subscribe(_ =>
                {
                    _shopSystem.IsActiveSell = false;
                })
                .AddTo(Context);
        }

        #endregion
    }
}
