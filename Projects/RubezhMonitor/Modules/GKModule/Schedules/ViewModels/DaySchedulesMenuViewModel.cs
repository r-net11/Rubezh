using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DaySchedulesMenuViewModel : BaseViewModel
	{
		public DaySchedulesMenuViewModel(DaySchedulesViewModel daySchedulesViewModel)
		{
			Context = daySchedulesViewModel;
		}

		public DaySchedulesViewModel Context { get; private set; }
	}
}