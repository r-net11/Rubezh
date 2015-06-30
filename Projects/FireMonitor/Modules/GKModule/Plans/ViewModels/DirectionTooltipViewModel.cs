using FiresecAPI.GK;
using Infrastructure.Client.Plans.Presenter;

namespace GKModule.ViewModels
{
	public class DirectionTooltipViewModel : StateTooltipViewModel<GKDirection>
	{
		public GKDirection Direction
		{
			get { return Item; }
		}

		public DirectionTooltipViewModel(GKDirection direction)
			: base(direction)
		{
		}
	}
}