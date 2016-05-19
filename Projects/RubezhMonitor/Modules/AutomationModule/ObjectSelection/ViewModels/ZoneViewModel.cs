using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;

namespace AutomationModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public GKZone Zone { get; private set; }
		public GKGuardZone GuardZone { get; private set; }

		public ZoneViewModel(GKZone zone)
		{
			Zone = zone;
		}

		public ZoneViewModel(GKGuardZone guardZone)
		{
			GuardZone = guardZone;
		}
	}
}