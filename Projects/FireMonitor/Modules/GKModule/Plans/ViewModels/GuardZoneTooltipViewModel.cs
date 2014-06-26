using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class GuardZoneTooltipViewModel : BaseViewModel
	{
		public XGuardZone GuardZone { get; private set; }
		public XState State { get; private set; }

		public GuardZoneTooltipViewModel(XGuardZone guardZone)
		{
			State = guardZone.State;
			GuardZone = guardZone;
		}

		public void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => GuardZone);
		}
	}
}