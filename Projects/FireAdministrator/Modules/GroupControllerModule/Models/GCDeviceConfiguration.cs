using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class GCDeviceConfiguration
    {
        public GCDeviceConfiguration()
        {
            Devices = new List<GCDevice>();
        }

        public List<GCDevice> Devices { get; set; }

        [DataMember]
        public GCDevice RootDevice { get; set; }


        public void Update()
        {
            Devices = new List<GCDevice>();
            if (RootDevice != null)
            {
                RootDevice.Parent = null;
                Devices.Add(RootDevice);
                AddChild(RootDevice);
            }
        }

        void AddChild(GCDevice parentDevice)
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