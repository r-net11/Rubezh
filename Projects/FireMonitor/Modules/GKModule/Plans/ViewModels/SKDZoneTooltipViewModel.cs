using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class SKDZoneTooltipViewModel : StateTooltipViewModel<GKSKDZone>
	{
		public GKSKDZone SKDZone
		{
			get { return Item; }
		}

		public SKDZoneTooltipViewModel(GKSKDZone guardZone)
			: base(guardZone)
		{
		}
	}
}