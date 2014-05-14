using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.TreeList;

namespace GKModule.ViewModels
{
	public class ZoneDeviceViewModel : TreeNodeViewModel<ZoneDeviceViewModel>
	{
		public XDevice Device { get; private set; }

		public ZoneDeviceViewModel(XDevice device)
		{
			Device = device;
		}

		public XDriver Driver
		{
			get { return Device.Driver; }
		}
		public string PresentationAddress
		{
			get { return Device.DottedPresentationAddress; }
		}
		public string PresentationZone
		{
			get
			{
				if(Device.Driver.HasLogic)
					return XManager.GetPresentationZone(Device);
				return null;
			}
		}
		public string Description
		{
			get { return Device.Description; }
		}
		public bool IsBold { get; set; }
	}
}