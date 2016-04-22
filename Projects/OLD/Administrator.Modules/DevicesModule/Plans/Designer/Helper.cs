using System;
using System.Collections.Generic;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Plans.Elements;

namespace DevicesModule.Plans.Designer
{
	public static class Helper
	{
		private static Dictionary<Guid, Zone> _zoneMap;
		private static Dictionary<Guid, Device> _deviceMap;
		public static void BuildMap()
		{
			BuildZoneMap();
			BuildDeviceMap();
		}
		public static void BuildDeviceMap()
		{
			_deviceMap = new Dictionary<Guid, Device>();
			FiresecManager.Devices.ForEach(item => _deviceMap.Add(item.UID, item));
		}
		public static void BuildZoneMap()
		{
			_zoneMap = new Dictionary<Guid, Zone>();
			FiresecManager.Zones.ForEach(item => _zoneMap.Add(item.UID, item));
		}

		public static Zone GetZone(IElementZone element)
		{
			return GetZone(element.ZoneUID);
		}
		public static Zone GetZone(Guid zoneUID)
		{
			return zoneUID != Guid.Empty && _zoneMap.ContainsKey(zoneUID) ? _zoneMap[zoneUID] : null;
		}
		public static void SetZone(IElementZone element)
		{
			Zone zone = GetZone(element);
			SetZone(element, zone);
		}
		public static void SetZone(IElementZone element, Zone zone)
		{
			ResetZone(element);
			element.ZoneUID = zone == null ? Guid.Empty : zone.UID;
			element.BackgroundColor = GetZoneColor(zone);
			if (zone != null)
				zone.PlanElementUIDs.Add(element.UID);
		}
		public static void ResetZone(IElementZone element)
		{
			Zone zone = GetZone(element);
			if (zone != null)
				zone.PlanElementUIDs.Remove(element.UID);
		}
		public static Color GetZoneColor(Zone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				switch (zone.ZoneType)
				{
					case ZoneType.Fire:
						color = Colors.Green;
						break;
					case ZoneType.Guard:
						color = Colors.Brown;
						break;
				}
			return color;
		}

		public static Device GetDevice(ElementDevice element)
		{
			return element.DeviceUID != Guid.Empty && _deviceMap.ContainsKey(element.DeviceUID) ? _deviceMap[element.DeviceUID] : null;
		}
		public static void SetDevice(ElementDevice element, Device device)
		{
			ResetDevice(element);
			element.DeviceUID = device == null ? Guid.Empty : device.UID;
			if (device != null)
				device.PlanElementUIDs.Add(element.UID);
		}
		public static Device SetDevice(ElementDevice element)
		{
			Device device = GetDevice(element);
			SetDevice(element, device);
			return device;
		}
		public static void ResetDevice(ElementDevice element)
		{
			Device device = GetDevice(element);
			if (device != null)
				device.PlanElementUIDs.Remove(element.UID);
		}

		public static string GetZoneTitle(IElementZone element)
		{
			Zone zone = GetZone(element);
			return GetZoneTitle(zone);
		}
		public static string GetZoneTitle(Zone zone)
		{
			return zone == null ? "Несвязанная зона" : zone.PresentationName;
		}
		public static string GetDeviceTitle(ElementDevice element)
		{
			var device = GetDevice(element);
			return device == null ? "Неизвестное устройство" : device.DottedPresentationAddressAndName;
		}
	}
}
