using XFiresecAPI;
using System.Linq;

namespace FiresecClient
{
    public static class XManager
    {
        static XManager()
        {
            DeviceConfiguration = new XDeviceConfiguration();
            DriversConfiguration = new XDriversConfiguration();
        }

        public static void SetEmptyConfiguration()
        {
            DeviceConfiguration = new XDeviceConfiguration();
            DeviceConfiguration.RootDevice = new XDevice()
            {
                DriverUID = DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System).UID,
                Driver = DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System)
            };
        }

        public static void UpdateConfiguration()
        {
            DeviceConfiguration.Update();

            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
                if (device.Driver == null)
                {
                    System.Windows.MessageBox.Show("Ошибка при сопоставлении драйвера устройств");
                }
            }
        }

        public static void Invalidate()
        {
        }

        public static XDeviceConfiguration DeviceConfiguration;
        public static XDriversConfiguration DriversConfiguration;
    }
}