using FiresecAPI.GK;
using Infrastructure.Client.Plans.Presenter;

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