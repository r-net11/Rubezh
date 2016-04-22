using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace FiresecAPI.SKD
{
	public partial class SKDManager
	{
		public static void EditDevice(SKDDevice device)
		{
			if (device.Door == null)
			{
				if (device.Children == null) return;

				foreach (var child in device.Children.Where(child => child.Door != null))
				{
					child.Door.OnChanged();
				}
			}
			else
			{
				device.Door.OnChanged();
			}
		}

		public static void DeleteDevice(SKDDevice device)
		{
			foreach (var subDevice in device.Children)
				DeleteDeviceInternal(subDevice);
			DeleteDeviceInternal(device);
			if (device.Parent != null)
				device.Parent.Children.Remove(device);
		}

		private static void DeleteDeviceInternal(SKDDevice device)
		{
			if (device.Zone != null)
			{
				device.Zone.Devices.Remove(device);
				device.Zone.OnChanged();
			}
			if (device.Door != null)
			{
				device.Door.InDeviceUID = Guid.Empty;
				device.Door.OutDeviceUID = Guid.Empty;
				device.Door.OnChanged();
			}
			Devices.Remove(device);
			device.OnChanged();
		}

		public static void ChangeDeviceZone(SKDDevice device, SKDZone zone)
		{
			if (device.Zone != null && device.Zone != zone)
			{
				device.Zone.Devices.Remove(device);
				device.Zone.OnChanged();
			}
			else
			{
				zone.Devices.Add(device);
			}
			device.ZoneUID = zone.UID;
			device.Zone = zone;
		}

		/// <summary>
		/// Удаляет устройство из зоны
		/// </summary>
		/// <param name="device">Устройство</param>
		public static void RemoveDeviceFromZone(SKDDevice device)
		{
			if (device == null)
				throw new ArgumentNullException("RemoveDeviceFromZone вызван с null аргументом", "device");

			if (device.Zone == null)
				return;
			
			device.Zone.Devices.Remove(device);
			device.Zone.OnChanged();
			device.ZoneUID = Guid.Empty;
			device.Zone = null;
		}

		public static void EditZone(SKDZone zone)
		{
			foreach (var device in zone.Devices)
				device.OnChanged();
			zone.OnChanged();
		}

		public static void RemoveZone(SKDZone zone)
		{
			foreach (var device in zone.Devices)
			{
				device.Zone = null;
				device.ZoneUID = Guid.Empty;
				device.OnChanged();
			}
			Zones.Remove(zone);
			zone.OnChanged();
		}

		public static void RemoveDeviceDoor(SKDDevice device, SKDDoor door)
		{
			if (device.Children == null) return;

			foreach (var child in device.Children)
			{
				var s = child.NameWithParent;
				if (child.Door == null || child.Door.UID != door.UID) continue;

				child.Door = null;
				child.OnChanged();
			}
		}

		public static void ChangeDoorDevice(SKDDoor door, SKDDevice device)
		{
			if (door.InDevice != null)
				door.InDevice.Door = null;
			if (door.OutDevice != null)
				door.OutDevice.Door = null;

			if (device != null)
			{
				door.InDeviceUID = device.UID;
				device.Door = door;
			}
			else
			{
				door.InDeviceUID = Guid.Empty;
				door.InDevice = null;
				door.OutDeviceUID = Guid.Empty;
				door.OutDevice = null;
			}
			UpdateDoor(door);
		}

		public static void UpdateDoor(SKDDoor door)
		{
			door.InDevice = Devices.FirstOrDefault(x => x.UID == door.InDeviceUID);
			door.OutDevice = Devices.FirstOrDefault(x => x.UID == door.OutDeviceUID);

			if (door.InDevice != null && door.InDevice.Parent != null)
			{
				switch (door.DoorType)
				{
					case DoorType.OneWay:
						door.OutDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Button && x.IntAddress == door.InDevice.IntAddress);
						break;

					case DoorType.TwoWay:
						door.OutDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Reader && x.IntAddress == door.InDevice.IntAddress + 1);
						break;
				}
				if (door.OutDevice != null)
				{
					door.OutDeviceUID = door.OutDevice.UID;
				}
			}

			if (door.InDevice != null)
			{
				door.InDevice.Door = door;
			}
			if (door.OutDevice != null)
			{
				door.OutDevice.Door = door;
			}
		}

		public static void RemoveDoor(SKDDoor door)
		{
			if (door.InDevice != null)
			{
				door.InDevice.Door = null;
				door.InDevice.OnChanged();
			}

			if (door.OutDevice != null)
			{
				door.OutDevice.Door = null;
				door.OutDevice.OnChanged();
			}

			SKDManager.SKDConfiguration.Doors.Remove(door);
		}

		public static bool ValidateIPAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}

			IPAddress ipAddress = null;
			return IPAddress.TryParse(address, out ipAddress);

			const string pattern = @"^([01]\d\d?|[01]?[1-9]\d?|2[0-4]\d|25[0-3])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])$";
			if (string.IsNullOrEmpty(address) || !Regex.IsMatch(address, pattern))
			{
				return false;
			}
			return true;
		}
	}
}