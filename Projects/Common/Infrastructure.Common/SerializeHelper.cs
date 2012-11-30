using System.IO;
using FiresecAPI;
using System.Runtime.Serialization;

namespace Infrastructure.Common
{
    public class SerializeHelper
    {
        public static void Serialize<T>(T configuration)
            where T : VersionedConfiguration
        {
            configuration.Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 };
            using (var memoryStream = new MemoryStream())
            {
                var dataContractSerializer = new DataContractSerializer(typeof(T));
                dataContractSerializer.WriteObject(memoryStream, configuration);
            }
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
