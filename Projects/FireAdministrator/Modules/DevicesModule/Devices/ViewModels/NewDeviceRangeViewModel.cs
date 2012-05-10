using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.MessageBox;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class NewDeviceRangeViewModel : SaveCancelDialogContent
    {
        DeviceViewModel _parentDeviceViewModel;
        Device _parent;

        public NewDeviceRangeViewModel(DeviceViewModel parent)
        {
            Title = "Новоые устройства";
            _parentDeviceViewModel = parent;
            _parent = _parentDeviceViewModel.Device;

            Drivers = new List<Driver>(
                from Driver driver in FiresecManager.Drivers
                where ((_parent.Driver.AvaliableChildren.Contains(driver.UID) && driver.HasAddress))
                select driver);

            if (Drivers.Count > 0)
                SelectedDriver = Drivers[0];
        }

        public List<Driver> Drivers{get;private set;}

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

        Device _startDevice;
        public Device StartDevice
        {
            get { return _startDevice; }
            set
            {
                _startDevice = value;
                OnPropertyChanged("StartDevice");
            }
        }

        Device _endDevice;
        public Device EndDevice
        {
            get { return _endDevice; }
            set
            {
                _endDevice = value;
                OnPropertyChanged("EndDevice");
            }
        }

        string _startAddress;
        public string StartAddress
        {
            get { return _startAddress; }
            set
            {
                if (_startAddress != value)
                {
                    _startAddress = value;
                    if (_startAddress.Contains("."))
                    {
                        if (EndAddress != null)
                            EndAddress = _startAddress[0] + EndAddress.Substring(1);
                    }

                    if ((StartAddress != null) && (EndAddress != null))
                    {
                        int startAddress = AddressConverter.StringToIntAddress(SelectedDriver, StartAddress);
                        int endAddress = AddressConverter.StringToIntAddress(SelectedDriver, EndAddress);

                        if (startAddress > endAddress)
                        {
                            EndAddress = StartAddress;
                        }
                    }

                    OnPropertyChanged("StartAddress");
                }
            }
        }

        string _endAddress;
        public string EndAddress
        {
            get { return _endAddress; }
            set
            {
                if (_endAddress != value)
                {
                    _endAddress = value;
                    if (_endAddress.Contains("."))
                    {
                        if (StartAddress != null)
                            StartAddress = _endAddress[0] + StartAddress.Substring(1);
                    }

                    if ((StartAddress != null) && (EndAddress != null))
                    {
                        int startAddress = AddressConverter.StringToIntAddress(SelectedDriver, StartAddress);
                        int endAddress = AddressConverter.StringToIntAddress(SelectedDriver, EndAddress);

                        if (startAddress > endAddress)
                        {
                            StartAddress = EndAddress;
                        }
                    }

                    OnPropertyChanged("EndAddress");
                }
            }
        }

        void UpdateAddressRange()
        {
            int maxAddress = NewDeviceHelper.GetMinAddress(SelectedDriver, _parent);

            StartDevice = new Device()
            {
                Driver = SelectedDriver,
                IntAddress = maxAddress,
                Parent = _parent
            };
            EndDevice = new Device()
            {
                Driver = SelectedDriver,
                IntAddress = maxAddress,
                Parent = _parent
            };
            StartAddress = StartDevice.EditingPresentationAddress;
            EndAddress = EndDevice.EditingPresentationAddress;
        }

        void CreateDevices()
        {
            int startAddress = AddressConverter.StringToIntAddress(SelectedDriver, StartAddress);
            int endAddress = AddressConverter.StringToIntAddress(SelectedDriver, EndAddress);

            for (int i = startAddress; i <= endAddress; i++)
            {
                if (_parent.Children.Any(x => x.IntAddress == i))
                {
                    MessageBoxService.ShowWarning("В заданном диапазоне уже сущеспвуют устройства");
                    return;
                }
            }

            for (int i = startAddress; i <= endAddress; i++)
            {
                if (SelectedDriver.IsChildAddressReservedRange)
                {
                    if (i + SelectedDriver.ChildAddressReserveRangeCount > endAddress)
                        break;
                }

                Device device = _parent.AddChild(SelectedDriver, i);
                NewDeviceHelper.AddDevice(device, _parentDeviceViewModel);

                if (SelectedDriver.IsChildAddressReservedRange)
                {
                    int reservedCount = SelectedDriver.ChildAddressReserveRangeCount;
                    if (SelectedDriver.DriverType == DriverType.MRK_30)
                        reservedCount = 30;
                    i += reservedCount - 1;
                }
            }
        }

        protected override bool CanSave()
        {
            return (SelectedDriver != null);
        }

        protected override void Save(ref bool cancel)
        {
            CreateDevices();

            _parentDeviceViewModel.Update();
            FiresecManager.DeviceConfiguration.Update();
        }
    }
}