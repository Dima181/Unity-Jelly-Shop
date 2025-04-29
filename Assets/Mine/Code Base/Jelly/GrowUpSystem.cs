using Mine.CodeBase.Framework.Manager.Sound;
using Mine.CodeBase.Main.Model;
using Mine.CodeBase.Main.Setting;
using VContainer;

namespace Mine.CodeBase.Jelly
{
    public class GrowUpSystem
    {
        #region Fields

        [Inject] readonly CurrencyModel _currencyModel;
        [Inject] readonly SoundManager _soundManager;
        [Inject] readonly UISetting _uISetting;
        [Inject] readonly MainSetting _mainSetting;

        #endregion


        #region Public Methods

        public void LevelUp(JellyContext jellyContext)
        {
            jellyContext.Model.Level.Value++;
        }

        public void LevelUpEvent(JellyContext jellyContext)
        {
            _soundManager.PlaySfx(_uISetting.Grow);
            var animator = jellyContext.Animator;
            animator.runtimeAnimatorController = _mainSetting.ControllersByLevel[jellyContext.Model.Level.Value - 1];
            jellyContext.Model.Exp.Value = 0;
        }

        public void GetExpByClick(JellyContext jellyContext)
        {
            jellyContext.Model.Exp.Value++;
        }

        public void GetExpByTime(JellyContext jellyContext)
        {
            jellyContext.Model.Exp.Value++;
        }

        public void AutoGetGelatin(JellyContext jellyContext)
        {
            _currencyModel.Gelatin.Value += jellyContext.Model.GelatinByTime;
        }

        #endregion
    }
}
