using System;
using System.ComponentModel;
using System.Reflection;

namespace FiresecAPI.Models
{
    public static class EnumHelper
    {
        public static string ToString(Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] descriptionAttribute = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descriptionAttribute.Length > 0)
            {
                return descriptionAttribute[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
