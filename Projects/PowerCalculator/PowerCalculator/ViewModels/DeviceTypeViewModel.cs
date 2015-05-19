using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class DeviceTypeViewModel : BaseViewModel
	{
		public DeviceTypeViewModel(AlsDeviceType deviceType)
		{
			DeviceType = deviceType;
		}

		public AlsDeviceType DeviceType { get; private set; }
	}
}