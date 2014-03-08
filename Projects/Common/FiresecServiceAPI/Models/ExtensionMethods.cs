using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace FiresecAPI
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
			if (fieldInfo != null)
			{
				DescriptionAttribute[] descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (descriptionAttributes.Length > 0)
					return descriptionAttributes[0].Description;
				return value.ToString();
			}
			return null;
		}

		public static Size Subtract(this Size s1, Size s2)
		{
			return new Size(s1.Width - s2.Width, s1.Height - s2.Height);
		}
	}
}