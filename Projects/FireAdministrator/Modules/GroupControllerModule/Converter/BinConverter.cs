using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure;
using XFiresecAPI;

namespace GKModule.Converter
{
    public static class BinConverter
    {
        public static void Convert()
        {
            foreach (var zone in XManager.DeviceConfiguration.Zones)
            {
                zone.KAUDevices = new List<XDevice>();
                foreach (var deviceUID in zone.DeviceUIDs)
                {
                    var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                    var kauDevice = device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);

                    if (zone.KAUDevices.Any(x => x.UID == kauDevice.UID) == false)
                        zone.KAUDevices.Add(kauDevice);
                }
            }

            foreach (var device in XManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.DriverType == XDriverType.KAU)
                {
                    device.InternalKAUNo = 1;
                    short currentNo = 3;

                    foreach (var childDevice in device.Children)
                    {
                        if (childDevice.Driver.DriverType == XDriverType.KAUIndicator)
                            childDevice.InternalKAUNo = 2;
                        else
                            childDevice.InternalKAUNo = currentNo++;
                    }

                    var indicatorDevice = device.Children.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAUIndicator);

                    foreach (var childDevice in device.Children)
                    {
                        if (childDevice.Driver.DriverType != XDriverType.KAUIndicator)
                        {
                        }
                    }

                    foreach (var zone in XManager.DeviceConfiguration.Zones)
                    {
                        if ((zone.KAUDevices.Count == 1) && (zone.KAUDevices[0].UID == device.UID))
                        {
                            zone.InternalKAUNo = currentNo++;
                        }
                    }
                }
            }

            var deviceConverterViewModel = new DeviceConverterViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(deviceConverterViewModel);
        }
    }
}