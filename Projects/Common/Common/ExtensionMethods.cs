using System.Collections.Generic;

namespace Common
{
    public static class ExtensionMethods
    {
        public static bool IsNotNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection != null && collection.Count > 0;
        }
    }
}