using System;
using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;
using FiresecAPI.SKD.PassCardLibrary;

namespace FiresecAPI
{
	public partial class SKDManager
	{
		public static SKDConfiguration SKDConfiguration { get; set; }
		public static SKDLibraryConfiguration SKDLibraryConfiguration { get; set; }
		public static SKDPassCardLibraryConfiguration SKDPassCardLibraryConfiguration { get; set; }
		public static List<SKDDriver> Drivers { get; set; }

		static SKDManager()
		{
			SKDConfiguration = new SKDConfiguration();
			SKDPassCardLibraryConfiguration = new SKDPassCardLibraryConfiguration();
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
			SKDConfiguration.ValidateVersion();
			UpdateConfiguration();

			SKDPassCardLibraryConfiguration.Templates = new List<PassCardTemplate>();
		}

		public static void UpdateConfiguration()
		{
			if (SKDConfiguration.RootDevice == null)
			{
				var driver = Drivers.FirstOrDefault(x => x.DriverType == SKDDriverType.System);
				SKDConfiguration.RootDevice = new SKDDevice()
				{
					DriverUID = driver.UID,
					Name = "Система"
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
			SKDConfiguration.Reorder();

			Invalidate();
		}

		public static string GetPresentationZone(SKDDevice device)
		{
			if (device.Zone != null)
				return device.Zone.Name;
			return "";
		}

		public static string GetPresentationOuterZone(SKDDevice device)
		{
			if (device.OuterZone != null)
				return device.OuterZone.Name;
			return "";
		}

		public static void CreateStates()
		{
			foreach (var device in Devices)
			{
				
				device.State = new SKDDeviceState(device);
				if (device.DriverType == SKDDriverType.System)
					device.State.IsInitialState = false;
			}
			foreach (var zone in Zones)
			{
				zone.State = new SKDZoneState(zone);
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

		static void Invalidate()
		{
			ClearAllReferences();
			InitializeDevicesInZone();
			InvalidateIntervals();
		}

		static void ClearAllReferences()
		{
			foreach (var device in Devices)
			{
				device.Zone = null;
				device.OuterZone = null;
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
				if (device.Driver.HasZone)
				{
					device.Zone = Zones.FirstOrDefault(x => x.UID == device.ZoneUID);
					if (device.Zone != null)
					{
						device.Zone.Devices.Add(device);
					}
					else
						device.ZoneUID = Guid.Empty;
				}
				else
					device.ZoneUID = Guid.Empty;

				if (device.Driver.HasOuterZone)
				{
					device.OuterZone = Zones.FirstOrDefault(x => x.UID == device.OuterZoneUID);
					if (device.OuterZone != null)
					{
						device.OuterZone.Devices.Add(device);
					}
					else
						device.OuterZoneUID = Guid.Empty;
				}
				else
					device.OuterZoneUID = Guid.Empty;
			}
		}

		static void InvalidateIntervals()
		{
			foreach (var weeklyInterval in SKDConfiguration.WeeklyIntervals)
			{
			}
		}
	}
}