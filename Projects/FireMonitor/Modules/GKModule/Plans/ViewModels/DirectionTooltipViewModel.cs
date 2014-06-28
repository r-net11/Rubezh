using FiresecAPI.GK;
using Infrastructure.Client.Plans.Presenter;

namespace GKModule.ViewModels
{
	public class DirectionTooltipViewModel : StateTooltipViewModel<XDirection>
	{
		public XDirection Direction
		{
			get { return Item; }
		}

		public DirectionTooltipViewModel(XDirection direction)
			: base(direction)
		{
		}
	}
}