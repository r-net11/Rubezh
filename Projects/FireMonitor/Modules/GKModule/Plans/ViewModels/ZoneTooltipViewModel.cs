using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;

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