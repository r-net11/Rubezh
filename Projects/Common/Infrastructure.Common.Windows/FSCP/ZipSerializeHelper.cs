using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;
using RubezhAPI;
using Infrastructure.Common.Windows.Windows;

namespace Infrastructure.Common.Windows
{
	public class ZipSerializeHelper
	{
		public static MemoryStream Serialize<T>(T configuration)
			where T : VersionedConfiguration
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 };
			var memoryStream = new MemoryStream();

			var xmlSerializer = new XmlSerializer(configuration.GetType());
			xmlSerializer.Serialize(memoryStream, configuration);
			return memoryStream;
		}

		public static T DeSerialize<T>(Stream stream)
			 where T : VersionedConfiguration, new()
		{
			T configuration = null;
			var xmlSerializer = new XmlSerializer(typeof(T));
			configuration = (T)xmlSerializer.Deserialize(stream);
			configuration.ValidateVersion();
			configuration.AfterLoad();
			return configuration;
		}

		public static bool Serialize<T>(T configuration, string fileName)
			where T : VersionedConfiguration
		{
			try
			{
				var xmlSerializer = new XmlSerializer(configuration.GetType());
				using (var fileStream = new FileStream(fileName, FileMode.Create))
				{
					xmlSerializer.Serialize(fileStream, configuration);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ZipSerializeHelper.Serialize");
				return false;
			}
			return true;
		}

		public static T DeSerialize<T>(string fileName)
			 where T : VersionedConfiguration, new()
		{
			try
			{
				using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					var xmlSerializer = new XmlSerializer(typeof(T));
					T configuration = (T)xmlSerializer.Deserialize(fileStream);
					fileStream.Close();
					configuration.ValidateVersion();
					configuration.AfterLoad();
					return configuration;
				}
			}
			catch (Exception e)
			{
				Logger.Error("ZipSerializeHelper.DeSerialize " + fileName + " " + e.Message);
				return new T();
			}
		}
	}
}