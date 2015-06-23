using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		public static GKDeviceConfiguration DeviceConfiguration { get; set; }
		public static GKDriversConfiguration DriversConfiguration { get; set; }

		static GKManager()
		{
			DeviceConfiguration = new GKDeviceConfiguration();
			DriversConfiguration = new GKDriversConfiguration();
		}

		public static List<GKDevice> Devices
		{
			get { return DeviceConfiguration.Devices; }
		}
		public static List<GKDriver> Drivers
		{
			get { return DriversConfiguration.Drivers; }
		}

		public static void SetEmptyConfiguration()
		{
			DeviceConfiguration = new GKDeviceConfiguration();
			var driver = Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
			DeviceConfiguration.RootDevice = new GKDevice()
			{
				DriverUID = driver.UID
			};
			UpdateConfiguration();
		}

		public static void UpdateConfiguration()
		{
			DeviceConfiguration.UpdateConfiguration();
		}

		public static string GetIpAddress(GKDevice device)
		{
			GKDevice gkControllerDevice = null;
			switch (device.DriverType)
			{
				case GKDriverType.GK:
					gkControllerDevice = device;
					break;

				default:
					{
						Logger.Error("GKManager.GetIpAddress Получить IP адрес можно только у ГК или в КАУ");
						throw new Exception("Получить IP адрес можно только у ГК или в КАУ");
					}
			}
			var ipAddress = gkControllerDevice.GetGKIpAddress();
			return ipAddress;
		}

		public static bool IsManyGK()
		{
			return DeviceConfiguration.Devices.Where(x => x.DriverType == GKDriverType.GK).Count() > 1;
		}
	}
}