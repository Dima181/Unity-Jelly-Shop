using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UniRx;

namespace Mine.CodeBase.Framework.Util.Reflection
{
    public static class DerivedTypeCache
    {
        #region Inner Structs

        public struct DeriveTypeContext
        {
            public Assembly[] Assembly;
            public Type[] DerivedTypes;
        }

        #endregion


        #region Fields

        private static Dictionary<(Type, Assembly[]), DeriveTypeContext> _contexts = new();

        #endregion


        #region Public Methods

        public static ReadOnlyCollection<Type> GetDerivedTypes(Type type, params Assembly[] assemblies)
        {
            if(_contexts.TryGetValue((type, assemblies), out DeriveTypeContext context) == true)
            {
                return new ReadOnlyCollection<Type>(context.DerivedTypes);
            }

            InternalInit(type, assemblies);

            return new ReadOnlyCollection<Type>(_contexts[(type, assemblies)].DerivedTypes);
        }

        #endregion


        #region Private Methods

        private static void InternalInit(Type type, Assembly[] assemblies)
        {
            DeriveTypeContext context = new();
            if (assemblies != null && assemblies.Length > 0)
            {
                List<Type> temp = new();
                foreach (Assembly assembly in assemblies)
                {
                    temp.AddRange(assembly.GetTypes()
                            .Where(t =>
                                type.IsAssignableFrom(t)
                                && t != type
                                && t.IsInterface == false
                                && t.IsAbstract == false));
                }

                context.Assembly = assemblies;
                context.DerivedTypes = temp.ToArray();
            }
            else
            {
                context.DerivedTypes = type.Assembly.GetTypes()
                    .Where(t =>
                        type.IsAssignableFrom(t)
                        && t != type
                        && t.IsInterface == false
                        && t.IsAbstract == false)
                    .ToArray();
            }

            _contexts.Add((type, assemblies), context);
        }

        #endregion
    }
}
