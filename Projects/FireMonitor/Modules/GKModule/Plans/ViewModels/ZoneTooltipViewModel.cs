using FiresecAPI.GK;
using Infrastructure.Client.Plans.Presenter;

namespace GKModule.ViewModels
{
	public class ZoneTooltipViewModel : StateTooltipViewModel<GKZone>
	{
		public GKZone Zone
		{
			get { return Item; }
		}

		public ZoneTooltipViewModel(GKZone zone)
			: base(zone)
		{
		}
	}
}