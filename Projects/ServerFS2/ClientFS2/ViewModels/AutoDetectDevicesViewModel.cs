using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using System.Collections.ObjectModel;

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