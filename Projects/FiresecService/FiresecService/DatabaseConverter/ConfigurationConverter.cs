using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;

namespace FiresecService
{
    public static class ConfigurationConverter
    {
        public static Firesec.CoreConfiguration.config FiresecConfiguration { get; set; }
        public static DeviceConfiguration DeviceConfiguration { get; set; }
        public static int Gid { get; set; }

        public static void Convert()
        {
            FiresecConfiguration = FiresecInternalClient.GetCoreConfig().Result;
            ConvertConfiguration();
            FiresecManager.DeviceConfiguration = DeviceConfiguration;
            FiresecManager.SetValidChars();
            FiresecManager.Update();

            ConfigurationFileManager.SetDeviceConfiguration(DeviceConfiguration);

            var plans = FiresecInternalClient.GetPlans().Result;
            var plansConfiguration = PlansConverter.Convert(plans);
            ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
        }

        static void ConvertConfiguration()
        {
            DeviceConfiguration = new DeviceConfiguration();
            ZoneConverter.Convert();
            DirectionConverter.Convert();
            GuardUserConverter.Convert();
            DeviceConverter.Convert();
        }

        public static void ConvertBack(DeviceConfiguration deviceConfiguration, bool includeSecurity)
        {
            deviceConfiguration.Update();

            foreach (var device in deviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
            }

            if (includeSecurity)
            {
                FiresecConfiguration = FiresecInternalClient.GetCoreConfig().Result;
                FiresecConfiguration.part = null;
            }
            else
            {
                FiresecConfiguration = new Firesec.CoreConfiguration.config();
            }

            Gid = 0;
            ZoneConverter.ConvertBack(deviceConfiguration);
            DeviceConverter.ConvertBack(deviceConfiguration);
            DirectionConverter.ConvertBack(deviceConfiguration);
            GuardUserConverter.ConvertBack(deviceConfiguration);
        }
    }
}