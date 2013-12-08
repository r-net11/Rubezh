using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ZoneTooltipViewModel : BaseViewModel
	{
		public XZone Zone { get; private set; }
		public XState State { get; private set; }

		public ZoneTooltipViewModel(XZone zone)
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