using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace ClientFS2.ViewModels
{
	public class AutoDetectedDevicesViewModel : DialogViewModel
	{
		public AutoDetectedDevicesViewModel(List<Device> devices)
		{
            Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }
	}
}