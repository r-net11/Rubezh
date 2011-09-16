using FiresecService.Converters;
using FiresecAPI.Models;
using System.Linq;

namespace FiresecService
{
    public static class ConfigurationConverter
    {
        public static Firesec.CoreConfiguration.config FiresecConfiguration { get; set; }

        public static void Convert()
        {
            FiresecConfiguration = FiresecInternalClient.GetCoreConfig();
            FiresecManager.DeviceConfiguration = new DeviceConfiguration();
            ConvertConfiguration();

            ConfigurationFileManager.SetDeviceConfiguration(FiresecManager.DeviceConfiguration);
            ConfigurationFileManager.SetSecurityConfiguration(FiresecManager.SecurityConfiguration);

            var plans = FiresecInternalClient.GetPlans();
            var plansConfiguration = PlansConverter.Convert(plans);

            ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
        }

        static void ConvertConfiguration()
        {
            FiresecManager.DeviceConfiguration = new DeviceConfiguration();
            ZoneConverter.Convert();
            DirectionConverter.Convert();
            GuardUserConverter.Convert();
            SecurityConverter.Convert();
            DeviceConverter.Convert();
        }

        public static void ConvertBack(DeviceConfiguration deviceConfiguration)
        {
            deviceConfiguration.Update();

            foreach (var device in deviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
            }
            FiresecConfiguration = FiresecInternalClient.GetCoreConfig();
            FiresecConfiguration.part = null;
            FiresecConfiguration.secGUI = null;
            FiresecConfiguration.secObjType = null;
            FiresecConfiguration.user = null;
            FiresecConfiguration.userGroup = null;
            FiresecConfiguration.zone = null;

            ZoneConverter.ConvertBack(deviceConfiguration);
            DeviceConverter.ConvertBack(deviceConfiguration);
            DirectionConverter.ConvertBack(deviceConfiguration);
            GuardUserConverter.ConvertBack(deviceConfiguration);
        }
    }
}
