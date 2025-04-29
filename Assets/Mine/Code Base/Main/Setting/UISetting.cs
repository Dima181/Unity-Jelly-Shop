using Mine.CodeBase.Framework.Manager.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mine.CodeBase.Main.Setting
{
    [CreateAssetMenu(fileName = "UISetting", menuName = "ScriptableObjects/UISetting")]
    public class UISetting : ScriptableObject
    {
        #region Properties

        public SoundClip Button => _button;
        public SoundClip Buy => _buy;
        public SoundClip Clear => _clear;
        public SoundClip Fail => _fail;
        public SoundClip Grow => _grow;
        public SoundClip PauseIn => _pauseIn;
        public SoundClip PauseOut => _pauseOut;
        public SoundClip Sell => _sell;
        public SoundClip Touch => _touch;
        public SoundClip UpgradeOrUnlock => _upgradeOrUnlock;

        #endregion


        #region Fields

        [SerializeField] private SoundClip _button;
        [SerializeField] private SoundClip _buy;
        [SerializeField] private SoundClip _clear;
        [SerializeField] private SoundClip _fail;
        [SerializeField] private SoundClip _grow;
        [SerializeField] private SoundClip _pauseIn;
        [SerializeField] private SoundClip _pauseOut;
        [SerializeField] private SoundClip _sell;
        [SerializeField] private SoundClip _touch;
        [SerializeField] private SoundClip _upgradeOrUnlock;

        #endregion
    }
}
