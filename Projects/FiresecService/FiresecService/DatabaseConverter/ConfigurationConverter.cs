using System.IO;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FiresecService
{
    public static class ConfigurationConverter
    {
        public static void Convert()
        {
            JournalDataConverter.Convert();
        }

        static void ConvertDevices()
        {
            var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
            using (var fileStream = new FileStream("DeviceConfiguration.xml", FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, FiresecManager.DeviceConfiguration);
            }
        }

        static void ConvertPlans()
        {
            var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
            using (var fileStream = new FileStream("PlansConfiguration.xml", FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, FiresecManager.PlansConfiguration);
            }
        }
    }
}
