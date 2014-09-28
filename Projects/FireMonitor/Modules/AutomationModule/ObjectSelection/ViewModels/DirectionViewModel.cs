using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		public XDirection Direction { get; private set; }

		public DirectionViewModel(XDirection direction)
		{
			Direction = direction;
		}
	}
}
