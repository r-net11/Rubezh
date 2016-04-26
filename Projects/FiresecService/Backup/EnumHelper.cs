using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;

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

		/// <summary>
		/// Получает атрибут поля
		/// </summary>
		/// <typeparam name="T">Тип атрибута, который хотим получить</typeparam>
		/// <param name="enumVal">Значение Enum</param>
		/// <returns>Атрибут типа T, который содержится в значении enumVal</returns>
		/// <example>JournalSubsystemType type = myEnumVariable.GetAttributeOfType<EventNameAttribute>().JournalSubsystemType;</example>
		public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
		{
			var type = enumVal.GetType();
			var memInfo = type.GetMember(enumVal.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
			return (attributes.Length > 0) ? (T)attributes[0] : null;
		}
	}
}