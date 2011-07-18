using System.Collections.Generic;

namespace FiresecClient.Models
{
    public class CurrentConfiguration
    {
        public List<Driver> Drivers { get; set; }
        public List<Device> Devices { get; set; }
        public Device RootDevice { get; set; }
        public List<Zone> Zones { get; set; }
        public List<Direction> Directions { get; set; }
        public List<User> Users { get; set; }
        public List<UserGroup> UserGroups { get; set; }
        public List<Perimission> Perimissions { get; set; }

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
            Driver.Metadata = metadata;
            Drivers = new List<Driver>();
            foreach (var firesecDriver in metadata.drv)
            {
                Driver driver = new Driver(firesecDriver);
                if (driver.IsIgnore == false)
                {
                    Drivers.Add(driver);
                }
            }
        }
    }
}
