using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace FiresecAPI.Models
{
    public static class ExtensionMethods
    {
        public static bool IsNotNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection != null && collection.Count > 0;
        }

        public static string ToDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] descriptionAttribute = (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descriptionAttribute.Length > 0)
                return descriptionAttribute[0].Description;
            return value.ToString();
        }
    }
}