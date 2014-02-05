using Infrastructure.Common.TreeList;
using XFiresecAPI;
using FiresecClient;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class ZoneDeviceViewModel : TreeNodeViewModel<ZoneDeviceViewModel>
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
		public string PresentationAddress
		{
			get { return Device.Address; }
		}
		public bool IsBold { get; set; }
	}
}