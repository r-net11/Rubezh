using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace ClientFS2.ViewModels
{
	public class AutoDetectDevicesViewModel : DialogViewModel
	{
		public AutoDetectDevicesViewModel(List<Device> devices)
		{
			foreach (var device in devices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }
	}
}