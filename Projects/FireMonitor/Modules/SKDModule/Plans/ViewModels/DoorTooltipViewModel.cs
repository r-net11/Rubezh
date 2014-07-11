using FiresecAPI.SKD;
using Infrastructure.Client.Plans.Presenter;

namespace SKDModule.ViewModels
{
	public class DoorTooltipViewModel : StateTooltipViewModel<SKDDoor>
	{
		public SKDDoor Door
		{
			get { return Item; }
		}

		public DoorTooltipViewModel(SKDDoor door)
			: base(door)
		{
		}
	}
}