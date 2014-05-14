using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DirectionTooltipViewModel : BaseViewModel
	{
		public XState State { get; private set; }
		public XDirection Direction { get; private set; }

		public DirectionTooltipViewModel(XDirection direction)
		{
			Direction = direction;
			State = direction.State;
		}

		public void OnStateChanged()
		{
			OnPropertyChanged("State");
			OnPropertyChanged("Direction");
		}
	}
}