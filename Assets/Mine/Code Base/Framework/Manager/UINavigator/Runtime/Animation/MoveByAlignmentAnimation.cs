using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Animation
{
    public enum EAlignment
    {
        None,
        Left,
        Top,
        Right,
        Bottom
    }

    public class MoveByAlignmentShowAnimation : MoveShowAnimation
    {
        #region Fields

        [SerializeField] private EAlignment _from;
        [SerializeField] private float _startDelay;
        [SerializeField] private float _duration = 0.25f;
        [SerializeField] private Ease _ease = Ease.Linear;

        #endregion


        #region Override Methods

        public override async UniTask AnimateAsync(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = PositionFromAlignment(rectTransform, _from);

            await rectTransform
                .DOAnchorPos(PositionFromAlignment(rectTransform, EAlignment.None), _duration)
                .SetDelay(_startDelay)
                .SetEase(_ease)
                .SetUpdate(true)
                .ToUniTask();
        }

        #endregion


        #region Private Methods

        private Vector2 PositionFromAlignment(RectTransform rectTransform, EAlignment alignment)
        {
            var rect = rectTransform.rect;
            return alignment switch
            {
                EAlignment.Left => Vector2.left * rect.width,
                EAlignment.Top => Vector2.up * rect.height,
                EAlignment.Right => Vector2.right * rect.width,
                EAlignment.Bottom => Vector2.down * rect.height,
                _ => Vector2.zero
            };
        }

        #endregion
    }

    public class MoveByAlignmentHideAnimation : MoveHideAnimation
    {
        #region Fields

        [SerializeField] private EAlignment _to;
        [SerializeField] private float _startDelay;
        [SerializeField] private float _duration = 0.25f;
        [SerializeField] private Ease _ease = Ease.Linear;

        #endregion


        #region Override Methods

        public override async UniTask AnimateAsync(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = PositionFromAlignment(rectTransform, EAlignment.None);

            await rectTransform
                .DOAnchorPos(PositionFromAlignment(rectTransform, _to), _duration)
                .SetDelay(_startDelay)
                .SetEase(_ease)
                .SetUpdate(true)
                .ToUniTask();
        }

        #endregion


        #region Private Methods

        private Vector2 PositionFromAlignment(RectTransform rectTransform, EAlignment alignment)
        {
            var rect = rectTransform.rect;
            return alignment switch
            {
                EAlignment.Left => Vector2.left * rect.width,
                EAlignment.Top => Vector2.up * rect.height,
                EAlignment.Right => Vector2.right * rect.width,
                EAlignment.Bottom => Vector2.down * rect.height,
                _ => Vector2.zero
            };
        }

        #endregion
    }
}
