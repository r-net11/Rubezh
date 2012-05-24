using System.Linq;
using XFiresecAPI;

namespace FiresecClient
{
	public static class XManager
	{
		public static XDeviceConfiguration DeviceConfiguration;
		public static XDriversConfiguration DriversConfiguration;

		static XManager()
		{
			DeviceConfiguration = new XDeviceConfiguration();
			DriversConfiguration = new XDriversConfiguration();
		}

		public static void SetEmptyConfiguration()
		{
			DeviceConfiguration = new XDeviceConfiguration();

			var systemDriver = DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
			if (systemDriver != null)
				DeviceConfiguration.RootDevice = new XDevice()
				{
					DriverUID = systemDriver.UID,
					Driver = systemDriver
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
	}
}