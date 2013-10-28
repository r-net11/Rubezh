using System;
using System.Linq;
using Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.Collections.Generic;

namespace FiresecClient
{
	public partial class XManager
	{
		public static XDeviceConfiguration DeviceConfiguration { get; set; }
		public static XDriversConfiguration DriversConfiguration { get; set; }
        public static XDeviceLibraryConfiguration DeviceLibraryConfiguration { get; set; }

		static XManager()
		{
			DeviceConfiguration = new XDeviceConfiguration();
			DriversConfiguration = new XDriversConfiguration();
		}

		public static List<XDevice> Devices
		{
			get { return DeviceConfiguration.Devices; }
		}
		public static List<XZone> Zones
		{
			get { return DeviceConfiguration.Zones; }
		}
		public static List<XDirection> Directions
		{
			get { return DeviceConfiguration.Directions; }
		}
		public static List<XDriver> Drivers
		{
			get { return DriversConfiguration.XDrivers; }
		}

		public static void SetEmptyConfiguration()
		{
			DeviceConfiguration = new XDeviceConfiguration();
			UpdateConfiguration();
		}

		public static void UpdateConfiguration()
		{
			if (DeviceConfiguration == null)
			{
				DeviceConfiguration = new XDeviceConfiguration();
			}
			if (DeviceConfiguration.RootDevice == null)
			{
				var systemDriver = Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
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
			}
			DeviceConfiguration.ValidateVersion();

			DeviceConfiguration.Update();
			foreach (var device in DeviceConfiguration.Devices)
			{
				if(device.DriverUID == Guid.Empty)
				{
					
				}
				device.Driver = Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					MessageBoxService.Show("Ошибка при сопоставлении драйвера устройств ГК");
				}
			}
			DeviceConfiguration.Reorder();

			InitializeMissingDefaultProperties();
            Invalidate();
		}

		public static void InitializeMissingDefaultProperties()
		{
			foreach (var device in Devices)
			{
				device.InitializeDefaultProperties();
			}
		}

		public static ushort GetKauLine(XDevice device)
		{
			if (!device.Driver.IsKauOrRSR2Kau)
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
				case XDriverType.RSR2_KAU:
                    gkDevice = device.Parent;
                    break;

                default:
                    {
                        Logger.Error("XManager.GetIpAddress Получить IP адрес можно только у ГК или в КАУ");
                        throw new Exception("Получить IP адрес можно только у ГК или в КАУ");
                    }
            }
			var ipAddress = gkDevice.GetGKIpAddress();
			return ipAddress;
		}

		public static bool IsManyGK()
		{
			return DeviceConfiguration.Devices.Where(x => x.Driver.DriverType == XDriverType.GK).Count() > 1;
		}
	}
}