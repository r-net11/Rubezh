using RubezhAPI.GK;
using Infrastructure.Client.Plans.Presenter;

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