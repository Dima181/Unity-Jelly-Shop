using Cysharp.Threading.Tasks;
using Mine.CodeBase.App.Model;
using Mine.CodeBase.Framework.Manager.Sound;
using Mine.CodeBase.Framework.Template;
using Mine.CodeBase.Jelly;
using Mine.CodeBase.Main.Model;
using Mine.CodeBase.Main.Setting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Main.System
{
    public class SaveSystem : VObject<MainContext>, IStartable
    {
        #region Fields

        [Inject] private readonly JellyFarmJsonDBModel _jellyFarmDBModel;
        [Inject] private readonly MainFactoryModel _factoryModel;
        [Inject] private readonly MainFolderModel _mainFolderModel;
        [Inject] private readonly FieldModel _fieldModel;
        [Inject] private readonly CurrencyModel _currencyModel;
        [Inject] private readonly UpgradeModel _upgradeModel;
        [Inject] private readonly SoundManager _soundManager;
        [Inject] private readonly MainSetting _mainSetting;

        #endregion


        #region Entry Point

        public async void Start()
        {
            /*throw new global::System.NotImplementedException();*/
            _fieldModel.Jellies = await LoadJellies();
            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    Save();
                });
        }

        #endregion


        #region Public Methods

        public void Save()
        {
            _jellyFarmDBModel.DB["Currency"] = JObject.FromObject(_currencyModel.Data);
            _jellyFarmDBModel.DB["Field"]["jellies"] = JArray.FromObject(_fieldModel.Jellies.Select(model => model.Data));
            _jellyFarmDBModel.DB["Plant"] = JObject.FromObject(_upgradeModel.Data);
            _jellyFarmDBModel.SaveDB();
        }

        private async UniTask<List<JellyModel>> LoadJellies()
        {
            var jellies = await UniTask.WhenAll(_jellyFarmDBModel.Jellies.Select(async data =>
            {
                var jellyContext = await _factoryModel.JellyFactory[(int)data["id"]].LoadAsync();
                jellyContext.gameObject.SetActive(false);
                Transform jellyTransform = jellyContext.transform;
                jellyTransform.SetParent(_mainFolderModel.JellyFolder);
                jellyTransform.position = _mainSetting.RandomPositionInField;
                var jellyModel = jellyContext.Model;
                Context.Container.Inject(jellyModel);
                jellyModel.Load(data.Value<int>("level"), data.Value<int>("exp"));
                return jellyContext;
            }));

            return jellies.Select(jelly =>
            {
                jelly.gameObject.SetActive(true);
                return jelly.Model;
            }).ToList();
        }

        #endregion
    }
}
