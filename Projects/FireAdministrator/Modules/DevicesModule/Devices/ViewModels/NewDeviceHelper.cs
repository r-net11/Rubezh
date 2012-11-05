using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public static class NewDeviceHelper
    {
        public static DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device);
            parentDeviceViewModel.Children.Add(deviceViewModel);

            foreach (var childDevice in device.Children)
            {
                AddDevice(childDevice, deviceViewModel);
            }
            return deviceViewModel;
        }

        static List<DeviceAddress> Addresses;

        public static int GetMinAddress(Driver driver, Device parentDevice)
        {
            if (driver.IsRangeEnabled)
            {
                Addresses = new List<DeviceAddress>();
                for (int i = driver.MinAddress; i <= driver.MaxAddress; i++)
                {
                    Addresses.Add(new DeviceAddress(i));
                }

                foreach (var child in parentDevice.Children)
                {
                    var address = Addresses.FirstOrDefault(x => x.Address == child.IntAddress);
                    if (address != null)
                    {
                        address.IsBuisy = true;
                    }
                }
                return GetBestAddress(driver);
            }

            Device panel = parentDevice.ParentPanel;
            if (parentDevice.Driver.IsPanel)
                panel = parentDevice;

            if (panel != null)
            {
                InitializeAddresses(parentDevice);
                SetBuisyChildAddress(panel);
                return GetBestAddress(driver);
            }
            return 0;
        }

        static void InitializeAddresses(Device device)
        {
            Addresses = new List<DeviceAddress>();

            if (device.Driver.IsChildAddressReservedRange)
            {
                for (int i = 1; i < device.GetReservedCount(); i++)
                {
                    Addresses.Add(new DeviceAddress(device.IntAddress + i));
                }
            }
            else
            {
                var shleifNo = GetMaxShleif(device);
                for (int i = 1; i < 256; i++)
                {
                    Addresses.Add(new DeviceAddress(shleifNo * 256 + i));
                }
            }
        }

        static int GetMaxShleif(Device device)
        {
            if (device.Driver.IsDeviceOnShleif)
                return device.IntAddress / 256;

            int maxShleif = 1;
            foreach (var child in device.Children)
            {
                var shleifNo = child.IntAddress / 256;
                if (shleifNo > maxShleif)
                    maxShleif = shleifNo;
            }
            return maxShleif;
        }

        static void SetBuisyChildAddress(Device device)
        {
            foreach (var child in device.Children)
            {
                var reservedCount = Math.Max(child.GetReservedCount(), 1);
                for (int i = 0; i < reservedCount; i++)
                {
                    var address = Addresses.FirstOrDefault(x => x.Address == child.IntAddress + i);
                    if (address != null)
                    {
                        address.IsBuisy = true;
                    }
                }
                SetBuisyChildAddress(child);
            }
        }

        static int GetBestAddress(Driver driver)
        {
            var bestAddress = Addresses.FirstOrDefault();
            foreach (var address in Addresses)
            {
                if (address.IsBuisy)
                    continue;

                bool isPrevAddressBuisy = true;
                var prevAddress = Addresses.FirstOrDefault(x => x.Address == address.Address-1);
                if (prevAddress != null)
                    isPrevAddressBuisy = prevAddress.IsBuisy;

                if (!isPrevAddressBuisy)
                    continue;

                bool reservedAddressesFree = true;
                for (int i = 0; i < Math.Max(driver.ChildAddressReserveRangeCount, 1); i++)
                {
                    var reservedAddress = Addresses.FirstOrDefault(x => x.Address == address.Address + i);
                    if (reservedAddress != null)
                    {
                        if (reservedAddress.IsBuisy)
                            reservedAddressesFree = false;
                    }
                    else
                        reservedAddressesFree = false;
                }
                if (!reservedAddressesFree)
                    continue;

                if (address.Address > bestAddress.Address)
                    bestAddress = address;
            }
            return bestAddress.Address;
        }

        class DeviceAddress
        {
            public DeviceAddress(int address)
            {
                Address = address;
            }

            public int Address { get; set; }
            public bool IsBuisy { get; set; }
        }
    }
}