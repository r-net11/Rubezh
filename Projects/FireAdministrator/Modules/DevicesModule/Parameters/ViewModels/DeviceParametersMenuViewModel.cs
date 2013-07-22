using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceParametersMenuViewModel : BaseViewModel
	{
		public DeviceParametersMenuViewModel(DeviceParametersViewModel context)
		{
			Context = context;
		}

		public DeviceParametersViewModel Context { get; private set; }
	}
}