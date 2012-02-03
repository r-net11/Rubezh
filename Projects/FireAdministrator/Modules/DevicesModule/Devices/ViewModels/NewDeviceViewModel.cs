using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    [SaveSizeAttribute]
    public class NewDeviceViewModel : SaveCancelDialogContent
    {
        public NewDeviceViewModel(DeviceViewModel parent)
        {
            Title = "Новое устройство";
            _parentDeviceViewModel = parent;
            _parent = _parentDeviceViewModel.Device;
        }

        DeviceViewModel _parentDeviceViewModel;
        Device _parent;

        public IEnumerable<Driver> Drivers
        {
            get
            {
                return from Driver driver in FiresecManager.Drivers
                       where _parent.Driver.AvaliableChildren.Contains(driver.UID)
                       select driver;
            }
        }

        Driver _selectedDriver;
        public Driver SelectedDriver
        {
            get { return _selectedDriver; }
            set
            {
                _selectedDriver = value;
                OnPropertyChanged("SelectedDriver");
            }
        }

        Device ParentAddressSystemDevice
        {
            get
            {
                Device parentAddressSystemDevice = _parent;
                while (parentAddressSystemDevice.Driver.UseParentAddressSystem)
                {
                    parentAddressSystemDevice = parentAddressSystemDevice.Parent;
                }
                return parentAddressSystemDevice;
            }
        }

        void AddChildAddressSystemDevice(List<Device> childAddressSystemDevices, Device parentDevice)
        {
            foreach (var childDevice in parentDevice.Children)
            {
                if (childDevice.Driver.UseParentAddressSystem)
                {
                    childAddressSystemDevices.Add(childDevice);
                    AddChildAddressSystemDevice(childAddressSystemDevices, childDevice);
                }
            }
        }

        List<Device> ChildAddressSystemDevices
        {
            get
            {
                var childAddressSystemDevices = new List<Device>();
                AddChildAddressSystemDevice(childAddressSystemDevices, ParentAddressSystemDevice);
                return childAddressSystemDevices;
            }
        }

        List<int> AvaliableAddresses
        {
            get { return NewDeviceHelper.GetAvaliableAddresses(SelectedDriver, ParentAddressSystemDevice); }
        }

        int GetNewAddress()
        {
            var avaliableAddresses = NewDeviceHelper.GetAvaliableAddresses(SelectedDriver, ParentAddressSystemDevice);

            int maxIndex = -1;
            for (int i = 0; i < avaliableAddresses.Count; ++i)
            {
                if (ParentAddressSystemDevice.Children.Any(x => x.IntAddress == avaliableAddresses[i]))
                    maxIndex = i;

                if (ChildAddressSystemDevices.Any(x => x.IntAddress == avaliableAddresses[i]))
                    maxIndex = i;
            }

            if (maxIndex == -1)
                return avaliableAddresses[0];

            int address = avaliableAddresses[maxIndex];
            if (avaliableAddresses.Count() > maxIndex + 1)
                address = avaliableAddresses[maxIndex + 1];

            return address;
        }

        void AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device, _parentDeviceViewModel.Source);
            deviceViewModel.Parent = parentDeviceViewModel;
            parentDeviceViewModel.Children.Add(deviceViewModel);

            foreach (var childDevice in device.Children)
            {
                AddDevice(childDevice, deviceViewModel);
            }

            if (device.Driver.AutoChild != Guid.Empty)
            {
                var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.Driver.AutoChild);

                for (int i = 0; i < device.Driver.AutoChildCount; ++i)
                {
                    var autoDevice = device.AddChild(driver, device.IntAddress + i);
                    AddDevice(autoDevice, deviceViewModel);
                }
            }
        }

        protected override bool CanSave()
        {
            return (SelectedDriver != null);
        }

        protected override void Save(ref bool cancel)
        {
            int address = GetNewAddress();
            Device device = _parent.AddChild(SelectedDriver, address);
            AddDevice(device, _parentDeviceViewModel);

            _parentDeviceViewModel.Update();
            FiresecManager.DeviceConfiguration.Update();
        }
    }
}