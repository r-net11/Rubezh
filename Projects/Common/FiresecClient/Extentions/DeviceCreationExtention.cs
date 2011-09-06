using System.Linq;
using FiresecAPI.Models;

namespace FiresecClient
{
    public static class DeviceCreationExtention
    {
        public static Device AddChild(this Device parentDevice, Driver newDriver, int newAddress)
        {
            var device = new Device();
            device.DriverUID = newDriver.UID;
            device.Driver = newDriver;
            device.IntAddress = newAddress;
            parentDevice.Children.Add(device);
            device.Parent = parentDevice;
            AddAutoCreateChildren(device);

            return device;
        }

        static void AddAutoCreateChildren(Device device)
        {
            foreach (var autoCreateDriverId in device.Driver.AutoCreateChildren)
            {
                var autoCreateDriver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == autoCreateDriverId);

                for (int i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; ++i)
                {
                    var childDevice = new Device();
                    childDevice.DriverUID = autoCreateDriver.UID;
                    childDevice.Driver = autoCreateDriver;
                    childDevice.IntAddress = i;
                    device.Children.Add(childDevice);

                    AddAutoCreateChildren(childDevice);
                }
            }
        }
    }
}