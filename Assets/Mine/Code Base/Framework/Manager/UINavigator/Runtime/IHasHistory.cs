﻿#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Runtime
{
    public interface IHasHistory
    {
        UniTask PrevAsync(int count = 1);
    }
}
#endif