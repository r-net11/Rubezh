using System.Collections.Generic;
using XFiresecAPI;

namespace FiresecClient
{
    public partial class XManager
    {
        public static void CreateStates()
        {
            foreach (var device in Devices)
            {
                var deviceState = new XDeviceState(device);
				deviceState.UID = device.UID;
				device.DeviceState = deviceState;
            }
            foreach (var zone in Zones)
            {
                var zoneState = new XZoneState()
                {
                    Zone = zone
                };
				zoneState.UID = zone.UID;
				zone.ZoneState = zoneState;
            }
			foreach (var direction in Directions)
			{
				var directionState = new XDirectionState()
				{
					Direction = direction
				};
				directionState.UID = direction.UID;
				direction.DirectionState = directionState;
			}
        }

        static List<XDevice> allDeviceChildren;
        public static List<XDevice> GetAllDeviceChildren(XDevice device)
        {
            allDeviceChildren = new List<XDevice>();
            AddChildren(device);
			allDeviceChildren.RemoveAt(0);
            return allDeviceChildren;
        }
        public static void AddChildren(XDevice device)
        {
            allDeviceChildren.Add(device);
			if ((device.Children != null) && (device.Children.Count != 0))
			{
				foreach (var childDevice in device.Children)
				{
					AddChildren(childDevice);
				}
			}
        }

		public static List<XZoneState> GetAllGKZoneStates(XDeviceState gkDeviceState)
		{
			var zoneStates = new List<XZoneState>();
			foreach (var zone in Zones)
			{
				if (zone.GkDatabaseParent == gkDeviceState.Device)
				{
					zoneStates.Add(zone.ZoneState);
				}
			}
			return zoneStates;
		}

		public static List<XDirectionState> GetAllGKDirectionStates(XDeviceState gkDeviceState)
		{
			var directionStates = new List<XDirectionState>();
			foreach (var direction in Directions)
			{
				if (direction.GkDatabaseParent == gkDeviceState.Device)
				{
					directionStates.Add(direction.DirectionState);
				}
			}
			return directionStates;
		}

		public static XStateClass GetMinStateClass()
		{
			var minStateClass = XStateClass.No;
			foreach (var device in XManager.Devices)
			{
				var stateClass = device.DeviceState.StateClass;
				if (device.DriverType == XDriverType.AM1_T && stateClass == XStateClass.Fire2)
				{
					stateClass = XStateClass.Info;
				}
				if (stateClass < minStateClass)
					minStateClass = device.DeviceState.StateClass;
			}
			foreach (var zone in XManager.Zones)
			{
				if (zone.ZoneState.StateClass < minStateClass)
					minStateClass = zone.ZoneState.StateClass;
			}
			foreach (var direction in XManager.Directions)
			{
				if (direction.DirectionState.StateClass < minStateClass)
					minStateClass = direction.DirectionState.StateClass;
			}
			return minStateClass;
		}
    }
}