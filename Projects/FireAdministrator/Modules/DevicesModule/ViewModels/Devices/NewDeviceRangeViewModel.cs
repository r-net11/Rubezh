using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using FiresecAPI.Models;
using FiresecAPI;

namespace DevicesModule.ViewModels
{
    public class NewDeviceRangeViewModel : DialogContent
    {
        public NewDeviceRangeViewModel(DeviceViewModel parent)
        {
            Title = "Новоые устройства";
            AddCommand = new RelayCommand(OnAdd, CanAdd);
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
                return from Driver driver in FiresecManager.Drivers
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
                UpdateAddressRange();
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

        void UpdateAddressRange()
        {
            List<int> avaliableAddresses = NewDeviceHelper.GetAvaliableAddresses(SelectedDriver, ParentAddressSystemDevice);

            int maxIndex = -1;
            for (int i = 0; i < avaliableAddresses.Count; i++)
            {
                if (ParentAddressSystemDevice.Children.Any(x => x.IntAddress == avaliableAddresses[i]))
                    maxIndex = i;

                if (ChildAddressSystemDevices.Any(x => x.IntAddress == avaliableAddresses[i]))
                    maxIndex = i;
            }

            int startAddress;
            int endAddress;

            if (maxIndex == -1)
            {
                startAddress = endAddress = avaliableAddresses[0];
                if (avaliableAddresses.Count > 1)
                    endAddress = avaliableAddresses[1];
            }
            else
            {
                startAddress = endAddress = avaliableAddresses[maxIndex];

                if (avaliableAddresses.Count() > maxIndex + 1)
                {
                    startAddress = endAddress = avaliableAddresses[maxIndex + 1];
                }
                if (avaliableAddresses.Count() > maxIndex + 2)
                {
                    endAddress = avaliableAddresses[maxIndex + 2];
                }
            }

            StartAddress = AddressConverter.IntToStringAddress(SelectedDriver, startAddress);
            EndAddress = AddressConverter.IntToStringAddress(SelectedDriver, endAddress);
        }

        void CreateDevices()
        {
            int startAddress = AddressConverter.StringToIntAddress(SelectedDriver, StartAddress);
            int endAddress = AddressConverter.StringToIntAddress(SelectedDriver, EndAddress);

            List<int> avaliableAddresses = NewDeviceHelper.GetAvaliableAddresses(SelectedDriver, ParentAddressSystemDevice);

            if (startAddress < endAddress)
            {
                ;
            }
            if (startAddress < avaliableAddresses[0])
            {
                ;
            }
            if (endAddress > avaliableAddresses[avaliableAddresses.Count() - 1])
            {
                ;
            }

            for (int i = 0; i < avaliableAddresses.Count(); i++)
            {
                int address = avaliableAddresses[i];

                if (ParentAddressSystemDevice.Children.Any(x => x.IntAddress == address))
                {
                    ;
                }

                if (ChildAddressSystemDevices.Any(x => x.IntAddress == address))
                {
                    ;
                }
            }

            for (int i = 0; i < avaliableAddresses.Count(); i++)
            {
                int address = avaliableAddresses[i];
                if ((address >= startAddress) && (address <= endAddress))
                {
                    Device device = _parent.AddChild(SelectedDriver, address);
                    AddDevice(device, _parentDeviceViewModel);

                    if (SelectedDriver.IsChildAddressReservedRange)
                        i += SelectedDriver.ChildAddressReserveRangeCount - 1;
                }
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

        public bool CanAdd(object obj)
        {
            return (SelectedDriver != null);
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (CanAdd(null))
            {
                CreateDevices();
            }

            _parentDeviceViewModel.Update();
            FiresecManager.DeviceConfiguration.Update();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
