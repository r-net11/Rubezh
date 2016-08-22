using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public SKDZone SKDZone { get; private set; }

		public ZoneViewModel(SKDZone sKDZone)
		{
			SKDZone = sKDZone;
		}
	}
}