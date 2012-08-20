using System;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public static class NewDeviceHelper
    {
        public static byte GetMinAddress(XDriver xDriver, XDevice parentXDevice)
        {
            XDevice parentAddressSystemDevice = parentXDevice;
            if (xDriver.UseParentAddressSystem)
            {
                if (xDriver.DriverType == XDriverType.MPT)
                {
                    while (parentAddressSystemDevice.Driver.UseParentAddressSystem)
                    {
                        parentAddressSystemDevice = parentAddressSystemDevice.Parent;
                    }
                }
                else
                {
                    parentAddressSystemDevice = parentAddressSystemDevice.Parent;
                }
            }

            byte maxAddress = 0;

            if (xDriver.IsRangeEnabled)
            {
                maxAddress = xDriver.MinAddress;
            }
            else
            {
                maxAddress = 1;

                if (parentXDevice.Driver.IsChildAddressReservedRange)
                {
                    maxAddress = parentXDevice.IntAddress;
                }
            }

            foreach (var child in parentAddressSystemDevice.Children)
            {
                if (child.Driver.IsAutoCreate)
                    continue;

                if (xDriver.IsRangeEnabled)
                {
                    if ((child.IntAddress < xDriver.MinAddress) && (child.IntAddress > xDriver.MaxAddress))
                        continue;
                }

                if (parentXDevice.Driver.IsChildAddressReservedRange)
                {
                    int reservedCount = parentXDevice.GetReservedCount();

                    if ((child.IntAddress < parentXDevice.IntAddress) && (child.IntAddress > parentXDevice.IntAddress + reservedCount - 1))
                        continue;
                }

                if (child.IntAddress > maxAddress)
                    maxAddress = child.IntAddress;
            }

            if (xDriver.IsRangeEnabled)
            {
                if (parentXDevice.Children.Count > 0)
                    if (maxAddress + 1 <= xDriver.MaxAddress)
                        maxAddress = (byte)(maxAddress + 1);
            }
            else
            {
                if (parentXDevice.Driver.IsChildAddressReservedRange)
                {
                    int reservedCount = xDriver.ChildAddressReserveRangeCount;

                    if (parentXDevice.Children.Count > 0)
                        if (maxAddress + 1 <= parentXDevice.IntAddress + reservedCount - 1)
                            maxAddress = (byte)(maxAddress + 1);
                }
                else
                {
                    if (parentXDevice.Children.Where(x => x.Driver.IsAutoCreate == false).ToList().Count > 0)
                        if (((maxAddress + 1) % 256) != 0)
                            maxAddress = (byte)(maxAddress + 1);
                }
            }

            return maxAddress;
        }

        public static void AddDevice(XDevice xDevice, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(xDevice, parentDeviceViewModel.Source)
            {
                Parent = parentDeviceViewModel
            };
            parentDeviceViewModel.Children.Add(deviceViewModel);

            foreach (var childDevice in xDevice.Children)
            {
                AddDevice(childDevice, deviceViewModel);
            }

            if (xDevice.Driver.IsGroupDevice)
            {
                var driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == xDevice.Driver.AutoChild);

                for (byte i = 0; i < xDevice.Driver.AutoChildCount; i++)
                {
                    var autoDevice = XManager.AddChild(xDevice, driver, xDevice.ShleifNo, (byte)(xDevice.IntAddress + i));
                    AddDevice(autoDevice, deviceViewModel);
                }
            }
        }
    }
}