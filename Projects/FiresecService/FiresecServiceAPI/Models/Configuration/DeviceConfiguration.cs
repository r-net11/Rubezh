using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DeviceConfiguration
    {
        public DeviceConfiguration()
        {
            Zones = new List<Zone>();
            Directions = new List<Direction>();
            GuardUsers = new List<GuardUser>();
            GuardLevels = new List<GuardLevel>();
        }

        public List<Device> Devices { get; set; }

        [DataMember]
        public Device RootDevice { get; set; }

        [DataMember]
        public List<Zone> Zones { get; set; }

        [DataMember]
        public List<Direction> Directions { get; set; }

        [DataMember]
        public List<GuardUser> GuardUsers { get; set; }

        [DataMember]
        public List<GuardLevel> GuardLevels { get; set; }

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