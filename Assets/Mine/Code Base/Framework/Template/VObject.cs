using VContainer;
using VContainer.Unity;

namespace Mine.CodeBase.Framework.Template
{
    public class VObject<T>
        where T : LifetimeScope
    {
        #region Preperties

        [Inject] protected LifetimeScope context { private get; set; }

        protected T Context => context as T;

        #endregion
    }
}
