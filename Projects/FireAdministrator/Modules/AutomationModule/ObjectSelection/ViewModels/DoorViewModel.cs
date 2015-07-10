using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public GKDoor GKDoor { get; private set; }

		public DoorViewModel(GKDoor gkDoor)
		{
			GKDoor = gkDoor;
		}
	}
}
