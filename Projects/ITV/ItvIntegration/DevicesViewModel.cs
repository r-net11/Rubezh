using System.Collections.ObjectModel;
using FiresecClient;

namespace ItvIntegration
{
	public class DevicesViewModel : BaseViewModel
	{
		public DevicesViewModel()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in ItvManager.DeviceConfiguration.Devices)
			{
				var deviceViewModel = new DeviceViewModel(device.DeviceState);
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