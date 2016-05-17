using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.GK;

namespace StrazhAPI.SKD
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

		public static List<SKDDoor> Doors
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
		}

		public static void UpdateConfiguration()
		{
			if (SKDConfiguration.RootDevice == null)
			{
				var driver = Drivers.FirstOrDefault(x => x.DriverType == SKDDriverType.System);
				SKDConfiguration.RootDevice = new SKDDevice
				{
					DriverUID = driver.UID,
					Name = "Система"
				};
			}

			SKDConfiguration.Update();
			foreach (var device in SKDConfiguration.Devices)
				device.Driver = Drivers.FirstOrDefault(x => x.UID == device.DriverUID);

			Invalidate();
		}

		public static string GetPresentationZone(SKDDevice device)
		{
			return device.Zone != null ? device.Zone.Name : string.Empty;
		}

		public static void CreateStates()
		{
			foreach (var device in Devices)
			{
				device.State = new SKDDeviceState(device) {UID = device.UID, StateClass = XStateClass.Unknown};
				if (device.DriverType == SKDDriverType.System)
				{
					device.State.IsInitialState = false;
					device.State.StateClass = XStateClass.Norm;
				}
			}

			foreach (var zone in Zones)
				zone.State = new SKDZoneState(zone) {StateClass = XStateClass.Norm};

			foreach (var door in Doors)
				door.State = new SKDDoorState(door) {StateClass = XStateClass.Norm};
		}

		public static XStateClass GetMinStateClass()
		{
			var minStateClass = XStateClass.No;
			foreach (var device in Devices.Where(x => x.IsRealDevice))
			{
				var stateClass = device.State.StateClass;
				if (stateClass < minStateClass)
					minStateClass = device.State.StateClass;
			}
			return minStateClass;
		}

		private static void Invalidate()
		{
			ClearAllReferences();
			InitializeDevicesInZone();
			InvalidateLockConfiguration();
			InvalidateDoors();
			InvalidateIntervals();
		}

		private static void ClearAllReferences()
		{
			Devices.ForEach(x => x.Zone = null);
			Zones.ForEach(x => x.Devices = new List<SKDDevice>());
		}

		private static void InvalidateLockConfiguration()
		{
			Devices.ForEach(InvalidateOneLockConfiguration);
		}

		public static void InvalidateOneLockConfiguration(SKDDevice device)
		{
			if (device.DriverType != SKDDriverType.Lock) return;

			if (device.SKDDoorConfiguration == null)
				device.SKDDoorConfiguration = new SKDDoorConfiguration();

			var doorDayIntervalsCollection = device.SKDDoorConfiguration.DoorDayIntervalsCollection;
			if (doorDayIntervalsCollection.DoorDayIntervals.Count != 7)
			{
				doorDayIntervalsCollection.DoorDayIntervals = new List<DoorDayInterval>();
				for (var i = 0; i < 7; i++)
				{
					doorDayIntervalsCollection.DoorDayIntervals.Add(new DoorDayInterval());
				}
				foreach (var doorDayInterval in doorDayIntervalsCollection.DoorDayIntervals)
				{
					if (doorDayInterval.DoorDayIntervalParts.Count != 4)
					{
						doorDayInterval.DoorDayIntervalParts = new List<DoorDayIntervalPart>();
						for (var i = 0; i < 4; i++)
						{
							doorDayInterval.DoorDayIntervalParts.Add(new DoorDayIntervalPart());
						}
					}
				}
			}
		}

		private static void InitializeDevicesInZone()
		{
			foreach (var device in Devices.Where(device => device.Driver != null))
			{
				if (device.Driver.HasZone)
				{
					device.Zone = Zones.FirstOrDefault(x => x.UID == device.ZoneUID);

					if (device.Zone != null)
						device.Zone.Devices.Add(device);
					else
						device.ZoneUID = Guid.Empty;
				}
				else
					device.ZoneUID = Guid.Empty;
			}
		}

		private static void InvalidateDoors()
		{
			foreach (var door in Doors)
			{
				UpdateDoor(door);

				if (door.InDevice == null)
					door.InDeviceUID = Guid.Empty;

				if (door.OutDevice == null)
					door.OutDeviceUID = Guid.Empty;
			}
		}

		private static void InvalidateIntervals()
		{
			//foreach (var weeklyInterval in SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals)
			//{
			//}
		}

		/// <summary>
		/// Получает точку доступа для замка
		/// </summary>
		/// <param name="device">Замок</param>
		/// <returns>Точка доступа</returns>
		public static SKDDoor GetDoorForLock(SKDDevice device)
		{
			foreach (var door in SKDManager.Doors)
			{
				if (door.InDevice != null)
				{
					var lockAddress = door.InDevice.IntAddress;
					if (door.DoorType == DoorType.TwoWay)
					{
						lockAddress = door.InDevice.IntAddress / 2;
					}
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice == device)
					{
						return door;
					}
				}
			}

			// Точка доступа для замка не задана
			return null;
		}

		/// <summary>
		/// Синхронизирует режим точки доступа согласно режиму замка
		/// </summary>
		/// <param name="device">Замок</param>
		public static SKDDoor SynchronizeDoorAccessStateForLock(SKDDevice device)
		{
			var door = GetDoorForLock(device);
			if (door != null)
				door.State.AccessState = device.State.AccessState;
			return door;
		}
	}
}