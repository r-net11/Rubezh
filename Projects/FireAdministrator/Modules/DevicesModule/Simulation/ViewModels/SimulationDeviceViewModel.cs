using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
	public class SimulationDeviceViewModel : BaseViewModel
	{
		public SimulationDeviceViewModel(Device device)
		{
			Device = device;
		}

		public Device Device { get; private set; }
	}
}