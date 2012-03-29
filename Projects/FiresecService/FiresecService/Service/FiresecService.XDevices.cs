using System;
using System.IO;
using System.Runtime.Serialization;
using XFiresecAPI;

namespace FiresecService
{
    public partial class FiresecService
    {
        readonly static string XDeviceConfigurationFileName = "SystemConfiguration.xml";
        public static string ConfigurationDirectory(string FileNameOrDirectory)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", FileNameOrDirectory);
        }

        public void SetXDeviceConfiguration(XDeviceConfiguration xDeviceConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(XDeviceConfiguration));
            using (var fileStream = new FileStream(ConfigurationDirectory(XDeviceConfigurationFileName), FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, xDeviceConfiguration);
            }
        }

        public XDeviceConfiguration GetXDeviceConfiguration()
        {
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(XDeviceConfiguration));
                using (var fileStream = new FileStream(ConfigurationDirectory(XDeviceConfigurationFileName), FileMode.Open))
                {
                    return (XDeviceConfiguration)dataContractSerializer.ReadObject(fileStream);
                }
            }
            catch
            {
                var xDeviceConfiguration = new XDeviceConfiguration();
                SetXDeviceConfiguration(xDeviceConfiguration);
                return xDeviceConfiguration;
            }
        }
    }
}