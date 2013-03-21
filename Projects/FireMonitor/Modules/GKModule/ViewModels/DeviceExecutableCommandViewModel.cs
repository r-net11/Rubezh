using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common;

namespace GKModule.ViewModels
{
	public class DeviceExecutableCommandViewModel : BaseViewModel
	{
		public XStateType StateType { get; private set; }
		public XDevice Device { get; private set; }

		public DeviceExecutableCommandViewModel(XDevice device, XStateType stateType)
		{
			Device = device;
			StateType = stateType;
			ExecuteControlCommand = new RelayCommand(OnExecuteControl);
		}

		public RelayCommand ExecuteControlCommand { get; private set; }
		void OnExecuteControl()
		{
			SendControlCommand(0x80 + (int)StateType);
		}

		void SendControlCommand(int code)
		{
			if (Device.Driver.IsDeviceOnShleif)
			{
				ObjectCommandSendHelper.SendControlCommand(Device, (byte)code);
			}
		}
	}
}