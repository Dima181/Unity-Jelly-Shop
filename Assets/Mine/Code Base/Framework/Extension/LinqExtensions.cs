using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mine.CodeBase.Framework.Extension
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> collection, Action<TSource> action)
        {
            for (int i = collection.Count() - 1; i >= 0; i--)
            {
                action(collection.ElementAtOrDefault(i));
            }

            return collection;
        }
    }
}
