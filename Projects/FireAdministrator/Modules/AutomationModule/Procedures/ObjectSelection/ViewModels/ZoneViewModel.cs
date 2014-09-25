using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public XZone Zone { get; private set; }
		public XGuardZone GuardZone { get; private set; }
		public SKDZone SKDZone { get; private set; }

		public ZoneViewModel(XZone zone)
		{
			Zone = zone;
		}

		public ZoneViewModel(XGuardZone guardZone)
		{
			GuardZone = guardZone;
		}

		public ZoneViewModel(SKDZone sKDZone)
		{
			SKDZone = sKDZone;
		}
	}
}