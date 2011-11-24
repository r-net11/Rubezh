using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Converters;

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

        public FiresecManager()
        {
        }

        public static bool ConnectFiresecCOMServer(string login, string password)
        {
            DeviceConfiguration = ConfigurationFileManager.GetDeviceConfiguration();
            SecurityConfiguration = ConfigurationFileManager.GetSecurityConfiguration();
            LibraryConfiguration = ConfigurationFileManager.GetLibraryConfiguration();
            SystemConfiguration = ConfigurationFileManager.GetSystemConfiguration();
            PlansConfiguration = ConfigurationFileManager.GetPlansConfiguration();

            if (FiresecInternalClient.Connect(login, password))
            {
                ConvertMetadataFromFiresec();
                SetValidChars();
                Update();
                DeviceStatesConverter.Convert();

                var watcher = new Watcher();
                watcher.Start();

                return true;
            }
            return false;
        }

        static void ConvertMetadataFromFiresec()
        {
            DriverConverter.Metadata = FiresecInternalClient.GetMetaData();
            Drivers = new List<Driver>(
                DriverConverter.Metadata.drv.
                Select(firesecDriver => DriverConverter.Convert(firesecDriver)).
                Where(driver => driver.IsIgnore == false)
            );
        }

        public static void SetValidChars()
        {
            DriverConverter.Metadata = FiresecInternalClient.GetMetaData();
            var validCharsBuilder = new StringBuilder(DriverConverter.Metadata.drv.Last().validChars);
            validCharsBuilder.Append('№');
            DeviceConfiguration.ValidChars = validCharsBuilder.ToString();
        }

        public static void Update()
        {
            DeviceConfiguration.Update();
            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
            }
        }
    }
}