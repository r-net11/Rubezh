using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI;
using XFiresecAPI;

namespace GKImitator.ViewModels
{
	public class SKDViewModel : BaseViewModel
	{
		public SKDViewModel()
		{
			SKDDevices = new ObservableCollection<SKDDeviceViewModel>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.DriverType == SKDDriverType.Controller)
				{
					var skdDeviceViewModel = new SKDDeviceViewModel(device);
					SKDDevices.Add(skdDeviceViewModel);
				}
			}
		}

		public ObservableCollection<SKDDeviceViewModel> SKDDevices { get; private set; }
	}
}