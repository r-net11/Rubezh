using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class XManager
	{
		public static XDeviceConfiguration DeviceConfiguration { get; set; }
		public static XDriversConfiguration DriversConfiguration { get; set; }
		public static XDeviceConfigurationStates DeviceStates { get; set; }

		static XManager()
		{
			DeviceConfiguration = new XDeviceConfiguration();
			DriversConfiguration = new XDriversConfiguration();
			DeviceStates = new XDeviceConfigurationStates();
		}

		public static void SetEmptyConfiguration()
		{
			DeviceConfiguration = new XDeviceConfiguration();

			var systemDriver = DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
			if (systemDriver != null)
			{
				DeviceConfiguration.RootDevice = new XDevice()
				{
					DriverUID = systemDriver.UID,
					Driver = systemDriver
				};
			}
			else
			{
				Logger.Error("XManager.SetEmptyConfiguration systemDriver = null");
			}

			UpdateConfiguration();
		}

		public static void UpdateConfiguration()
		{
			if (DeviceConfiguration == null)
			{
				DeviceConfiguration = new XDeviceConfiguration();
			}
			if (DeviceConfiguration.Directions == null)
				DeviceConfiguration.Directions = new List<XDirection>();

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

		public static ushort GetKauLine(XDevice device)
		{
			if (device.Driver.DriverType != XDriverType.KAU)
			{
				throw new Exception("В XManager.GetKauLine передан неверный тип устройства");
			}

			ushort lineNo = 0;
			var modeProperty = device.Properties.FirstOrDefault(x => x.Name == "Mode");
			if (modeProperty != null)
			{
				return modeProperty.Value;
			}
			return lineNo;
		}

		public static string GetIpAddress(XDevice device)
		{
			XDevice gkDevice = null;
			switch (device.Driver.DriverType)
			{
				case XDriverType.GK:
					gkDevice = device;
					break;

				case XDriverType.KAU:
					gkDevice = device.Parent;
					break;

				default:
					throw new Exception("Получить IP адрес можно только у ГК или в КАУ");
			}
			var ipAddress = gkDevice.GetGKIpAddress();
			if (ipAddress == null)
			{
				throw new Exception("Не задан IP адрес");
			}
			return ipAddress;
		}
	}
}