using System;
using System.IO;
using System.Runtime.Serialization;
using Common;
using FiresecAPI;

namespace Infrastructure.Common
{
	public class ZipSerializeHelper
	{
		public static MemoryStream Serialize<T>(T configuration)
			where T : VersionedConfiguration
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 };
			var memoryStream = new MemoryStream();

			var dataContractSerializer = new DataContractSerializer(configuration.GetType());
			dataContractSerializer.WriteObject(memoryStream, configuration);
			return memoryStream;
		}

		public static T DeSerialize<T>(MemoryStream memoryStream)
			 where T : VersionedConfiguration, new()
		{
			T configuration = null;
			var dataContractSerializer = new DataContractSerializer(typeof(T));
			configuration = (T)dataContractSerializer.ReadObject(memoryStream);
			configuration.ValidateVersion();
			configuration.AfterLoad();
			return configuration;
		}

		public static bool Serialize<T>(T configuration, string fileName)
			where T : VersionedConfiguration
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(configuration.GetType());
				using (var fileStream = new FileStream(fileName, FileMode.Create))
				{
					dataContractSerializer.WriteObject(fileStream, configuration);
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
					T configuration = null;
					var dataContractSerializer = new DataContractSerializer(typeof(T));
					configuration = (T)dataContractSerializer.ReadObject(fileStream);
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