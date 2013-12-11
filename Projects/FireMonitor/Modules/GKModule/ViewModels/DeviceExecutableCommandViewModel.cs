using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using XFiresecAPI;
using Infrastructure.Common;
using Infrastructure;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class DeviceExecutableCommandViewModel : BaseViewModel
	{
		public XStateBit StateBit { get; private set; }
		public XDevice Device { get; private set; }
		public string Name { get; private set; }

		public DeviceExecutableCommandViewModel(XDevice device, XStateBit stateType)
		{
			ExecuteControlCommand = new RelayCommand(OnExecuteControl);
			Device = device;
			StateBit = stateType;
			Name = ((XStateBit)stateType).ToDescription();
			if (Device.DriverType == XDriverType.Valve)
			{
				switch (stateType)
				{
					case XStateBit.TurnOn_InManual:
						Name = "Открыть";
						break;
					case XStateBit.TurnOnNow_InManual:
						Name = "Открыть немедленно";
						break;
					case XStateBit.TurnOff_InManual:
						Name = "Закрыть";
						break;
					case XStateBit.Stop_InManual:
						Name = "Остановить";
						break;
				}
			}
		}

		public RelayCommand ExecuteControlCommand { get; private set; }
		void OnExecuteControl()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKExecuteDeviceCommand(Device, StateBit);
			}
		}
	}
}