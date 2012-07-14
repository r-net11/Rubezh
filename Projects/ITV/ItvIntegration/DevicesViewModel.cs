using System.Collections.ObjectModel;
using FiresecClient;

namespace ItvIntegration
{
	public class DevicesViewModel : BaseViewModel
	{
		public DevicesViewModel()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var deviceState in ItvManager.DeviceStates.DeviceStates)
			{
				var deviceViewModel = new DeviceViewModel(deviceState);
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
				OnPropertyChanged("StateType");
			}
		}
	}
}