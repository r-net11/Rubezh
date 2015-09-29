using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ResursAPI
{
	public static class ExtensionMethods
	{
		public static bool IsNotNullOrEmpty<T>(this ICollection<T> collection)
		{
			return collection != null && collection.Count > 0;
		}

		public static bool IsExactly<T>(this object obj)
		{
			return obj != null && obj.GetType() == typeof(T);
		}

		public static string ToDescription(this Enum value)
		{
			FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
			if (fieldInfo != null)
			{
				DescriptionAttribute[] descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (descriptionAttributes.Length > 0)
					return descriptionAttributes[0].Description;
				return value.ToString();
			}
			return null;
		}

		public static Guid? EmptyToNull(this Guid value)
		{
			if (value == Guid.Empty)
				return null;
			return value;
		}
	}
}
