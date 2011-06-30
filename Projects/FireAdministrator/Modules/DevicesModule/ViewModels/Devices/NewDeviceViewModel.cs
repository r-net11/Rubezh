using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Windows;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class NewDeviceViewModel : DialogContent
    {
        public NewDeviceViewModel(DeviceViewModel parent)
        {
            //ViewModels.Devices.DriversView driversView = new ViewModels.Devices.DriversView();
            //driversView.ShowDialog();

            Title = "Новое устройство";
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
            _parentDeviceViewModel = parent;
            _parent = _parentDeviceViewModel.Device;
        }

        DeviceViewModel _parentDeviceViewModel;
        Device _parent;

        public IEnumerable<Driver> Drivers
        {
            get
            {
                return from Driver driver in FiresecManager.Configuration.Drivers
                       where _parent.Driver.AvaliableChildren.Contains(driver.Id)
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
                List<Device> childAddressSystemDevices = new List<Device>();
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
            List<int> avaliableAddresses = NewDeviceHelper.GetAvaliableAddresses(SelectedDriver, ParentAddressSystemDevice);

            int maxIndex = 0;
            for (int i = 0; i < avaliableAddresses.Count; i++)
            {
                if (ParentAddressSystemDevice.Children.Any(x => x.IntAddress == avaliableAddresses[i]))
                    maxIndex = i;

                if (ChildAddressSystemDevices.Any(x => x.IntAddress == avaliableAddresses[i]))
                    maxIndex = i;
            }

            int address = avaliableAddresses[maxIndex];
            if (avaliableAddresses.Count() > maxIndex + 1)
            {
                address = avaliableAddresses[maxIndex + 1];
            }

            return address;
        }

        void AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            DeviceViewModel deviceViewModel = new DeviceViewModel();
            deviceViewModel.Initialize(device, _parentDeviceViewModel.Source);
            deviceViewModel.Parent = parentDeviceViewModel;
            parentDeviceViewModel.Children.Add(deviceViewModel);

            foreach (var childDevice in device.Children)
            {
                AddDevice(childDevice, deviceViewModel);
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (SelectedDriver != null)
            {
                int address = GetNewAddress();
                Device device = _parent.AddChild(SelectedDriver, address);
                AddDevice(device, _parentDeviceViewModel);
            }

            _parentDeviceViewModel.Update();
            FiresecManager.Configuration.Update();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
