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
				SKDConfiguration.RootDevice = new SKDDevice()
				{
					DriverUID = driver.UID,
                    Name = Resources.Language.SKD.SKDManager.SKDManager.Name
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
				door.State = new SKDDoorState(door);
				door.State.StateClass = XStateClass.Norm;
			}
		}

		public static XStateClass GetMinStateClass()
		{
			var minStateClass = XStateClass.No;
			foreach (var device in Devices)
				if (device.IsRealDevice)
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
			foreach (var device in Devices)
			{
				device.Zone = null;
			}
			foreach (var zone in Zones)
			{
				zone.Devices = new List<SKDDevice>();
			}
		}

		private static void InvalidateLockConfiguration()
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
		}

		private static void InitializeDevicesInZone()
		{
			foreach (var device in Devices)
			{
				if (device.Driver != null)
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
		}

		private static void InvalidateDoors()
		{
			foreach (var door in SKDManager.Doors)
			{
				UpdateDoor(door);
				if (door.InDevice == null)
				{
					door.InDeviceUID = Guid.Empty;
				}
				if (door.OutDevice == null)
				{
					door.OutDeviceUID = Guid.Empty;
				}
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