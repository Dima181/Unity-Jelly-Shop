using Mine.CodeBase.Framework.Extension;
using UnityEngine;

namespace Mine.CodeBase.Main.Setting
{
    [CreateAssetMenu(fileName = "MainSetting", menuName = "ScriptableObjects/MainSetting")]
    public class MainSetting : ScriptableObject
    {
        #region Properties

        public RuntimeAnimatorController[] ControllersByLevel => _controllersByLevel;
        public Vector2 MinRange => _minRange;
        public Vector2 MaxRange => _maxRange;
        public Vector2 RandomPositionInField => MinRange.RandomPosition(MaxRange);

        #endregion


        #region Fields

        [SerializeField] private RuntimeAnimatorController[] _controllersByLevel;
        [SerializeField] private Vector2 _minRange;
        [SerializeField] private Vector2 _maxRange;


        #endregion
    }
}
