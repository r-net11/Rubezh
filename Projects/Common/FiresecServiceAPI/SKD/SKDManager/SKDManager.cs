using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
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

		public static List<Door> Doors
		{
			get { return SKDConfiguration.Doors; }
		}

		public static TimeIntervalsConfiguration TimeIntervalsConfiguration
		{
			get { return SKDConfiguration.TimeIntervalsConfiguration; }
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

		public static string GetPresentationZone(SKDDevice device)
		{
			if (device.Zone != null)
				return device.Zone.Name;
			return "";
		}

		public static void CreateStates()
		{
			foreach (var device in Devices)
			{
				
				device.State = new SKDDeviceState(device);
				device.State.UID = device.UID;
				device.State.StateClass = XStateClass.Unknown;
				if (device.DriverType == SKDDriverType.System)
				{
					device.State.IsInitialState = false;
					device.State.StateClass = XStateClass.Norm;
				}
			}
			foreach (var zone in Zones)
			{
				zone.State = new SKDZoneState(zone);
				zone.State.StateClass = XStateClass.Norm;
			}
			foreach (var door in Doors)
			{
				door.State = new DoorState(door);
				door.State.StateClass = XStateClass.Norm;
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
			InvalidateLockConfiguration();
			InvalidateDoors();
			InvalidateIntervals();
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

		static void InvalidateLockConfiguration()
		{
			foreach (var device in Devices)
			{
				InvalidateOneLockConfiguration(device);
			}
		}

		public static void InvalidateOneLockConfiguration(SKDDevice device)
		{
			if (device.DriverType == SKDDriverType.Lock)
			{
				if (device.SKDDoorConfiguration == null)
					device.SKDDoorConfiguration= new SKDDoorConfiguration();
				var doorDayIntervalsCollection = device.SKDDoorConfiguration.DoorDayIntervalsCollection;
				if (doorDayIntervalsCollection.DoorDayIntervals.Count != 7)
				{
					doorDayIntervalsCollection.DoorDayIntervals = new List<DoorDayInterval>();
					for (int i = 0; i < 7; i++)
					{
						doorDayIntervalsCollection.DoorDayIntervals.Add(new DoorDayInterval());
					}
					foreach (var doorDayInterval in doorDayIntervalsCollection.DoorDayIntervals)
					{
						if (doorDayInterval.DoorDayIntervalParts.Count != 4)
						{
							doorDayInterval.DoorDayIntervalParts = new List<DoorDayIntervalPart>();
							for (int i = 0; i < 4; i++)
							{
								doorDayInterval.DoorDayIntervalParts.Add(new DoorDayIntervalPart());
							}
						}
					}
				}
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
			}
		}

		static void InvalidateDoors()
		{
			foreach (var door in SKDManager.Doors)
			{
				var inDevice = SKDManager.Devices.FirstOrDefault(x=>x.UID == door.InDeviceUID);
				if (inDevice == null)
					door.InDeviceUID = Guid.Empty;

				var outDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == door.OutDeviceUID);
				if (outDevice == null)
					door.OutDeviceUID = Guid.Empty;
			}
		}

		static void InvalidateIntervals()
		{
			//foreach (var weeklyInterval in SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals)
			//{
			//}
		}
	}
}