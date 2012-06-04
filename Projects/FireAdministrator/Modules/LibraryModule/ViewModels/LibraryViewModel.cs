using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace LibraryModule.ViewModels
{
	public class LibraryViewModel : ViewPartViewModel
	{
		public LibraryViewModel()
		{
			AddDeviceCommand = new RelayCommand(OnAddDevice);
			RemoveDeviceCommand = new RelayCommand(OnRemoveDevice, CanRemoveDevice);
		}

		public void Initialize()
		{
			// Чтобы протестить случай, когда для какого-то устройства из библиотеки нету драйвера.
			//FiresecManager.LibraryConfiguration.Devices[0].DriverId = System.Guid.NewGuid();
			DeviceViewModels = new ObservableCollection<DeviceViewModel>(
				FiresecManager.LibraryConfiguration.Devices.Select(device => new DeviceViewModel(device))
			);

			if (DeviceViewModels.Count > 0)
				SelectedDeviceViewModel = DeviceViewModels[0];
		}

		ObservableCollection<DeviceViewModel> _deviceViewModels;
		public ObservableCollection<DeviceViewModel> DeviceViewModels
		{
			get { return _deviceViewModels; }
			set
			{
				_deviceViewModels = value;
				OnPropertyChanged("DeviceViewModels");
			}
		}

		DeviceViewModel _selectedDeviceViewModel;
		public DeviceViewModel SelectedDeviceViewModel
		{
			get { return _selectedDeviceViewModel; }
			set
			{
				_selectedDeviceViewModel = value;
				OnPropertyChanged("SelectedDeviceViewModel");
			}
		}

		public RelayCommand AddDeviceCommand { get; private set; }
		void OnAddDevice()
		{
			var addDeviceViewModel = new DeviceDetailsViewModel();
			if (DialogService.ShowModalWindow(addDeviceViewModel))
			{
				FiresecManager.LibraryConfiguration.Devices.Add(addDeviceViewModel.SelectedItem.LibraryDevice);
				DeviceViewModels.Add(addDeviceViewModel.SelectedItem);

				ServiceFactory.SaveService.LibraryChanged = true;
			}
		}

		public RelayCommand RemoveDeviceCommand { get; private set; }
		void OnRemoveDevice()
		{
			var result = MessageBoxService.ShowQuestion("Вы уверены что хотите удалить выбранное устройство?");

			if (result == MessageBoxResult.Yes)
			{
				FiresecManager.LibraryConfiguration.Devices.Remove(SelectedDeviceViewModel.LibraryDevice);
				DeviceViewModels.Remove(SelectedDeviceViewModel);

				ServiceFactory.SaveService.LibraryChanged = true;
			}
		}

		bool CanRemoveDevice()
		{
			return SelectedDeviceViewModel != null;
		}
	}
}