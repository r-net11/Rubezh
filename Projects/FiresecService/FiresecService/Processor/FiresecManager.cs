using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;

namespace FiresecService
{
    public class FiresecManager
    {
        public static List<Driver> Drivers { get; set; }
        public static DeviceConfiguration DeviceConfiguration { get; set; }
        public static DeviceConfigurationStates DeviceConfigurationStates { get; set; }
        public static DeviceLibraryConfiguration DeviceLibraryConfiguration { get; set; }
        public static SystemConfiguration SystemConfiguration { get; set; }
        public static PlansConfiguration PlansConfiguration { get; set; }
        public static SecurityConfiguration SecurityConfiguration { get; set; }
        public static Firesec.CoreConfig.config CoreConfig { get; set; }

        public static bool Connect(string login, string password)
        {
            bool result = FiresecInternalClient.Connect(login, password);
            if (result)
            {
                BuildDeviceTree();
                DeviceConfiguration.Update();
                var watcher = new Watcher();
                watcher.Start();
            }
            return result;
        }

        static void BuildDeviceTree()
        {
            CoreConfig = FiresecInternalClient.GetCoreConfig();
            DeviceConfiguration = new DeviceConfiguration();
            var metadata = FiresecInternalClient.GetMetaData();
            FillDrivrs(metadata);
            Convert();
        }

        public static void FillDrivrs(Firesec.Metadata.config metadata)
        {
            DriverConverter.Metadata = metadata;
            Drivers = new List<Driver>();
            foreach (var firesecDriver in metadata.drv)
            {
                var driver = DriverConverter.Convert(firesecDriver);
                if (driver.IsIgnore == false)
                {
                    Drivers.Add(driver);
                }
            }
        }

        public static void Update()
        {
            DeviceConfiguration.Update();

            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.Id == device.DriverId);
            }
        }

        public static void SetNewConfig()
        {
            Update();
            ConvertBack();
            FiresecInternalClient.SetNewConfig(CoreConfig);
        }

        static void Convert()
        {
            FiresecManager.DeviceConfigurationStates = new DeviceConfigurationStates();
            ZoneConverter.Convert(CoreConfig);
            DirectionConverter.Convert(CoreConfig);
            GuardUserConverter.Convert(CoreConfig);
            SecurityConverter.Convert(CoreConfig);
            DeviceConverter.Convert(CoreConfig);
        }

        static void ConvertBack()
        {
            ZoneConverter.ConvertBack(DeviceConfiguration);
            DeviceConverter.ConvertBack(DeviceConfiguration);
            DirectionConverter.ConvertBack(DeviceConfiguration);
            GuardUserConverter.ConvertBack(DeviceConfiguration);
            SecurityConverter.ConvertBack(SecurityConfiguration);
        }

        public static void LoadFromFile(string fileName)
        {
            CoreConfig = FiresecInternalClient.LoadConfigFromFile(fileName);
            FiresecManager.DeviceConfigurationStates = new DeviceConfigurationStates();
            ZoneConverter.Convert(CoreConfig);
            DirectionConverter.Convert(CoreConfig);
            GuardUserConverter.Convert(CoreConfig);
            DeviceConverter.Convert(CoreConfig);
        }

        public static void SaveToFile(string fileName)
        {
            ZoneConverter.ConvertBack(DeviceConfiguration);
            DeviceConverter.ConvertBack(DeviceConfiguration);
            DirectionConverter.ConvertBack(DeviceConfiguration);
            GuardUserConverter.ConvertBack(DeviceConfiguration);
            FiresecInternalClient.SaveConfigToFile(CoreConfig, fileName);
        }
    }
}
