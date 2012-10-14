using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

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
				from XDriver driver in XManager.DriversConfiguration.Drivers
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

        protected override bool CanSave()
        {
            return (SelectedDriver != null);
        }

		protected override bool Save()
		{
            for (int i = 0; i < Count; i++)
            {
                byte address = NewDeviceHelper.GetMinAddress(SelectedDriver, _parent);
                XDevice device = XManager.AddChild(_parent, SelectedDriver, 1, address);
                NewDeviceHelper.AddDevice(device, _parentDeviceViewModel);
            }

            _parentDeviceViewModel.Update();
            XManager.DeviceConfiguration.Update();
			return base.Save();
		}
    }
}