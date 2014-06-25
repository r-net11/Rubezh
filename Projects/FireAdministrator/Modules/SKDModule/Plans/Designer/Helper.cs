using System;
using System.Collections.Generic;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrustructure.Plans.Elements;

namespace SKDModule.Plans.Designer
{
	internal static class Helper
	{
		private static Dictionary<Guid, SKDDevice> _deviceMap;
		private static Dictionary<Guid, SKDZone> _zoneMap;
		private static Dictionary<Guid, Door> _doorMap;
		public static void BuildMap()
		{
			BuildDeviceMap();
			BuildZoneMap();
			BuildDoorMap();
		}
		public static void BuildDeviceMap()
		{
			_deviceMap = new Dictionary<Guid, SKDDevice>();
			foreach (var device in SKDManager.Devices)
			{
				if (!_deviceMap.ContainsKey(device.UID))
					_deviceMap.Add(device.UID, device);
			}
		}
		public static void BuildZoneMap()
		{
			_zoneMap = new Dictionary<Guid, SKDZone>();
			foreach (var zone in SKDManager.Zones)
			{
				if (!_zoneMap.ContainsKey(zone.UID))
					_zoneMap.Add(zone.UID, zone);
			}
		}
		public static void BuildDoorMap()
		{
			_doorMap = new Dictionary<Guid, Door>();
			foreach (var door in SKDManager.SKDConfiguration.Doors)
			{
				if (!_doorMap.ContainsKey(door.UID))
					_doorMap.Add(door.UID, door);
			}
		}

		public static string GetSKDZoneTitle(IElementZone element)
		{
			SKDZone zone = GetSKDZone(element);
			return GetSKDZoneTitle(zone);
		}
		public static string GetSKDZoneTitle(SKDZone zone)
		{
			return zone == null ? "Несвязанная зона" : zone.Name;
		}
		public static SKDZone GetSKDZone(IElementZone element)
		{
			return GetSKDZone(element.ZoneUID);
		}
		public static SKDZone GetSKDZone(Guid zoneUID)
		{
			return zoneUID != Guid.Empty && _zoneMap.ContainsKey(zoneUID) ? _zoneMap[zoneUID] : null;
		}
		public static void SetSKDZone(IElementZone element)
		{
			SKDZone zone = GetSKDZone(element);
			SetSKDZone(element, zone);
		}
		public static void SetSKDZone(IElementZone element, Guid zoneUID)
		{
			SKDZone zone = GetSKDZone(zoneUID);
			SetSKDZone(element, zone);
		}
		public static void SetSKDZone(IElementZone element, SKDZone zone)
		{
			ResetSKDZone(element);
			element.ZoneUID = zone == null ? Guid.Empty : zone.UID;
			element.BackgroundColor = GetSKDZoneColor(zone);
			if (zone != null)
				zone.PlanElementUIDs.Add(element.UID);
		}
		public static void ResetSKDZone(IElementZone element)
		{
			SKDZone zone = GetSKDZone(element);
			if (zone != null)
				zone.PlanElementUIDs.Remove(element.UID);
		}
		public static Color GetSKDZoneColor(SKDZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Green;
			return color;
		}

		public static SKDDevice GetSKDDevice(ElementSKDDevice element)
		{
			return element.DeviceUID != Guid.Empty && _deviceMap.ContainsKey(element.DeviceUID) ? _deviceMap[element.DeviceUID] : null;
		}
		public static string GetSKDDeviceTitle(ElementSKDDevice element)
		{
			var device = GetSKDDevice(element);
			return device == null ? "Неизвестное устройство" : device.Name;
		}
		public static string GetSKDDeviceImageSource(ElementSKDDevice element)
		{
			var device = GetSKDDevice(element);
			return device == null ? null : device.Driver.ImageSource;
		}
		public static SKDDevice SetSKDDevice(ElementSKDDevice element)
		{
			SKDDevice device = GetSKDDevice(element);
			if (device != null)
				device.PlanElementUIDs.Add(element.UID);
			return device;
		}
		public static void SetSKDDevice(ElementSKDDevice element, SKDDevice device)
		{
			ResetSKDDevice(element);
			element.DeviceUID = device == null ? Guid.Empty : device.UID;
			if (device != null)
				device.PlanElementUIDs.Add(element.UID);
		}
		public static void ResetSKDDevice(ElementSKDDevice element)
		{
			SKDDevice device = GetSKDDevice(element);
			if (device != null)
				device.PlanElementUIDs.Remove(element.UID);
		}


		public static Door GetDoor(ElementDoor element)
		{
			return element.DoorUID != Guid.Empty && _doorMap.ContainsKey(element.DoorUID) ? _doorMap[element.DoorUID] : null;
		}
		public static string GetDoorTitle(ElementDoor element)
		{
			var door = GetDoor(element);
			return door == null ? "Неизвестная дверь" : door.Name;
		}
		public static Door SetDoor(ElementDoor element)
		{
			Door door = GetDoor(element);
			if (door != null)
				door.PlanElementUIDs.Add(element.UID);
			return door;
		}
		public static void SetDoor(ElementDoor element, Door door)
		{
			ResetDoor(element);
			element.DoorUID = door == null ? Guid.Empty : door.UID;
			if (door != null)
				door.PlanElementUIDs.Add(element.UID);
		}
		public static void ResetDoor(ElementDoor element)
		{
			Door door = GetDoor(element);
			if (door != null)
				door.PlanElementUIDs.Remove(element.UID);
		}

	}
}