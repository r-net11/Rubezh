using FiresecAPI;
using Infrastructure.Common.TreeList;
using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ZoneDeviceViewModel : BaseViewModel
	{
		public SKDDevice Device { get; private set; }

		public ZoneDeviceViewModel(SKDDevice device)
		{
			Device = device;
		}

		public SKDDriver Driver
		{
			get { return Device.Driver; }
		}
		public string Name
		{
			get { return Device.Name; }
		}
		public string ControllerPresentationName
		{
			get { return Device.Parent.Name; }
		}
	}
}