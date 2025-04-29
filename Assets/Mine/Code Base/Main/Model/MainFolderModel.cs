using System;
using UnityEngine;

namespace Mine.CodeBase.Main.Model
{
    [Serializable]
    public class MainFolderModel
    {
        #region Properties

        public Transform JellyFolder => _jellyFolder;

        #endregion


        #region Fields

        [SerializeField] private Transform _jellyFolder;

        #endregion
    }
}
