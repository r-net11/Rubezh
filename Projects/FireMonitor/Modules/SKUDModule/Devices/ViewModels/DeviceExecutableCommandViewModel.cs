using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using XFiresecAPI;
using Infrastructure.Common;
using Infrastructure;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class DeviceExecutableCommandViewModel : BaseViewModel
	{
		public XStateBit StateBit { get; private set; }
		public SKDDevice Device { get; private set; }
		public string Name { get; private set; }

		public DeviceExecutableCommandViewModel(SKDDevice device, XStateBit stateType)
		{
			ExecuteControlCommand = new RelayCommand(OnExecuteControl);
			Device = device;
			StateBit = stateType;
			Name = ((XStateBit)stateType).ToDescription();
		}

		public RelayCommand ExecuteControlCommand { get; private set; }
		void OnExecuteControl()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.SKDExecuteDeviceCommand(Device, StateBit);
			}
		}
	}
}