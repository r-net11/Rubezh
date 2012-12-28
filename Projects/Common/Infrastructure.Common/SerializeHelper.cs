using System.IO;
using FiresecAPI;
using System.Runtime.Serialization;

namespace Infrastructure.Common
{
	public class SerializeHelper
	{
		public static MemoryStream Serialize<T>(T configuration)
			where T : VersionedConfiguration
		{
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
			return configuration;
		}
	}
}