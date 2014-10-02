using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DeviceExecutableCommandViewModel : BaseViewModel
	{
		public GKStateBit StateBit { get; private set; }
		public GKDevice Device { get; private set; }
		public string Name { get; private set; }

		public DeviceExecutableCommandViewModel(GKDevice device, GKStateBit stateType)
		{
			ExecuteControlCommand = new RelayCommand(OnExecuteControl);
			Device = device;
			StateBit = stateType;
			Name = ((GKStateBit)stateType).ToDescription();
			if (Device.DriverType == GKDriverType.Valve)
			{
				switch (stateType)
				{
					case GKStateBit.TurnOn_InManual:
						Name = "Открыть";
						break;
					case GKStateBit.TurnOnNow_InManual:
						Name = "Открыть немедленно";
						break;
					case GKStateBit.TurnOff_InManual:
						Name = "Закрыть";
						break;
					case GKStateBit.Stop_InManual:
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