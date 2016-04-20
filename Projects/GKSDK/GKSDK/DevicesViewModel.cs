using System.Collections.ObjectModel;
using System.Linq;
using RubezhClient;
using Infrastructure.Common.Windows;
using RubezhAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;

namespace GKSDK
{
	public class DevicesViewModel : BaseViewModel
	{
		public DevicesViewModel()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in GKManager.Devices.Where(x => x.IsRealDevice))
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
                OnPropertyChanged(()=>SelectedDevice);
			}
		}
	}
}