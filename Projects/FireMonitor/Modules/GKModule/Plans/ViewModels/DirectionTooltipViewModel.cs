using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;

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