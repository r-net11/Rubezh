using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using System;
using System.Linq;

namespace DevicesModule.ViewModels
{
    public static class NewDeviceHelper
    {
        public static int GetMinAddress(Driver driver, Device parentDevice)
        {
            Device parentAddressSystemDevice = parentDevice;
            if (driver.UseParentAddressSystem)
            {
                parentAddressSystemDevice = parentAddressSystemDevice.Parent;

                while (parentAddressSystemDevice.Driver.UseParentAddressSystem)
                {
                    parentAddressSystemDevice = parentAddressSystemDevice.Parent;
                }
            }

            int maxAddress = 0;

            if (driver.IsRangeEnabled)
            {
                maxAddress = driver.MinAddress;
            }
            else
            {
                if (parentDevice.Driver.ShleifCount > 0)
                    maxAddress = 257;

                if (parentDevice.Driver.IsChildAddressReservedRange)
                {
                    maxAddress = parentDevice.IntAddress;
                }
            }

            foreach (var child in parentAddressSystemDevice.Children)
            {
                if (child.Driver.IsAutoCreate)
                    continue;

                if (driver.IsRangeEnabled)
                {
                    if ((child.IntAddress < driver.MinAddress) && (child.IntAddress > driver.MaxAddress))
                        continue;
                }

                if (parentDevice.Driver.IsChildAddressReservedRange)
                {
                    int reservedCount = parentDevice.GetReservedCount();

                    if ((child.IntAddress < parentDevice.IntAddress) && (child.IntAddress > parentDevice.IntAddress + reservedCount - 1))
                        continue;
                }

                if (child.IntAddress > maxAddress)
                    maxAddress = child.IntAddress;
            }

            if (parentDevice.Driver.DriverType == DriverType.MRK_30)
                maxAddress = parentDevice.IntAddress;

            if (driver.IsRangeEnabled)
            {
                if (parentDevice.Children.Count > 0)
                    if (maxAddress + 1 <= driver.MaxAddress)
                        maxAddress = maxAddress + 1;
            }
            else
            {
                if (parentDevice.Driver.IsChildAddressReservedRange)
                {
                    int reservedCount = driver.ChildAddressReserveRangeCount;
                    if (parentDevice.Driver.DriverType == DriverType.MRK_30)
                    {
                        reservedCount = 30;

                        var reservedCountProperty = parentDevice.Properties.FirstOrDefault(x => x.Name == "MRK30ChildCount");
                        if (reservedCountProperty != null)
                        {
                            reservedCount = int.Parse(reservedCountProperty.Value);
                        }
                    }

                    if (parentDevice.Children.Count > 0)
                        if (maxAddress + 1 <= parentDevice.IntAddress + reservedCount - 1)
                            maxAddress = maxAddress + 1;
                }
                else
                {
                    if (parentDevice.Children.Where(x=>x.Driver.IsAutoCreate == false).ToList().Count > 0)
                        if (((maxAddress + 1) % 256) != 0)
                            maxAddress = maxAddress + 1;
                }
            }

            return maxAddress;
        }

        public static List<int> GetAvaliableAddresses(Driver driver, Device device)
        {
            var avaliableAddresses = new List<int>();

            if (device.Driver.IsChildAddressReservedRange)
            {
                int range = device.Driver.ChildAddressReserveRangeCount;
                for (int i = device.IntAddress + 1; i < device.IntAddress + range; i++)
                {
                    if ((i % 256) == 0)
                    {
                        i = device.IntAddress + range;
                        break;
                    }
                    avaliableAddresses.Add(i);
                }
                return avaliableAddresses;
            }

            if (driver.IsRangeEnabled)
            {
                for (int i = driver.MinAddress; i <= driver.MaxAddress; i++)
                {
                    avaliableAddresses.Add(i);
                }
            }
            else
            {
                int shleifCount = GetShleifCount(driver, device);

                List<int> reservedAddresses = GetReservedAddresses(driver, device);

                for (int i = 257; i <= shleifCount * 256 + 255; i++)
                {
                    if (reservedAddresses.Contains(i))
                        continue;

                    if ((i % 256) == 0)
                        continue;

                    avaliableAddresses.Add(i);
                }
            }

            return avaliableAddresses;
        }

        static int GetShleifCount(Driver driver, Device device)
        {
            string addressMask = device.Driver.AddressMask;
            if (addressMask == null)
                addressMask = driver.AddressMask;

            int shleifCount = 0;
            if (addressMask != null)
            {
                switch (addressMask)
                {
                    case "[8(1)-15(2)];[0(1)-7(255)]":
                        shleifCount = 2;
                        break;

                    case "[8(1)-15(4)];[0(1)-7(255)]":
                        shleifCount = 4;
                        break;

                    case "[8(1)-15(10)];[0(1)-7(255)]":
                        shleifCount = 10;
                        break;

                    default:
                        shleifCount = 0;
                        break;
                }
            }

            return shleifCount;
        }

        static List<int> GetReservedAddresses(Driver driver, Device device)
        {
            var reservedAddresses = new List<int>();
            foreach (var childDevice in device.Children)
            {
                if (childDevice.Driver.IsChildAddressReservedRange)
                {
                    int range = childDevice.Driver.ChildAddressReserveRangeCount;
                    for (int i = childDevice.IntAddress + 1; i < childDevice.IntAddress + range; i++)
                    {
                        if ((i & 0xff) == 0) // i % 256;
                        {
                            i = device.IntAddress + range;
                            break;
                        }
                        reservedAddresses.Add(i);
                    }
                }
            }

            return reservedAddresses;
        }

        public static void AddDevice(Device device, DeviceViewModel parentDeviceViewModel)  
        {
            var deviceViewModel = new DeviceViewModel(device, parentDeviceViewModel.Source)
            {
                Parent = parentDeviceViewModel
            };
            parentDeviceViewModel.Children.Add(deviceViewModel);

            foreach (var childDevice in device.Children)
            {
                AddDevice(childDevice, deviceViewModel);
            }

            if (device.Driver.AutoChild != Guid.Empty)
            {
                var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.Driver.AutoChild);

                for (int i = 0; i < device.Driver.AutoChildCount; i++)
                {
                    var autoDevice = device.AddChild(driver, device.IntAddress + i);
                    AddDevice(autoDevice, deviceViewModel);
                }
            }
        }
    }
}