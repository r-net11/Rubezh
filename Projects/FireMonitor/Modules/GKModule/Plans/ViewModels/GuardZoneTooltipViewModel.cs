using FiresecAPI.GK;
using Infrastructure.Client.Plans.Presenter;

namespace GKModule.ViewModels
{
	public class GuardZoneTooltipViewModel : StateTooltipViewModel<XGuardZone>
	{
		public XGuardZone GuardZone
		{
			get { return Item; }
		}

		public GuardZoneTooltipViewModel(XGuardZone guardZone)
			: base(guardZone)
		{
		}
	}
}