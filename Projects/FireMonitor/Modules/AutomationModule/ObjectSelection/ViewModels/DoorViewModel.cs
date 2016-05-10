using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public SKDDoor Door { get; private set; }

		public DoorViewModel(SKDDoor door)
		{
			Door = door;
		}
	}
}
