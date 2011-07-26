using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FiresecClient.Converters;

namespace FiresecClient.Models
{
    [DataContract]
    public class CurrentConfiguration
    {
        [DataMember]
        public List<Driver> Drivers { get; set; }

        [DataMember]
        public List<Device> Devices { get; set; }

        [DataMember]
        public Device RootDevice { get; set; }

        [DataMember]
        public List<Zone> Zones { get; set; }

        [DataMember]
        public List<Direction> Directions { get; set; }


        public List<User> Users { get; set; }
        public List<UserGroup> UserGroups { get; set; }
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

        public void FillDrivrs(Firesec.Metadata.config metadata)
        {
            DriverConverter.Metadata = metadata;
            Drivers = new List<Driver>();
            foreach (var firesecDriver in metadata.drv)
            {
                Driver driver = DriverConverter.Convert(firesecDriver);
                if (driver.IsIgnore == false)
                {
                    Drivers.Add(driver);
                }
            }
        }
    }
}
