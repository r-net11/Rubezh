using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Converter
{
    public class ConfigurationConverter2
    {
        XDevice gkDevice;

        public void Convert()
        {
            AddRootDevices();

            int shleifPairNo = 0;
            byte kauAddress = 1;
            XDevice curentKauDevice = null;

            foreach (var panelDevice in GetPanels())
            {
                if (shleifPairNo == 0)
                {
                    curentKauDevice = XManager.AddChild(gkDevice, XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU), 0, kauAddress);
                    curentKauDevice.UID = panelDevice.UID;
                    XManager.DeviceConfiguration.Devices.Add(curentKauDevice);
                    kauAddress++;
                    shleifPairNo++;
                }
                else
                {
                    shleifPairNo++;
                    if (shleifPairNo == 4)
                        shleifPairNo = 0;
                }

                foreach (var childDevice in panelDevice.Children)
                {
                    var driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == childDevice.DriverUID);
                    if (driver == null)
                    {
                        continue;
                    }
                    var xDevice = new XDevice()
                    {
                        UID = childDevice.UID,
                        DriverUID = driver.UID,
                        Driver = driver,
                        ShleifNo = (byte)((childDevice.IntAddress >> 8) * (shleifPairNo)),
                        IntAddress = (byte)(childDevice.IntAddress & 0xff),
                        Description = childDevice.Description
                    };
                    XManager.DeviceConfiguration.Devices.Add(xDevice);
                    curentKauDevice.Children.Add(xDevice);
                    xDevice.Parent = curentKauDevice;
                }
            }
        }

        public void AddRootDevices()
        {
            XManager.DeviceConfiguration = new XDeviceConfiguration();

            var systemDevice = new XDevice()
            {
                UID = Guid.NewGuid(),
                DriverUID = GKDriversHelper.System_UID,
                Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System)
            };
            XManager.DeviceConfiguration.Devices.Add(systemDevice);
            XManager.DeviceConfiguration.RootDevice = systemDevice;

            var gkDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.GK);
            gkDevice = XManager.AddChild(systemDevice, gkDriver, 0, 1);
            gkDevice.UID = Guid.NewGuid();
            XManager.DeviceConfiguration.Devices.Add(gkDevice);
        }

        IEnumerable<Device> GetPanels()
        {
            foreach (var device in FiresecManager.Devices)
            {
                if (device.Driver.DeviceClassName == "ППКП")
                    yield return device;
            }
        }
    }
}