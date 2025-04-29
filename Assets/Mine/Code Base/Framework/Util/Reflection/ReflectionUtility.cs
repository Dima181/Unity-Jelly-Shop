using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Mine.CodeBase.Framework.Util.Reflection
{
    public static class ReflectionUtility
    {
        #region Public Methods

        public static ReadOnlyCollection<Type> GetDerivedTypes<T>(params Assembly[] assemblies)
        {
            return DerivedTypeCache.GetDerivedTypes(typeof(T), assemblies);
        }

        #endregion
    }
}
