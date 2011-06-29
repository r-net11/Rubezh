using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class NewDeviceRangeViewModel : DialogContent
    {
        public NewDeviceRangeViewModel(DeviceViewModel parent)
        {
            Title = "Новоые устройства";
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
                       where ((_parent.Driver.AvaliableChildren.Contains(driver.Id)) && (driver.HasAddress))
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
                SetNewAddressRange();
                OnPropertyChanged("SelectedDriver");
            }
        }

        string _startAddress;
        public string StartAddress
        {
            get { return _startAddress; }
            set
            {
                _startAddress = value;
                OnPropertyChanged("StartAddress");
            }
        }

        string _endAddress;
        public string EndAddress
        {
            get { return _endAddress; }
            set
            {
                _endAddress = value;
                OnPropertyChanged("EndAddress");
            }
        }

        void SetNewAddressRange()
        {
            List<int> avaliableAddresses = NewDeviceHelper.GetAvaliableAddresses(SelectedDriver, _parent);

            int maxIndex = 0;

            for (int i = 0; i < avaliableAddresses.Count; i++)
            {
                int address = avaliableAddresses[i];
                if (_parent.Children.Any(x => x.IntAddress == address))
                    maxIndex = i;
            }

            int startAddress = avaliableAddresses[maxIndex];
            int endAddress = avaliableAddresses[maxIndex];

            if (avaliableAddresses.Count() > maxIndex + 1)
            {
                startAddress = avaliableAddresses[maxIndex + 1];
                endAddress = avaliableAddresses[maxIndex + 1];
            }
            if (avaliableAddresses.Count() > maxIndex + 2)
            {
                endAddress = avaliableAddresses[maxIndex + 2];
            }

            Device tempDevice = new Device();
            tempDevice.Driver = SelectedDriver;
            tempDevice.IntAddress = startAddress;
            StartAddress = tempDevice.Address;
            tempDevice.IntAddress = endAddress;
            EndAddress = tempDevice.Address;
        }

        void CreateAddressRange()
        {
            Device tempDevice = new Device();
            tempDevice.Driver = SelectedDriver;
            tempDevice.SetAddress(StartAddress);
            int startAddress = tempDevice.IntAddress;
            tempDevice.SetAddress(EndAddress);
            int endAddress = tempDevice.IntAddress;

            if (startAddress < endAddress)
            {
                ;
            }
            if (startAddress < SelectedDriver.MinAddress)
            {
                ;
            }
            if (endAddress < SelectedDriver.MaxAddress)
            {
                ;
            }

            if (_parent.Children.Any(x => ((x.IntAddress >= startAddress) || (x.IntAddress <= endAddress))))
            {
                ;
            }

            for (int i = startAddress; i <= endAddress; i++)
            {
                if ((SelectedDriver.IsDeviceOnShleif) && (i % 256 == 0))
                    continue;

                Device device = _parent.AddChild(SelectedDriver, i);
                AddDevice(device, _parentDeviceViewModel);
            }
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
                CreateAddressRange();
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
