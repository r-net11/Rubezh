using System.IO;
using FiresecAPI;
using System.Runtime.Serialization;
using System.Collections.Generic;

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

		public static T DeSerialize<T>(MemoryStream memStream)
			 where T : VersionedConfiguration, new()
		{
			T configuration = null;
			var dataContractSerializer = new DataContractSerializer(typeof(T));
			configuration = (T)dataContractSerializer.ReadObject(memStream);
			configuration.ValidateVersion();
			configuration.AfterLoad();
			return configuration;
		}
	}
}