using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using System;

namespace GKModule.ViewModels
{
    public class NewDeviceViewModel : SaveCancelDialogViewModel
    {
        DeviceViewModel _parentDeviceViewModel;
        XDevice _parent;

        public NewDeviceViewModel(DeviceViewModel parent)
        {
            Title = "Новое устройство";
            _parentDeviceViewModel = parent;
            _parent = _parentDeviceViewModel.Device;

			Drivers = new List<XDriver>(
				from XDriver driver in XManager.XDriversConfiguration.XDrivers
                       where _parent.Driver.Children.Contains(driver.DriverType)
                       select driver);

			SelectedDriver = Drivers.FirstOrDefault();
            Count = 1;
        }

        public List<XDriver> Drivers { get; private set; }

        XDriver _selectedDriver;
        public XDriver SelectedDriver
        {
            get { return _selectedDriver; }
            set
            {
                _selectedDriver = value;
                UpdateAddressRange();
                OnPropertyChanged("SelectedDriver");
            }
        }


        XDevice _startDevice;
        public XDevice StartDevice
        {
            get { return _startDevice; }
            set
            {
                _startDevice = value;
                OnPropertyChanged("StartDevice");
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
                    OnPropertyChanged("StartAddress");
                }
            }
        }

        int _count;
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }

        void UpdateAddressRange()
        {
            int maxAddress = NewDeviceHelper.GetMinAddress(SelectedDriver, _parent);

            StartDevice = new XDevice()
            {
                Driver = SelectedDriver,
                ShleifNo = (byte)(maxAddress / 256 + 1),
                IntAddress = (byte)(maxAddress % 256),
                Parent = _parent
            };
            StartAddress = StartDevice.Address;
        }

        void CreateDevices()
        {
            var step = Math.Max(SelectedDriver.GroupDeviceChildrenCount, (byte)1);
            for (int i = StartDevice.IntAddress; i <= StartDevice.IntAddress + Count * step; i++)
            {
                if (_parent.Children.Any(x => x.IntAddress == i && x.ShleifNo == StartDevice.ShleifNo))
                {
                    MessageBoxService.ShowWarning("В заданном диапазоне уже существуют устройства");
                    return;
                }
            }

            if (_parent.Driver.IsGroupDevice)
            {
                Count = Math.Min(Count, _parent.Driver.GroupDeviceChildrenCount);
            }

            byte shleifNo = StartDevice.ShleifNo;
            for (int i = 0; i < Count; i++)
            {
                var address = StartDevice.IntAddress + i * step;
                if (address + SelectedDriver.GroupDeviceChildrenCount >= 256)
                {
                    return;
                }

                XDevice device = XManager.AddChild(_parent, SelectedDriver, shleifNo, (byte)address);
                NewDeviceHelper.AddDevice(device, _parentDeviceViewModel);
            }
        }

        protected override bool CanSave()
        {
            return (SelectedDriver != null);
        }

		protected override bool Save()
		{
            CreateDevices();
            _parentDeviceViewModel.Update();
            XManager.DeviceConfiguration.Update();
			return base.Save();
		}
    }
}