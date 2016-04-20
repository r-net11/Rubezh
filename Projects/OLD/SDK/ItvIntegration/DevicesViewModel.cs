using System.Collections.ObjectModel;
using FiresecClient.Itv;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ItvIntegration
{
	public class DevicesViewModel : BaseViewModel
	{
		public DevicesViewModel()
		{
            ResetAllCommand = new RelayCommand(OnResetAll);

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in ItvManager.DeviceConfiguration.Devices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; set; }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
				OnPropertyChanged("StateType");
			}
		}

        public RelayCommand ResetAllCommand { get; private set; }
        void OnResetAll()
        {
            ItvManager.ResetAllStates();
        }
	}
}