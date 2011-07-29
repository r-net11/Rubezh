﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class CurrentConfiguration
    {
        public CurrentConfiguration()
        {
            Journals = new List<JournalFilter>();
        }

        public List<Device> Devices { get; set; }

        [DataMember]
        public Device RootDevice { get; set; }

        [DataMember]
        public List<Driver> Drivers { get; set; }

        [DataMember]
        public List<Zone> Zones { get; set; }

        [DataMember]
        public List<Direction> Directions { get; set; }

        [DataMember]
        public List<JournalFilter> Journals { get; set; }

        [DataMember]
        public List<User> Users { get; set; }

        [DataMember]
        public List<UserGroup> UserGroups { get; set; }

        [DataMember]
        public List<Perimission> Perimissions { get; set; }

        public void UpdateDrivers()
        {
            foreach (var device in Devices)
            {
                device.Driver = Drivers.FirstOrDefault(x => x.Id == device.DriverId);
            }
        }

        public void Update()
        {
            Devices = new List<Device>();
            RootDevice.Parent = null;
            Devices.Add(RootDevice);
            AddChild(RootDevice);
        }

        void AddChild(Device parentDevice)
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
