using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DeviceInfoViewModel : DialogViewModel
	{
		public SKDDeviceInfo DeviceInfo { get; private set; }

		public DeviceInfoViewModel(SKDDeviceInfo deviceInfo)
		{
			Title = "Информация об устройстве";
			DeviceInfo = deviceInfo;
		}
	}
}