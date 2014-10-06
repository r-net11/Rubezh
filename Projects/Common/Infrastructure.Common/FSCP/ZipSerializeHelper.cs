using System;
using System.IO;
using System.Runtime.Serialization;
using Common;
using FiresecAPI;
using System.Xml.Serialization;

namespace Infrastructure.Common
{
	public class ZipSerializeHelper
	{
		public static MemoryStream Serialize<T>(T configuration, bool useXml)
			where T : VersionedConfiguration
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 };
			var memoryStream = new MemoryStream();

			if (useXml)
			{
				var xmlSerializer = new XmlSerializer(configuration.GetType());
				xmlSerializer.Serialize(memoryStream, configuration);
			}
			else
			{
				var dataContractSerializer = new DataContractSerializer(configuration.GetType());
				dataContractSerializer.WriteObject(memoryStream, configuration);
			}
			return memoryStream;
		}

		public static T DeSerialize<T>(MemoryStream memoryStream, bool useXml)
			 where T : VersionedConfiguration, new()
		{
			T configuration = null;
			if (useXml)
			{
				var xmlSerializer = new XmlSerializer(typeof(T));
				configuration = (T)xmlSerializer.Deserialize(memoryStream);
			}
			else
			{
				var dataContractSerializer = new DataContractSerializer(typeof(T));
				configuration = (T)dataContractSerializer.ReadObject(memoryStream);
			}
			configuration.ValidateVersion();
			configuration.AfterLoad();
			return configuration;
		}

		public static bool Serialize<T>(T configuration, string fileName, bool useXml)
			where T : VersionedConfiguration
		{
			try
			{
				if (useXml)
				{
					var xmlSerializer = new XmlSerializer(configuration.GetType());
					using (var fileStream = new FileStream(fileName, FileMode.Create))
					{
						xmlSerializer.Serialize(fileStream, configuration);
					}
				}
				else
				{
					var dataContractSerializer = new DataContractSerializer(configuration.GetType());
					using (var fileStream = new FileStream(fileName, FileMode.Create))
					{
						dataContractSerializer.WriteObject(fileStream, configuration);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ZipSerializeHelper.Serialize");
				return false;
			}
			return true;
		}

		public static T DeSerialize<T>(string fileName, bool useXml)
			 where T : VersionedConfiguration, new()
		{
			try
			{
				using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					T configuration = null;
					if (useXml)
					{
						var xmlSerializer = new XmlSerializer(typeof(T));
						configuration = (T)xmlSerializer.Deserialize(fileStream);
					}
					else
					{
						var dataContractSerializer = new DataContractSerializer(typeof(T));
						configuration = (T)dataContractSerializer.ReadObject(fileStream);
					}
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