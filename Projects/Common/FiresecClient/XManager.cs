using System;
using System.Linq;
using XFiresecAPI;
using System.Net;

namespace FiresecClient
{
	public partial class XManager
	{
		public static XDeviceConfiguration DeviceConfiguration;
		public static XDriversConfiguration DriversConfiguration;
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
				DeviceConfiguration.RootDevice = new XDevice()
				{
					DriverUID = systemDriver.UID,
					Driver = systemDriver
				};
		}

		public static void UpdateConfiguration()
		{
			if (DeviceConfiguration == null)
			{
				DeviceConfiguration = new XDeviceConfiguration();
			}
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
			}
			return lineNo;
		}

		public static void CreateStates()
		{
			DeviceStates = new XDeviceConfigurationStates();
			foreach (var device in DeviceConfiguration.Devices)
			{
				var deviceState = new XDeviceState()
				{
					Device = device,
					UID = device.UID,
				};
				DeviceStates.DeviceStates.Add(deviceState);
			}
			foreach (var zone in DeviceConfiguration.Zones)
			{
				var zoneState = new XZoneState()
				{
					Zone = zone,
					No = zone.No
				};
				DeviceStates.ZoneStates.Add(zoneState);
			}
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
			var ipProperty = gkDevice.Properties.FirstOrDefault(x => x.Name == "IPAddress1");
			if (ipProperty != null)
			{
				return ipProperty.StringValue;
			}
			else
			{
				throw new Exception("Не задан IP адрес");
			}
		}
	}
}