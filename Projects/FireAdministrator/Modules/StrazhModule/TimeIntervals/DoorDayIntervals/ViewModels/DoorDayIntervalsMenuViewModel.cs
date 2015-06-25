using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DoorDayIntervalsMenuViewModel : BaseViewModel
	{
		public DoorDayIntervalsMenuViewModel(DoorDayIntervalsViewModel dayIntervalsViewModel)
		{
			Context = dayIntervalsViewModel;
		}

		public DoorDayIntervalsViewModel Context { get; private set; }
	}
}