using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		public GKDirection Direction { get; private set; }

		public DirectionViewModel(GKDirection direction)
		{
			Direction = direction;
		}
	}
}
