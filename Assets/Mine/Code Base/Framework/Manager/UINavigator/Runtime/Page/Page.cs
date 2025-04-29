#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using UnityEngine;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Page
{
    public abstract class Page : UIContext
    {
        public bool IsRecycle => _isRecycle;

        [SerializeField] private bool _isRecycle = true;
    }
}
#endif