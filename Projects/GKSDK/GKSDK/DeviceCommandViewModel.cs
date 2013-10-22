using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using XFiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKSDK
{
	public class DeviceCommandViewModel : BaseViewModel
	{
		XDevice Device;

		public DeviceCommandViewModel(XDevice device)
		{
			ExecuteCommand = new RelayCommand(OnExecute);
			Device = device;
		}

		public string ConmmandName { get; private set; }

		public RelayCommand ExecuteCommand { get; private set; }
		void OnExecute()
		{
		}
	}
}