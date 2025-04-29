#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Animation
{
    [Serializable]
    public class ViewShowAnimation
    {
        #region Properties

        public MoveShowAnimation MoveAnimation => _moveAnimation;
        public RotateShowAnimation RotateAnimation => _rotateAnimation;
        public ScaleShowAnimation CaleAnimation => _scaleAnimation;
        public FadeShowAnimation AdeAnimation => _fadeAnimation;

        #endregion


        #region Fields

        [SerializeReference] private MoveShowAnimation _moveAnimation;
        [SerializeReference] private RotateShowAnimation _rotateAnimation;
        [SerializeReference] private ScaleShowAnimation _scaleAnimation;
        [SerializeReference] private FadeShowAnimation _fadeAnimation;

        #endregion


        #region Public Methods

        public async UniTask AnimateAsync(Transform transform, CanvasGroup canvasGroup) =>
            await AnimateAsync((RectTransform)transform, canvasGroup);

        public async UniTask AnimateAsync(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            // ReSharper disable once HeapView.ObjectAllocation
            await UniTask.WhenAll(
                _moveAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                _rotateAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                _scaleAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                _fadeAnimation?.AnimateAsync(canvasGroup) ?? UniTask.CompletedTask
                );
        }

        #endregion
    }

    [Serializable]
    public class ViewHideAnimation
    {
        #region Properties

        public MoveHideAnimation MoveAnimation => _moveAnimation;
        public RotateHideAnimation RotateAnimation => _rotateAnimation;
        public ScaleHideAnimation CaleAnimation => _scaleAnimation;
        public FadeHideAnimation AdeAnimation => _fadeAnimation;

        #endregion


        #region Fields

        [SerializeReference] private MoveHideAnimation _moveAnimation;
        [SerializeReference] private RotateHideAnimation _rotateAnimation;
        [SerializeReference] private ScaleHideAnimation _scaleAnimation;
        [SerializeReference] private FadeHideAnimation _fadeAnimation;

        #endregion


        #region Public Methods

        public async UniTask AnimateAsync(Transform transform, CanvasGroup canvasGroup) => await AnimateAsync((RectTransform)transform, canvasGroup);
        public async UniTask AnimateAsync(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            // ReSharper disable once HeapView.ObjectAllocation
            await UniTask.WhenAll(
                _moveAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                _rotateAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                _scaleAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                _fadeAnimation?.AnimateAsync(canvasGroup) ?? UniTask.CompletedTask
            );
        }

        #endregion
    }
}
#endif