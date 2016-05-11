using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class GKDoorTooltipViewModel : StateTooltipViewModel<GKDoor>
	{
		public GKDoor Door
		{
			get { return Item; }
		}

		public GKDoorTooltipViewModel(GKDoor door)
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