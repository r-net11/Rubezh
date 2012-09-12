using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;

namespace FiresecClient
{
    public partial class XManager
    {
        public static void CreateStates()
        {
            var deviceStates = new XDeviceConfigurationStates();
            foreach (var device in DeviceConfiguration.Devices)
            {
                var deviceState = new XDeviceState()
                {
                    Device = device,
                    UID = device.UID,
                };
				device.DeviceState = deviceState;
                deviceStates.DeviceStates.Add(deviceState);
            }
			foreach (var deviceState in deviceStates.DeviceStates)
            {
                if (deviceState.Device.Parent != null)
                {
					deviceState.Parent = deviceStates.DeviceStates.FirstOrDefault(x => x.Device == deviceState.Device.Parent);
                }
                foreach (var childDevice in deviceState.Device.Children)
                {
					deviceState.Children.Add(deviceStates.DeviceStates.FirstOrDefault(x => x.Device == childDevice));
                }
            }
            foreach (var zone in DeviceConfiguration.Zones)
            {
                var zoneState = new XZoneState()
                {
                    Zone = zone,
					UID = zone.UID
                };
				zone.ZoneState = zoneState;
                deviceStates.ZoneStates.Add(zoneState);
            }
			foreach (var direction in DeviceConfiguration.Directions)
			{
				var directionState = new XDirectionState()
				{
					Direction = direction,
					UID = direction.UID
				};
				direction.DirectionState = directionState;
				deviceStates.DirectionStates.Add(directionState);
			}
			DeviceStates = deviceStates;
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
            foreach (var child in deviceState.Children)
            {
                alDevicelChildren.Add(child);
                AllChildren(child);
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