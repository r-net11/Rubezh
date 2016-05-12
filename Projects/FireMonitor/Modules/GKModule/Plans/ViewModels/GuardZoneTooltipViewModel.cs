using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class GuardZoneTooltipViewModel : StateTooltipViewModel<GKGuardZone>
	{
		public GKGuardZone GuardZone
		{
			get { return Item; }
		}

		public GuardZoneTooltipViewModel(GKGuardZone guardZone)
			: base(guardZone)
		{
		}
	}
}