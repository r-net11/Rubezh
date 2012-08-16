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
                    No = zone.No
                };
                DeviceStates.ZoneStates.Add(zoneState);
            }
        }

        static List<XDeviceState> allChildren;
        public static List<XDeviceState> GetAllChildren(XDeviceState deviceState)
        {
            allChildren = new List<XDeviceState>();
            AllChildren(deviceState);
            return allChildren;
        }
        public static void AllChildren(XDeviceState deviceState)
        {
            allChildren.Add(deviceState);
            foreach (var child in deviceState.Children)
            {
                allChildren.Add(child);
                AllChildren(child);
            }
        }
    }
}