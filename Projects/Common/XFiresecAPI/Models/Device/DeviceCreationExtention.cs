using System.Linq;

namespace XFiresecAPI
{
    public static class XDeviceCreationExtention
    {
        public static XDevice AddChild(this XDevice parentXDevice, XDriver newXDriver, byte shleifNo, byte intAddress)
        {
            var xDevice = new XDevice()
            {
                DriverUID = newXDriver.UID,
                Driver = newXDriver,
                ShleifNo = shleifNo,
                IntAddress = intAddress,
                Parent = parentXDevice
            };
            parentXDevice.Children.Add(xDevice);
            AddAutoCreateChildren(xDevice);

            return xDevice;
        }

        static void AddAutoCreateChildren(XDevice xDevice)
        {
            foreach (var autoCreateDriverId in xDevice.Driver.AutoCreateChildren)
            {
                var autoCreateDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == autoCreateDriverId);

                for (byte i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; i++)
                {
                    xDevice.AddChild(autoCreateDriver, 0, i);
                }
            }
        }

        public static void SynchronizeChildern(this XDevice xDevice)
        {
            for (int i = xDevice.Children.Count(); i > 0; i--)
            {
                var childDevice = xDevice.Children[i - 1];

                if (xDevice.Driver.Children.Contains(childDevice.Driver.UID) == false)
                {
                    xDevice.Children.RemoveAt(i - 1);
                }
            }

            foreach (var autoCreateChildUID in xDevice.Driver.AutoCreateChildren)
            {
                var autoCreateDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == autoCreateChildUID);

                for (byte i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; i++)
                {
                    var newDevice = new XDevice()
                    {
                        DriverUID = autoCreateDriver.UID,
                        Driver = autoCreateDriver,
                        IntAddress = i
                    };
                    if (xDevice.Children.Any(x => x.Driver.DriverType == newDevice.Driver.DriverType && x.Address == newDevice.Address) == false)
                    {
                        xDevice.Children.Add(newDevice);
                        newDevice.Parent = xDevice;
                    }
                }
            }
        }
    }
}