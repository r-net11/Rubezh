using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Common
{
	/// <summary>
	/// Helper class for enum types
	/// </summary>
	public static class EnumHelper
	{
		/// <summary>
		/// Allows the use of the 'DescriptionAttribute' to convert from and to the Enum values.
		/// </summary>
		/// <returns>Description string of the input Enum value</returns>
		public static string GetEnumDescription(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] attributes =
			  (DescriptionAttribute[])fi.GetCustomAttributes
			  (typeof(DescriptionAttribute), false);
			return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
		}

		/// <summary>
		/// Allow to use the Name of enum value
		/// </summary>
		public static string GetEnumName(System.Type value, string description)
		{
			FieldInfo[] fis = value.GetFields();
			foreach (FieldInfo fi in fis)
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])fi.GetCustomAttributes
				  (typeof(DescriptionAttribute), false);
				if (attributes.Length > 0)
				{
					if (attributes[0].Description == description)
					{
						return fi.Name;
					}
				}
			}
			return description;
		}
	}
}
