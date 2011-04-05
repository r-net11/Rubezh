using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class CurrentConfiguration
    {
        [IgnoreDataMember]
        public List<Device> AllDevices { get; set; }

        [DataMember]
        public Device RootDevice { get; set; }

        [DataMember]
        public List<Zone> Zones { get; set; }

        [DataMember]
        public Firesec.Metadata.config Metadata { get; set; }

        public void FillAllDevices()
        {
            AllDevices = new List<Device>();
            RootDevice.Parent = null;
            AllDevices.Add(RootDevice);
            AddChild(RootDevice);
        }

        void AddChild(Device parentDevice)
        {
            foreach (Device device in parentDevice.Children)
            {
                device.Parent = parentDevice;
                AllDevices.Add(device);
                AddChild(device);
            }
        }
    }
}
