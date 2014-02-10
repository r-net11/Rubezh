using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SKDZoneTooltipViewModel : BaseViewModel
	{
		public SKDZone Zone { get; private set; }
		public SKDZoneState State { get; private set; }

		public SKDZoneTooltipViewModel(SKDZone zone)
		{
			State = zone.State;
			Zone = zone;
		}

		public void OnStateChanged()
		{
			OnPropertyChanged("State");
			OnPropertyChanged("Zone");
		}
	}
}