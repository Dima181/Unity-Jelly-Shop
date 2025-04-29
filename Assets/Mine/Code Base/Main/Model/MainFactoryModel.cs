using Mine.CodeBase.App.Model;
using Mine.CodeBase.Framework.Manager.ResourceFactory;
using Mine.CodeBase.Jelly;
using System.Collections.Generic;

namespace Mine.CodeBase.Main.Model
{
    public class MainFactoryModel
    {
        #region Properties

        public List<ResourceFactory<JellyContext>> JellyFactory { get; } = new();

        #endregion


        #region Constructor

        public MainFactoryModel(JellyFarmJsonDBModel jellyFarmDBModel)
        {
            for (int i = 0; i < jellyFarmDBModel.JellyPresets.Count; i++)
            {
                JellyFactory.Add(ResourceFactory<JellyContext>.Builder.ByInject.Build(jellyFarmDBModel.JellyPresets[i].Value<string>("jellyPrefabPath")));
            }
        }

        #endregion
    }
}
