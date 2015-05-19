using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class DeviceViewModel : BaseViewModel
	{
		public DeviceViewModel(AlsDevice alsDevice)
		{
			AlsDevice = alsDevice;
		}

		public AlsDevice AlsDevice { get; private set; }
	}
}