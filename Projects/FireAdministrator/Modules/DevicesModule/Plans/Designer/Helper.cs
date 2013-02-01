using System;
using System.Linq;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using System.Collections.Generic;

namespace DevicesModule.Plans.Designer
{
	public static class Helper
	{
		private static Dictionary<Guid, Zone> _zoneMap;
		private static Dictionary<Guid, Device> _deviceMap;
		public static void BuildMap()
		{
			BuildZoneMap();
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
			element.BackgroundColor = GetZoneColor(zone);
		}
		public static void SetZone(IElementZone element, Zone zone)
		{
			element.ZoneUID = zone == null ? Guid.Empty : zone.UID;
			element.BackgroundColor = GetZoneColor(zone);
		}
		public static Color GetZoneColor(Zone zone)
		{
			Color color = Colors.Gray;
			if (zone != null)
			{
				if (zone.ZoneType == ZoneType.Fire)
					color = Colors.Green;

				if (zone.ZoneType == ZoneType.Guard)
					color = Colors.Brown;
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
			if (device != null)
				device.PlanElementUIDs.Add(element.UID);
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
