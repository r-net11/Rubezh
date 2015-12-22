﻿using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Common
{
	public static class Utils
	{
		public static T Clone<T>(T obj)
			where T : class
		{
			var serializer = new XmlSerializer(typeof(T));
			using (var ms = new MemoryStream())
			{
				serializer.Serialize(ms, obj);
				ms.Position = 0;
				return (T)serializer.Deserialize(ms);
			}
		}

		public static T Cast<T>(object obj)
		{
			if (obj is T)
				return (T)obj;
			var converter = TypeDescriptor.GetConverter(typeof(T));
			if (converter != null)
				try
				{
					return (T)(obj is string ? converter.ConvertFromString((string)obj) : converter.ConvertFrom(obj));
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "Исключение при вызове Cast<{0}>(obj={1})", typeof(T), obj);
				}
			return default(T);
		}
	}
}