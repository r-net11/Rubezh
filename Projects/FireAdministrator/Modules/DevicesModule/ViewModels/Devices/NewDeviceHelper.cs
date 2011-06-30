using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public static class NewDeviceHelper
    {
        public static List<int> GetAvaliableAddresses(Driver driver, Device device)
        {
            List<int> avaliableAddresses = new List<int>();

            if (device.Driver.IsChildAddressRange)
            {
                int range = device.Driver.ChildAddressRange;
                for (int i = device.IntAddress; i < device.IntAddress + range; i++)
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
                for (int i = driver.MinAddress; i <= driver.MaxAddress; i++)
                {
                    avaliableAddresses.Add(i);
                }
            }
            else
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

                for (int i = 257; i <= shleifCount * 256 + 255; i++)
                {
                    if (i % 256 == 0)
                        continue;

                    avaliableAddresses.Add(i);
                }
            }

            return avaliableAddresses;
        }
    }
}
