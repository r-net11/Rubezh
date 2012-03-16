using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using GroupControllerModule.Models;

namespace GroupControllerModule.Converter
{
    public class ConfigurationConverter
    {
        public void Convert()
        {
            DriversConverter.Convert();
            XManager.DeviceConfiguration = new XDeviceConfiguration();
            ConvertDevices();
            ConvertZones();
        }

        void ConvertDevices()
        {
            byte kauAddress = 1;

            var systemDevice = new XDevice()
            {
                UID = Guid.NewGuid(),
                DriverUID = DriversHelper.System_UID,
                Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System)
            };
            XManager.DeviceConfiguration.Devices.Add(systemDevice);
            XManager.DeviceConfiguration.RootDevice = systemDevice;

            var gCDevice = new XDevice()
            {
                UID = Guid.NewGuid(),
                DriverUID = DriversHelper.GK_UID,
                Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x=>x.DriverType == XDriverType.GK),
                IntAddress = 1
            };
            systemDevice.Children.Add(gCDevice);
            gCDevice.Parent = systemDevice;
            XManager.DeviceConfiguration.Devices.Add(gCDevice);

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                bool isKAU = false;
                var driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);

                if (driver == null)
                {
                    switch (device.Driver.DriverType)
                    {
                        case FiresecAPI.Models.DriverType.BUNS:
                        case FiresecAPI.Models.DriverType.BUNS_2:
                        case FiresecAPI.Models.DriverType.Rubezh_10AM:
                        case FiresecAPI.Models.DriverType.Rubezh_2AM:
                        case FiresecAPI.Models.DriverType.Rubezh_2OP:
                        case FiresecAPI.Models.DriverType.Rubezh_4A:
                        case FiresecAPI.Models.DriverType.USB_BUNS:
                        case FiresecAPI.Models.DriverType.USB_BUNS_2:
                        case FiresecAPI.Models.DriverType.USB_Rubezh_2AM:
                        case FiresecAPI.Models.DriverType.USB_Rubezh_2OP:
                        case FiresecAPI.Models.DriverType.USB_Rubezh_4A:
                            isKAU = true;
                            driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU);
                            break;

                        default:
                            continue;
                    }
                }

                if (isKAU)
                {
                    var kauDevice = gCDevice.AddChild(driver, 0, kauAddress++);
                    kauDevice.UID = device.UID;
                    XManager.DeviceConfiguration.Devices.Add(kauDevice);
                }
                else
                {
                    var xDevice = new XDevice()
                    {
                        UID = device.UID,
                        DriverUID = driver.UID,
                        Driver = driver,
                        ShleifNo = (byte)(device.IntAddress >> 8),
                        IntAddress = (byte)(device.IntAddress & 0xff),
                        Description = device.Description
                    };
                    XManager.DeviceConfiguration.Devices.Add(xDevice);
                }
            }

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                var xDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.UID);
                if (xDevice != null)
                {
                    foreach (var child in device.Children)
                    {
                        var xChildDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == child.UID);
                        xDevice.Children.Add(xChildDevice);
                        xChildDevice.Parent = xDevice;
                    }
                }
            }
        }

        void ConvertZones()
        {
            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                var xZone = new XZone()
                {
                    No = zone.No,
                    Name = zone.Name,
                    Description = zone.Description,
                    DetectorCount = zone.DetectorCount,
                };
                XManager.DeviceConfiguration.Zones.Add(xZone);
            }
        }
    }
}