using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using GroupControllerModule.Converter;
using GroupControllerModule.Models;

namespace GroupControllerModule.ViewModels
{
    public class ConfigurationConverter
    {
        public void Convert()
        {
            ConvertDrivers();
            XManager.DeviceConfiguration = new XDeviceConfiguration();
            ConvertDevices();
            ConvertZones();
        }

        void ConvertDrivers()
        {
            XManager.DriversConfiguration = new XDriversConfiguration();

            foreach (var driver in FiresecManager.Drivers)
            {
                var driverItem = DriversHelper.Drivers.FirstOrDefault(x => x.OldDriverType == driver.DriverType);
                if (driverItem == null)
                    continue;

                var xDriver = new XDriver()
                {
                    DriverType = driverItem.XDriverType,
                    UID = driver.UID,
                    OldDriverUID = driver.UID,
                    Name = driver.Name,
                    ShortName = driver.ShortName,
                    ImageSource = driver.ImageSource,
                    HasImage = driver.HasImage,
                    CanEditAddress = driver.CanEditAddress,
                    IsChildAddressReservedRange = driver.IsChildAddressReservedRange,
                    IsAutoCreate = driver.IsAutoCreate,
                    AutoChild = driver.AutoChild,
                    AutoChildCount = driver.AutoChildCount,
                    MinAutoCreateAddress = driver.MinAutoCreateAddress,
                    MaxAutoCreateAddress = driver.MaxAutoCreateAddress,
                    UseParentAddressSystem = driver.UseParentAddressSystem,
                    IsRangeEnabled = driver.IsRangeEnabled,
                    MinAddress = driver.MinAddress,
                    MaxAddress = driver.MaxAddress,
                    ChildAddressReserveRangeCount = driver.ChildAddressReserveRangeCount
                };

                xDriver.Children = new List<Guid>();
                foreach (var childDriver in driver.AvaliableChildren)
                {
                    xDriver.Children.Add(childDriver);
                }

                xDriver.AutoCreateChildren = new List<Guid>();
                foreach (var childDriver in driver.AutoCreateChildren)
                {
                    xDriver.AutoCreateChildren.Add(childDriver);
                }

                XManager.DriversConfiguration.Drivers.Add(xDriver);
            }

            XManager.DriversConfiguration.Drivers.Add(GroupControllerHelper.Create());
            XManager.DriversConfiguration.Drivers.Add(AddressControllerHelper.Create());
        }

        void ConvertDevices()
        {
            var xRootDevice = new XDevice()
            {
                UID = Guid.NewGuid(),
                DriverUID = DriversHelper.GroupControllerUID,
                Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x=>x.DriverType == XDriverType.GroupController),
                Address = "",
                Description = "Описание для группового контроллера"
            };
            XManager.DeviceConfiguration.Devices.Add(xRootDevice);
            XManager.DeviceConfiguration.RootDevice = xRootDevice;

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                bool isPanel = false;
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
                            isPanel = true;
                            driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.AddressController);
                            break;

                        default:
                            continue;
                    }
                }

                var xDevice = new XDevice()
                {
                    UID = device.UID,
                    DriverUID = driver.UID,
                    Driver = driver,
                    Address = device.PresentationAddress,
                    Description = device.Description
                };
                XManager.DeviceConfiguration.Devices.Add(xDevice);

                if (isPanel)
                {
                    xRootDevice.Children.Add(xDevice);
                }
            }

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                var xDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.UID);
                if (xDevice != null)
                {
                    xDevice.Children = new List<XDevice>();
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