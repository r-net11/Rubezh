using System;
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
					System.Windows.MessageBox.Show("Ошибка при сопоставлении драйвера устройств ГК");
				}
			}

			InitializeMissingDefaultProperties();
		}

		public static void InitializeMissingDefaultProperties()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				foreach (var driverProperty in device.Driver.Properties)
				{
					if (device.Properties.Any(x => x.Name == driverProperty.Name) == false)
					{
						var property = new XProperty()
						{
							Name = driverProperty.Name,
							Value = driverProperty.Default
						};
						device.Properties.Add(property);
					}
				}
			}
		}

		public static void Invalidate()
		{
		}

		public static short GetKauLine(XDevice device)
		{
			if (device.Driver.DriverType != XDriverType.KAU)
			{
				throw new Exception("В XManager.GetKauLine передан неверный тип устройства");
			}

			short lineNo = 0;
			var modeProperty = device.Properties.FirstOrDefault(x => x.Name == "Mode");
			if (modeProperty != null)
			{
				return modeProperty.Value;
				//var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == "Mode");
				//lineNo = driverProperty.Parameters.FirstOrDefault(x => x.Name == modeProperty.Name).Value;
			}
			return lineNo;
		}
	}
}