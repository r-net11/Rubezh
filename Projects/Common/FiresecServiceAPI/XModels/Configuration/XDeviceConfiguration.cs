using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace XFiresecAPI
{
    [DataContract]
	public class XDeviceConfiguration : VersionedConfiguration
    {
        public XDeviceConfiguration()
        {
            Devices = new List<XDevice>();
            Zones = new List<XZone>();
			Directions = new List<XDirection>();
        }

        public List<XDevice> Devices { get; set; }

        [DataMember]
        public XDevice RootDevice { get; set; }

        [DataMember]
        public List<XZone> Zones { get; set; }

        [DataMember]
		public List<XDirection> Directions { get; set; }

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