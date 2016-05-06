using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrastructure.Common.TreeList;

namespace AutomationModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public SKDDevice SKDDevice { get; private set; }

		public DeviceViewModel(SKDDevice sKDDevice)
		{
			SKDDevice = sKDDevice;
		}
	}
}