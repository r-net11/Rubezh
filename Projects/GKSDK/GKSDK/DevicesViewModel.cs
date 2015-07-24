using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Common;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace GKSDK
{
	public class DevicesViewModel : BaseViewModel
	{
		public DevicesViewModel()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in GKManager.Devices)
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
	}
}