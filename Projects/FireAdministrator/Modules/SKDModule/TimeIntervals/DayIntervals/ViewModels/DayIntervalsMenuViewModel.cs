using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalsMenuViewModel : BaseViewModel
	{
		public DayIntervalsMenuViewModel(DayIntervalsViewModel dayIntervalsViewModel)
		{
			Context = dayIntervalsViewModel;
		}

		public DayIntervalsViewModel Context { get; private set; }
	}
}