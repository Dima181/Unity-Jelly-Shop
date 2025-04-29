using Mine.CodeBase.App.Model;
using Mine.CodeBase.Framework.StateMachine;
using Mine.CodeBase.Framework.UniRxCustom;
using Mine.CodeBase.Main.Model;
using System;
using UnityEngine;
using VContainer;

namespace Mine.CodeBase.Jelly
{
    [Serializable]
    public partial class JellyModel
    {
        #region Constants

        private const string JELLYCOST = "jellyCost";

        #endregion


        #region Inner Structs

        [Serializable]
        public struct SaveData
        {
            public int id;
            public int level;
            public int exp;
        }

        #endregion


        #region Enum

        public enum EJellyState
        {
            Idle = 1,
            Move = 2
        }

        #endregion


        #region Preperties

        public int Id => _id;
        public IntReactivePropertyWithRange Level { get; } = new(1, 1, 3);
        public IntReactivePropertyWithRange Exp { get; } = new(0, 50);
        public int GelatinByClick => (Id + 1) * Level.Value * (_upgradeModel.ClickLevel.Value + 1);
        public int GelatinByTime => (Id + 1) * Level.Value;
        public int JellyPrice => _jellyFarmDBModel.JellyPresets[Id].Value<int>(JELLYCOST) * Level.Value;
        public StateMachine<EJellyState> AI { get; } = new();
        public SaveData Data => new() { id = Id, level = Level.Value, exp = Exp.Value };

        #endregion


        #region Fields

        [Inject] readonly FieldModel _fieldModel;
        [Inject] readonly UpgradeModel _upgradeModel;
        [Inject] readonly JellyFarmJsonDBModel _jellyFarmDBModel;

        [SerializeField] private int _id;

        #endregion


        #region Public Methods

        public void Respawn()
        {
            Level.Value = 0;
            Exp.Value = 0;
            _fieldModel.Jellies.Add(this);
        }

        public void Load(int level, int exp)
        {
            Level.Value = level;
            Exp.Value = exp;
        }

        public void Despawn()
        {
            _fieldModel.Jellies.Remove(this);
        }

        #endregion
    }
}
