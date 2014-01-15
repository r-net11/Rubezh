using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using SkudModule.ViewModels;
using FiresecAPI;
using System.Collections.Generic;

namespace SkudModule.ViewModels
{
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public NewDeviceViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Новое устройство";
			ParentDeviceViewModel = deviceViewModel;
			ParentDevice = ParentDeviceViewModel.Device;
			Drivers = new ObservableCollection<SKDDriver>();
			AvailableShleifs = new ObservableCollection<byte>();

			foreach (var driver in SKDManager.Drivers)
			{
				if (ParentDevice.Driver.Children.Contains(driver.DriverType))
					Drivers.Add(driver);
			}

			var driverType = deviceViewModel.Driver.DriverType;
			var parentShleif = ParentDevice;

			SelectedDriver = Drivers.FirstOrDefault();
		}

		protected DeviceViewModel ParentDeviceViewModel;
		protected SKDDevice ParentDevice;
		public DeviceViewModel AddedDevice { get; protected set; }
		public ObservableCollection<SKDDriver> Drivers { get; protected set; }
		public ObservableCollection<byte> AvailableShleifs { get; protected set; }

		SKDDriver _selectedDriver;
		public SKDDriver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged("SelectedDriver");
			}
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				if (_address != value)
				{
					_address = value;
					OnPropertyChanged("Address");
				}
			}
		}

		protected override bool CanSave()
		{
			return (SelectedDriver != null);
		}

		protected override bool Save()
		{
			var device = new SKDDevice()
			{
				Address = Address,
				Driver = SelectedDriver,
				DriverUID = SelectedDriver.UID
			};
			SKDManager.Devices.Add(device);
			AddedDevice = new DeviceViewModel(device);
			ParentDeviceViewModel.Device.Children.Add(device);
			ParentDeviceViewModel.AddChild(AddedDevice);
			ParentDeviceViewModel.Update();
			XManager.DeviceConfiguration.Update();
			return true;
		}
	}
}