using FiresecAPI.GK;
using Infrastructure.Common.TreeList;

namespace AutomationModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public XDevice Device { get; private set; }

		public DeviceViewModel(XDevice device)
		{
			Device = device;
		}
	}
}