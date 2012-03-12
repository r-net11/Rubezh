using Infrastructure.Common;
using GroupControllerModule.Models;
using FiresecClient;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using GroupControllerModule.Converter;

namespace GroupControllerModule.ViewModels
{
    public class ConfigurationConverter
    {
        public static GCDriversConfiguration GCDriversConfiguration;
        public static GCDeviceConfiguration GCDeviceConfiguration;

        public void Convert()
        {
            ConvertDrivers();
            ConvertDevices();
        }

        void ConvertDrivers()
        {
            GCDriversConfiguration = new GCDriversConfiguration();

            foreach (var driver in FiresecManager.Drivers)
            {
                var driverItem = DriversHelper.Drivers.FirstOrDefault(x => x.OldDriverType == driver.DriverType);
                if (driverItem == null)
                    continue;

                var gCDriver = new GCDriver()
                {
                    DriverType = driverItem.GCDriverType,
                    UID = driver.UID,
                    OldDriverUID = driver.UID,
                    Name = driver.Name,
                    ShortName = driver.ShortName,
                    ImageSource = driver.ImageSource,
                    HasImage = driver.HasImage,
                    CanEditAddress = driver.CanEditAddress,
                    IsChildAddressReservedRange = driver.IsChildAddressReservedRange
                };

                gCDriver.Children = new List<Guid>();
                foreach (var childDriver in driver.AvaliableChildren)
                {
                    gCDriver.Children.Add(childDriver);
                }

                gCDriver.AutoCreateChildren = new List<Guid>();
                foreach (var childDriver in driver.AutoCreateChildren)
                {
                    gCDriver.AutoCreateChildren.Add(childDriver);
                }

                GCDriversConfiguration.Drivers.Add(gCDriver);
            }

            GCDriversConfiguration.Drivers.Add(GroupControllerHelper.Create());
            GCDriversConfiguration.Drivers.Add(AddressControllerHelper.Create());
        }

        void ConvertDevices()
        {
            GCDeviceConfiguration = new GCDeviceConfiguration();

            var gCRootDevice = new GCDevice()
            {
                UID = Guid.NewGuid(),
                DriverUID = DriversHelper.GroupControllerUID,
                Driver = GCDriversConfiguration.Drivers.FirstOrDefault(x=>x.DriverType == GCDriverType.GroupController),
                Address = "",
                Description = "Описание для группового контроллера"
            };
            GCDeviceConfiguration.Devices.Add(gCRootDevice);
            GCDeviceConfiguration.RootDevice = gCRootDevice;

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                bool isPanel = false;
                var driver = GCDriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);

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
                            driver = GCDriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == GCDriverType.AddressController);
                            break;

                        default:
                            continue;
                    }
                }

                var gCDevice = new GCDevice()
                {
                    UID = device.UID,
                    DriverUID = driver.UID,
                    Driver = driver,
                    Address = device.PresentationAddress,
                    Description = device.Description
                };
                GCDeviceConfiguration.Devices.Add(gCDevice);

                if (isPanel)
                {
                    gCRootDevice.Children.Add(gCDevice);
                }
            }

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                var gCDevice = GCDeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.UID);
                if (gCDevice != null)
                {
                    gCDevice.Children = new List<GCDevice>();
                    foreach (var child in device.Children)
                    {
                        var gCChildDevice = GCDeviceConfiguration.Devices.FirstOrDefault(x => x.UID == child.UID);
                        gCDevice.Children.Add(gCChildDevice);
                        gCChildDevice.Parent = gCDevice;
                    }
                }
            }
        }
    }
}