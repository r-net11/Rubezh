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

		public static List<SKDZone> Zones
		{
			get { return SKDConfiguration.Zones; }
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

			if (SKDConfiguration.RootZone == null)
			{
				SKDConfiguration.RootZone = new SKDZone()
				{
					IsRootZone = true,
					Name = "Неконтролируемая территория"
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

			Invalidate();
		}

		public static List<byte> CreateHash()
		{
			return new List<byte>();
		}

		public static bool CompareHashes(List<byte> hash1, List<byte> hash2)
		{
			return true;
		}

		public static string GetPresentationZone(SKDDevice device)
		{
			return "Зона";
		}

		public static void CreateStates()
		{
			foreach (var device in Devices)
			{
				device.State = new SKDDeviceState();
			}
		}

		public static XStateClass GetMinStateClass()
		{
			var minStateClass = XStateClass.No;
			foreach (var device in Devices)
			{
				if (device.IsRealDevice)
				{
					var stateClass = device.State.StateClass;
					if (stateClass < minStateClass)
						minStateClass = device.State.StateClass;
				}
			}
			foreach (var zone in Zones)
			{
				if (zone.State.StateClass < minStateClass)
					minStateClass = zone.State.StateClass;
			}
			return minStateClass;
		}

		public static void AddDeviceToZone(SKDDevice device, SKDZone zone)
		{
			device.ZoneUID = zone.UID;
			device.Zone = zone;
			zone.Devices.Add(device);
			zone.OnChanged();
			device.OnChanged();
		}

		public static void RemoveDeviceFromZone(SKDDevice device, SKDZone zone)
		{
			if (zone != null)
			{
				device.Zone = null;
				device.ZoneUID = Guid.Empty;
				zone.Devices.Remove(device);
				zone.OnChanged();
				device.OnChanged();
			}
		}

		static void Invalidate()
		{
			ClearAllReferences();
			InitializeDevicesInZone();
		}

		static void ClearAllReferences()
		{
			foreach (var device in Devices)
			{
				device.Zone = null;
			}
			foreach (var zone in Zones)
			{
				zone.Devices = new List<SKDDevice>();
			}
		}

		static void InitializeDevicesInZone()
		{
			foreach (var device in Devices)
			{
				var zoneUID = Guid.Empty;
				if (device.Driver.HasZone)
				{
					var zone = Zones.FirstOrDefault(x => x.UID == device.ZoneUID);
					if (zone != null)
					{
						zoneUID = device.ZoneUID;
						device.Zone = zone;
						zone.Devices.Add(device);
					}
				}
				device.ZoneUID = zoneUID;
			}
		}
	}
}