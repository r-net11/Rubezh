using System.Collections.ObjectModel;
using System.Linq;
using Localization.Strazh.ViewModels;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public NewDeviceViewModel(DeviceViewModel parentDeviceViewModel)
		{
			Title = CommonViewModels.NewDevice;
			ParentDevice = parentDeviceViewModel.Device;
			Drivers = new ObservableCollection<SKDDriver>();

			foreach (var driver in SKDManager.Drivers)
			{
				if (ParentDevice.Driver.Children.Contains(driver.DriverType))
					Drivers.Add(driver);
			}

			var driverType = parentDeviceViewModel.Driver.DriverType;
			var parentShleif = ParentDevice;

			SelectedDriver = Drivers.FirstOrDefault();
		}

		SKDDevice ParentDevice;
		public SKDDevice AddedDevice { get; protected set; }
		public ObservableCollection<SKDDriver> Drivers { get; protected set; }

		SKDDriver _selectedDriver;
		public SKDDriver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged(() => SelectedDriver);
				Name = value.ShortName;
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		protected override bool CanSave()
		{
			return SelectedDriver != null && !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			AddedDevice = new SKDDevice()
			{
				Driver = SelectedDriver,
				DriverUID = SelectedDriver.UID,
				Name = Name,
				Parent = ParentDevice
			};

			ParentDevice.Children.Add(AddedDevice);

			foreach (var autocreationItem in SelectedDriver.AutocreationItems)
			{
				var childDriver = SKDManager.Drivers.FirstOrDefault(x => x.DriverType == autocreationItem.DriverType);

				for (int i = 0; i < autocreationItem.Count; i++)
				{
					var childDevice = new SKDDevice()
					{
						Driver = childDriver,
						DriverUID = childDriver.UID,
						IntAddress = i,
						Name = childDriver.Name + " " + (i + 1).ToString(),
						Parent = AddedDevice
					};
					AddedDevice.Children.Add(childDevice);
				}
			}

			SKDManager.SKDConfiguration.Update();
			return true;
		}
	}
}