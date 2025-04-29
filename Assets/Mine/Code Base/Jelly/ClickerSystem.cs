using Mine.CodeBase.App.Common;
using Mine.CodeBase.Framework.Manager.Sound;
using Mine.CodeBase.Main.Model;
using Mine.CodeBase.Main.Setting;
using Unity.Burst.CompilerServices;
using VContainer;

namespace Mine.CodeBase.Jelly
{
    public class ClickerSystem
    {
        #region Fields

        [Inject] readonly CurrencyModel _currencyModel;
        [Inject] readonly GrowUpSystem _growUpSystem;
        [Inject] readonly SoundManager _soundManager;
        [Inject] readonly UISetting _uISetting;

        #endregion


        #region Public Methods

        public void Click(JellyContext jellyContext)
        {
            _soundManager.PlaySfx(_uISetting.Touch);

            StopJelly(jellyContext);
            GetGelatin(jellyContext);
            _growUpSystem.GetExpByClick(jellyContext);
        }

        public void GetGelatin(JellyContext jellyContext)
        {
            _currencyModel.Gelatin.Value += jellyContext.Model.GelatinByClick;
        }

        public void StopJelly(JellyContext jellyContext)
        {
            jellyContext.Animator.SetTrigger(Constants.DoTouch);
            jellyContext.Model.AI.Transition(JellyModel.EJellyState.Idle);
        }

        #endregion
    }
}
