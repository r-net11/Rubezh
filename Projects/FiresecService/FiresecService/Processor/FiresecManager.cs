using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;

namespace FiresecService
{
    public class FiresecManager
    {
        public static CurrentConfiguration Configuration { get; set; }
        public static CurrentStates States { get; set; }
        public static Firesec.CoreConfig.config CoreConfig { get; set; }

        public static bool Connect(string login, string password)
        {
            bool result = FiresecInternalClient.Connect(login, password);
            if (result)
            {
                BuildDeviceTree();
                Configuration.Update();
                Watcher watcher = new Watcher();
                watcher.Start();
            }
            return result;
        }

        static void BuildDeviceTree()
        {
            CoreConfig = FiresecInternalClient.GetCoreConfig();
            Configuration = new CurrentConfiguration();
            var metadata = FiresecInternalClient.GetMetaData();
            FillDrivrs(metadata);
            Convert();
        }

        public static void FillDrivrs(Firesec.Metadata.config metadata)
        {
            DriverConverter.Metadata = metadata;
            Configuration.Drivers = new List<Driver>();
            foreach (var firesecDriver in metadata.drv)
            {
                Driver driver = DriverConverter.Convert(firesecDriver);
                if (driver.IsIgnore == false)
                {
                    Configuration.Drivers.Add(driver);
                }
            }
        }

        public static void SetNewConfig()
        {
            Validator validator = new Validator();
            validator.Validate(Configuration);
            ConvertBack();
            FiresecInternalClient.SetNewConfig(CoreConfig);
        }

        static void Convert()
        {
            FiresecManager.States = new CurrentStates();
            ZoneConverter.Convert(CoreConfig);
            DirectionConverter.Convert(CoreConfig);
            SecurityConverter.Convert(CoreConfig);
            DeviceConverter.Convert(CoreConfig);
        }

        static void ConvertBack()
        {
            ZoneConverter.ConvertBack(Configuration);
            DeviceConverter.ConvertBack(Configuration);
            DirectionConverter.ConvertBack(Configuration);
            SecurityConverter.ConvertBack(Configuration);
        }

        public static void LoadFromFile(string fileName)
        {
            CoreConfig = FiresecInternalClient.LoadConfigFromFile(fileName);
            FiresecManager.States = new CurrentStates();
            ZoneConverter.Convert(CoreConfig);
            DirectionConverter.Convert(CoreConfig);
            DeviceConverter.Convert(CoreConfig);
        }

        public static void SaveToFile(string fileName)
        {
            ZoneConverter.ConvertBack(Configuration);
            DeviceConverter.ConvertBack(Configuration);
            DirectionConverter.ConvertBack(Configuration);
            FiresecInternalClient.SaveConfigToFile(CoreConfig, fileName);
        }
    }
}
