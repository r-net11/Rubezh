using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.TreeList;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class ZoneDeviceViewModel : TreeNodeViewModel<ZoneDeviceViewModel>
	{
		public GKDevice Device { get; private set; }

		public ZoneDeviceViewModel(GKDevice device)
		{
			Device = device;
		}

		public GKDriver Driver
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
					return GKManager.GetPresentationZoneOrLogic(Device);
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