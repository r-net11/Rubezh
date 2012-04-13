using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Converters;
using System;

namespace FiresecService
{
    public class FiresecManager
    {
        public static List<Driver> Drivers { get; set; }
        public static DeviceConfiguration DeviceConfiguration { get; set; }
        public static DeviceConfigurationStates DeviceConfigurationStates { get; set; }
        public static LibraryConfiguration LibraryConfiguration { get; set; }
        public static SystemConfiguration SystemConfiguration { get; set; }
        public static PlansConfiguration PlansConfiguration { get; set; }
        public static SecurityConfiguration SecurityConfiguration { get; set; }
        public static bool IsConnected { get; private set; }
        public static string DriversError { get; private set; }

        public static void ConnectFiresecCOMServer(string login, string password)
        {
            DeviceConfiguration = ConfigurationFileManager.GetDeviceConfiguration();
            SecurityConfiguration = ConfigurationFileManager.GetSecurityConfiguration();
            LibraryConfiguration = ConfigurationFileManager.GetLibraryConfiguration();
            SystemConfiguration = ConfigurationFileManager.GetSystemConfiguration();
            PlansConfiguration = ConfigurationFileManager.GetPlansConfiguration();

            if (FiresecSerializedClient.Connect(login, password).Result)
            {
                ConvertMetadataFromFiresec();
                SetValidChars();
                Update();
                DeviceStatesConverter.Convert();

                var watcher = new Watcher();
                watcher.Start();

                IsConnected = true;
                return;
            }
            IsConnected = false;
        }

        static void ConvertMetadataFromFiresec()
        {
            DriverConverter.Metadata = FiresecSerializedClient.GetMetaData().Result;
            Drivers = new List<Driver>();
            foreach (var innerDriver in DriverConverter.Metadata.drv)
            {
                var driver = DriverConverter.Convert(innerDriver);
                if (driver == null)
                {
                    DriversError = "Не удается найти данные для драйвера " + innerDriver.name;
                }
                else
                {
                    if (driver.IsIgnore == false)
                        Drivers.Add(driver);
                }
            }
        }

        public static void SetValidChars()
        {
            DriverConverter.Metadata = FiresecSerializedClient.GetMetaData().Result;
            var validCharsBuilder = new StringBuilder(DriverConverter.Metadata.drv.Last().validChars);
            validCharsBuilder.Append('№');
            DeviceConfiguration.ValidChars = validCharsBuilder.ToString();
        }

        public static void Update()
        {
            var hasInvalidDriver = false;
            DeviceConfiguration.Update();
            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
                if (device.Driver == null)
                {
                    hasInvalidDriver = true;
                    device.Parent.Children.Remove(device);
                }
            }
            if (hasInvalidDriver)
                DeviceConfiguration.Update();
        }
    }
}