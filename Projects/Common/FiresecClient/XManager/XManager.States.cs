using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;

namespace FiresecClient
{
    public partial class XManager
    {
        public static void CreateStates()
        {
            DeviceStates = new XDeviceConfigurationStates();
            foreach (var device in DeviceConfiguration.Devices)
            {
                var deviceState = new XDeviceState()
                {
                    Device = device,
                    UID = device.UID,
                };
                DeviceStates.DeviceStates.Add(deviceState);
            }
            foreach (var deviceState in DeviceStates.DeviceStates)
            {
                if (deviceState.Device.Parent != null)
                {
                    deviceState.Parent = DeviceStates.DeviceStates.FirstOrDefault(x => x.Device == deviceState.Device.Parent);
                }
                foreach (var childDevice in deviceState.Device.Children)
                {
                    deviceState.Children.Add(DeviceStates.DeviceStates.FirstOrDefault(x => x.Device == childDevice));
                }
            }
            foreach (var zone in DeviceConfiguration.Zones)
            {
                var zoneState = new XZoneState()
                {
                    Zone = zone,
					UID = zone.UID
                };
                DeviceStates.ZoneStates.Add(zoneState);
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
            foreach (var child in deviceState.Children)
            {
                alDevicelChildren.Add(child);
                AllChildren(child);
            }
        }

		public static List<XZoneState> GetAllGKZoneStates(XDeviceState gkDeviceState)
		{
			var zoneStates = new List<XZoneState>();
			foreach (var zoneState in DeviceStates.ZoneStates)
			{
				if (zoneState.Zone.GkDatabaseParent == gkDeviceState.Device)
				{
					zoneStates.Add(zoneState);
				}
			}
			return zoneStates;
		}
    }
}