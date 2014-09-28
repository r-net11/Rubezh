using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;

namespace AutomationModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public XDevice Device { get; private set; }
		public SKDDevice SKDDevice { get; private set; }
		
		public DeviceViewModel(XDevice device)
		{
			Device = device;
		}

		public DeviceViewModel(SKDDevice sKDDevice)
		{
			SKDDevice = sKDDevice;
		}
	}
}