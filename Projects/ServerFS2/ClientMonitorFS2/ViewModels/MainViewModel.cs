using System.Collections.ObjectModel;
using ClientFS2;
using ClientFS2.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ClientMonitorFS2.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			GetDevicesCommand = new RelayCommand(OnGetDevices);
		}

		public ObservableCollection<DeviceViewModel> DevicesViewModel { get; private set; }

		private DeviceViewModel _selectedDeviceViewModel;

		public DeviceViewModel SelectedDeviceViewModel
		{
			get { return _selectedDeviceViewModel; }
			set
			{
				_selectedDeviceViewModel = value;
				OnPropertyChanged("SelectedDeviceViewModel");
			}
		}

		public RelayCommand GetDevicesCommand { get; private set; }

		private void OnGetDevices()
		{
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				DevicesViewModel.Add(new DeviceViewModel(device));
			}
			OnPropertyChanged("DevicesViewModel");
			//ShowJournal(ServerHelper.GetJournalItems(SelectedDeviceViewModel.Device));
		}

		public RelayCommand ReadJournalCommand { get; private set; }

		private void OnReadJournal()
		{
			//ShowJournal(ServerHelper.GetJournalItems(SelectedDeviceViewModel.Device));
		}
	}
}