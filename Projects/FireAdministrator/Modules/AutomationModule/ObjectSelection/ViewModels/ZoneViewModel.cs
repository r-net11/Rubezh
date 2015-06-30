using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public GKZone Zone { get; private set; }
		public GKGuardZone GuardZone { get; private set; }
		public SKDZone SKDZone { get; private set; }

		public ZoneViewModel(GKZone zone)
		{
			Zone = zone;
		}

		public ZoneViewModel(GKGuardZone guardZone)
		{
			GuardZone = guardZone;
		}

		public ZoneViewModel(SKDZone sKDZone)
		{
			SKDZone = sKDZone;
		}
	}
}