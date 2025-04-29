using Mine.CodeBase.Jelly;
using System;
using System.Collections.Generic;

namespace Mine.CodeBase.Main.Model
{
    [Serializable]
    public class FieldModel
    {

        #region Properties

        public List<JellyModel> Jellies
        {
            get => jellies;
            set => jellies = value;
        }

        #endregion
        
        
        #region Fields

        List<JellyModel> jellies;

        #endregion
    }
}
