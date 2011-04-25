using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecClient
{
    public class CurrentConfiguration
    {
        public List<Device> AllDevices { get; set; }
        public Device RootDevice { get; set; }
        public List<Zone> Zones { get; set; }
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

        public void SetUnderlyingZones()
        {
            foreach (Device device in AllDevices)
            {
                device.UderlyingZones = new List<string>();
            }

            foreach (Device device in AllDevices)
            {
                if (string.IsNullOrEmpty(device.ZoneNo) == false)
                {
                    device.AddUnderlyingZone(device.ZoneNo);
                }
            }
        }
    }
}
