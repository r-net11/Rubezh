using System;
using System.Collections.Generic;

namespace Common
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source != null)
                foreach (T item in source)
                    action(item);
        }
    }
}
