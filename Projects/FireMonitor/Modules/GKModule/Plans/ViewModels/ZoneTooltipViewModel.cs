using FiresecAPI.GK;
using Infrastructure.Client.Plans.Presenter;

namespace GKModule.ViewModels
{
	public class ZoneTooltipViewModel : StateTooltipViewModel<XZone>
	{
		public XZone Zone
		{
			get { return Item; }
		}

		public ZoneTooltipViewModel(XZone zone)
			: base(zone)
		{
		}
	}
}