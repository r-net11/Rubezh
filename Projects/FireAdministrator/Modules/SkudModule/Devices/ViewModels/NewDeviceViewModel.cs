using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace SKDModule.ViewModels
{
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public NewDeviceViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Новое устройство";
			ParentDeviceViewModel = deviceViewModel;
			ParentDevice = ParentDeviceViewModel.Device;
			Drivers = new ObservableCollection<SKDDriver>();

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

		SKDDriver _selectedDriver;
		public SKDDriver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged("SelectedDriver");
				Name = value.ShortName;
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

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		protected override bool CanSave()
		{
			return SelectedDriver != null && !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			var device = new SKDDevice()
			{
				Driver = SelectedDriver,
				DriverUID = SelectedDriver.UID,
				Address = Address,
				Name = Name,
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