using System.Collections;

namespace Common
{
    public static class ExtensionMethods
    {
        public static bool IsNotNullOrEmpty(this ICollection collection)
        {
            return collection != null && collection.Count > 0;
        }
    }
}