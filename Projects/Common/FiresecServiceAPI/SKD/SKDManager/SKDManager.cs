using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace FiresecAPI
{
	public partial class SKDManager
	{
		public static SKDConfiguration SKDConfiguration { get; set; }
		public static SKDLibraryConfiguration SKDLibraryConfiguration { get; set; }
		public static List<SKDDriver> Drivers { get; set; }

		static SKDManager()
		{
			SKDConfiguration = new SKDConfiguration();
			CreateDrivers();
		}

		public static List<SKDDevice> Devices
		{
			get { return SKDConfiguration.Devices; }
		}

		public static void SetEmptyConfiguration()
		{
			SKDConfiguration = new SKDConfiguration();
			UpdateConfiguration();
		}

		public static void UpdateConfiguration()
		{
			if (SKDConfiguration.RootDevice == null)
			{
				var driver = Drivers.FirstOrDefault(x => x.DriverType == SKDDriverType.System);
				SKDConfiguration.RootDevice = new SKDDevice()
				{
					DriverUID = driver.UID
				};
			}

			SKDConfiguration.Update();
			foreach (var device in SKDConfiguration.Devices)
			{
				device.Driver = Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					//MessageBoxService.Show("Ошибка при сопоставлении драйвера устройств ГК");
				}
			}
		}
	}
}