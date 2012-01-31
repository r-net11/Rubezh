using System.Linq;
using FiresecAPI.Models;

namespace FiresecClient
{
    public static class DeviceCreationExtention
    {
        public static Device AddChild(this Device parentDevice, Driver newDriver, int newAddress)
        {
            var device = new Device()
            {
                DriverUID = newDriver.UID,
                Driver = newDriver,
                IntAddress = newAddress,
                Parent = parentDevice
            };
            parentDevice.Children.Add(device);
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
                    device.AddChild(autoCreateDriver, i);
                }
            }
        }

        public static void SynchronizeChildern(this Device device)
        {
            for (int i = device.Children.Count(); i > 0; i--)
            {
                var childDevice = device.Children[i - 1];

                if (device.Driver.AvaliableChildren.Contains(childDevice.Driver.UID) == false)
                {
                    device.Children.RemoveAt(i - 1);
                }
            }

            foreach (var autoCreateChildUID in device.Driver.AutoCreateChildren)
            {
                var autoCreateDriver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == autoCreateChildUID);

                for (int i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; ++i)
                {
                    var newDevice = new Device()
                    {
                        DriverUID = autoCreateDriver.UID,
                        Driver = autoCreateDriver,
                        IntAddress = i
                    };
                    if (device.Children.Any(x => x.Driver.DriverType == newDevice.Driver.DriverType && x.IntAddress == newDevice.IntAddress) == false)
                    {
                        device.Children.Add(newDevice);
                        newDevice.Parent = device;
                    }
                }
            }
        }
    }
}