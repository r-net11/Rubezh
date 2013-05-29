using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
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
			ObjectCommandSendHelper.ExecuteDeviceCommand(Device, StateType);
			//var code = 0x80 + (int)StateType;
			//ObjectCommandSendHelper.SendControlCommand(Device, (byte)code);
			//JournaActionlHelper.Add("Команда оператора", StateType.ToDescription(), XStateClass.Info, Device);
		}
	}
}