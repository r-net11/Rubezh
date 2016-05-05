using StrazhAPI.SKD;
using Infrastructure.Client.Plans.Presenter;

namespace StrazhModule.ViewModels
{
	public class SKDDoorTooltipViewModel : StateTooltipViewModel<SKDDoor>
	{
		public SKDDoor Door
		{
			get { return Item; }
		}

		public SKDDoorTooltipViewModel(SKDDoor door)
			: base(door)
		{
		}

		public override void OnStateChanged()
		{
			base.OnStateChanged();
			OnPropertyChanged(() => Door);
		}
	}
}