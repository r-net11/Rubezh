using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XDeviceConfiguration
    {
        public XDeviceConfiguration()
        {
            Devices = new List<XDevice>();
            Zones = new List<XZone>();
        }

        public List<XDevice> Devices { get; set; }

        [DataMember]
        public XDevice RootDevice { get; set; }

        [DataMember]
        public List<XZone> Zones { get; set; }

        public void Update()
        {
            Devices = new List<XDevice>();
            if (RootDevice != null)
            {
                RootDevice.Parent = null;
                Devices.Add(RootDevice);
                AddChild(RootDevice);
            }
        }

        void AddChild(XDevice parentDevice)
        {
            foreach (var device in parentDevice.Children)
            {
                device.Parent = parentDevice;
                Devices.Add(device);
                AddChild(device);
            }
        }
    }
}