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
				device.DeviceState = deviceState;
            }
            foreach (var zone in Zones)
            {
                var zoneState = new XZoneState()
                {
                    Zone = zone
                };
				zone.ZoneState = zoneState;
            }
			foreach (var direction in Directions)
			{
				var directionState = new XDirectionState()
				{
					Direction = direction
				};
				direction.DirectionState = directionState;
			}
        }

        static List<XDevice> allDeviceChildren;
        public static List<XDevice> GetAllDeviceChildren(XDevice device)
        {
            allDeviceChildren = new List<XDevice>();
            AddChildren(device);
            return allDeviceChildren;
        }
        public static void AddChildren(XDevice device)
        {
            allDeviceChildren.Add(device);
            foreach (var childDevice in device.Children)
            {
				AddChildren(childDevice);
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
    }
}