using System.Collections.Generic;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public static class NewDeviceHelper
    {
        public static List<int> GetAvaliableAddresses(Driver driver, Device device)
        {
            List<int> avaliableAddresses = new List<int>();

            if (device.Driver.IsChildAddressReservedRange)
            {
                int range = device.Driver.ChildAddressReserveRangeCount;
                for (int i = device.IntAddress + 1; i < device.IntAddress + range; ++i)
                {
                    if (i % 256 == 0)
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
                for (int i = driver.MinAddress; i <= driver.MaxAddress; ++i)
                {
                    avaliableAddresses.Add(i);
                }
            }
            else
            {
                int shleifCount = GetShleifCount(driver, device);

                List<int> reservedAddresses = GetReservedAddresses(driver, device);

                for (int i = 257; i <= shleifCount * 256 + 255; ++i)
                {
                    if (reservedAddresses.Contains(i))
                        continue;

                    if (i % 256 == 0)
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
            List<int> reservedAddresses = new List<int>();
            foreach (var childDevice in device.Children)
            {
                if (childDevice.Driver.IsChildAddressReservedRange)
                {
                    int range = childDevice.Driver.ChildAddressReserveRangeCount;
                    for (int i = childDevice.IntAddress + 1; i < childDevice.IntAddress + range; ++i)
                    {
                        if (i % 256 == 0)
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
    }
}