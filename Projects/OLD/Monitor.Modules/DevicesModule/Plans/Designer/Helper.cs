using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Plans.Elements;

namespace DevicesModule.Plans.Designer
{
	public static class Helper
	{
		private static Dictionary<Guid, Zone> _zoneMap;
		private static Dictionary<Guid, Device> _deviceMap;
		private static Dictionary<Guid, List<Device>> _zoneDevices;
		public static void BuildMap()
		{
			_zoneMap = new Dictionary<Guid, Zone>();
			FiresecManager.Zones.ForEach(item => _zoneMap.Add(item.UID, item));
			_deviceMap = new Dictionary<Guid, Device>();
			FiresecManager.Devices.ForEach(item => _deviceMap.Add(item.UID, item));
			_zoneDevices = new Dictionary<Guid, List<Device>>();
			FiresecManager.Devices.ForEach(item =>
				{
					if (!_zoneDevices.ContainsKey(item.ZoneUID))
						_zoneDevices.Add(item.ZoneUID, new List<Device>());
					_zoneDevices[item.ZoneUID].Add(item);
				});
		}

		public static Zone GetZone(IElementZone element)
		{
			return GetZone(element.ZoneUID);
		}
		public static Zone GetZone(Guid zoneUID)
		{
			return zoneUID != Guid.Empty && _zoneMap.ContainsKey(zoneUID) ? _zoneMap[zoneUID] : null;
		}

		public static Device GetDevice(ElementDevice element)
		{
			return element.DeviceUID != Guid.Empty && _deviceMap.ContainsKey(element.DeviceUID) ? _deviceMap[element.DeviceUID] : null;
		}
		
		public static List<Device> GetDevices(IElementZone element)
		{
			return GetDevices(element.ZoneUID);
		}
		public static List<Device> GetDevices(Guid zoneUID)
		{
			return _zoneDevices.ContainsKey(zoneUID) ? _zoneDevices[zoneUID] : new List<Device>();
		}
	}
}
