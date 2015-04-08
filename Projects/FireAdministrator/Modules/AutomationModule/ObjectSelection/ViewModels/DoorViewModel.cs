using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public SKDDoor Door { get; private set; }
		public GKDoor GKDoor { get; private set; }

		public DoorViewModel(SKDDoor door)
		{
			Door = door;
		}

		public DoorViewModel(GKDoor gkDoor)
		{
			GKDoor = gkDoor;
		}
	}
}
