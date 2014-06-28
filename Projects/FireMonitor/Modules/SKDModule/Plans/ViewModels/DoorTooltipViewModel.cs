using FiresecAPI.SKD;
using Infrastructure.Client.Plans.Presenter;

namespace SKDModule.ViewModels
{
	public class DoorTooltipViewModel : StateTooltipViewModel<Door>
	{
		public Door Door
		{
			get { return Item; }
		}

		public DoorTooltipViewModel(Door door)
			: base(door)
		{
		}
	}
}