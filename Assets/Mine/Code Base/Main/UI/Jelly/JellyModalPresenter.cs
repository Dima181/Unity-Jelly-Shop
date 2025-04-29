using Cysharp.Threading.Tasks;
using Mine.CodeBase.App.Model;
using Mine.CodeBase.Framework.Extension;
using Mine.CodeBase.Framework.Manager.Sound;
using Mine.CodeBase.Framework.Template;
using Mine.CodeBase.Main.Setting;
using Mine.CodeBase.Main.System;
using System.Linq;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Main.UI.Jelly
{
    public class JellyModalPresenter : VObject<JellyModalContext>, IStartable
    {
        #region Field

        [Inject] private readonly JellyFarmJsonDBModel _jellyFarmDBModel;
        [Inject] private readonly JellyModalContext.UIView _view;
        [Inject] private readonly JellyModalContext.UIModel _model;
        [Inject] private readonly ShopSystem _shopSystem;
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
            _model.CurrentPage.Value = _jellyFarmDBModel.JellyPresets
                .LastOrDefault(preset => preset.Value<bool>("isUnlocked"))
                .Value<int>("id");

            // When data for the current page of the Model is updated, it is reflected in the UI text.
            _model.CurrentPage
                .Subscribe(index =>
                {
                    _view.JellyIndexText.text = $"#{(index + 1):00}";
                })
                .AddTo(Context.gameObject);

            // Defines how to handle Unlock and Lock Page presentations according to Model updates.
            InitializeUnlock();
            InitializeLock();

        }

        private void InitializeView()
        {
            _view.LeftButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _soundManager.PlaySfx(_uISetting.Button);
                    _model.CurrentPage.Value--;
                })
                .AddTo(Context.gameObject);


            _view.RightButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _soundManager.PlaySfx(_uISetting.Button);
                    _model.CurrentPage.Value++;
                })
                .AddTo(Context.gameObject);


            _view.BuyButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _shopSystem.Buy(_model.CurrentPage.Value);
                })
                .AddTo(Context.gameObject);


            _view.UnlockButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (_shopSystem.Unlock(_model.CurrentPage.Value))
                    {
                        _model.CurrentPage.SetValueAndForceNotify(_model.CurrentPage.Value);
                    }
                })
                .AddTo(Context.gameObject);
        }

        private void InitializeUnlock()
        {
            _model.CurrentPage
                .Select(index => _jellyFarmDBModel.JellyPresets[index])
                .Where(preset => (bool)preset["isUnlocked"])
                .Subscribe(preset =>
                {
                    _view.LockFolder.SetActive(false);
                    _view.UnlockFolder.SetActive(true);
                    _view.UnlockJellyImage.sprite = Resources.Load<Sprite>(preset["jellySpritePath"].ToString());
                    _view.UnlockJellyNameText.text = preset["jellyName"].ToString();
                    _view.UnlockJellyCostText.text = preset["jellyCost"].ToString();
                })
                .AddTo(Context.gameObject);
        }

        private void InitializeLock()
        {
            _model.CurrentPage
                .Select(index => _jellyFarmDBModel.JellyPresets[index])
                .Where(preset => !(bool)preset["isUnlocked"])
                .Subscribe(preset =>
                {
                    _view.UnlockFolder.SetActive(false);
                    _view.LockFolder.SetActive(true);
                    _view.LockJellyImage.sprite = Resources.Load<Sprite>(preset["jellySpritePath"].ToString());
                    _view.LockJellyCostText.text = preset["jellyCost"].ToString();
                })
                .AddTo(Context.gameObject);
        }

        #endregion
    }
}
