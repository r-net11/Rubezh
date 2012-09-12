using System.Collections.Generic;
using XFiresecAPI;

namespace FiresecClient
{
    public partial class XManager
    {
        public static void CreateStates()
        {
            foreach (var device in DeviceConfiguration.Devices)
            {
                var deviceState = new XDeviceState()
                {
                    Device = device
                };
				device.DeviceState = deviceState;
            }
            foreach (var zone in DeviceConfiguration.Zones)
            {
                var zoneState = new XZoneState()
                {
                    Zone = zone
                };
				zone.ZoneState = zoneState;
            }
			foreach (var direction in DeviceConfiguration.Directions)
			{
				var directionState = new XDirectionState()
				{
					Direction = direction
				};
				direction.DirectionState = directionState;
			}
        }

        static List<XDeviceState> alDevicelChildren;
        public static List<XDeviceState> GetAllDeviceChildren(XDeviceState gkDeviceState)
        {
            alDevicelChildren = new List<XDeviceState>();
            AllChildren(gkDeviceState);
            return alDevicelChildren;
        }
        public static void AllChildren(XDeviceState deviceState)
        {
            alDevicelChildren.Add(deviceState);
            foreach (var childDevice in deviceState.Device.Children)
            {
				alDevicelChildren.Add(childDevice.DeviceState);
				AllChildren(childDevice.DeviceState);
            }
        }

		public static List<XZoneState> GetAllGKZoneStates(XDeviceState gkDeviceState)
		{
			var zoneStates = new List<XZoneState>();
			foreach (var zone in DeviceConfiguration.Zones)
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
			foreach (var direction in DeviceConfiguration.Directions)
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