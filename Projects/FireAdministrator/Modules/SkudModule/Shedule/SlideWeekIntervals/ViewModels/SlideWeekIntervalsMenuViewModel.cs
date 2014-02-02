using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideWeekIntervalsMenuViewModel : BaseViewModel
	{
		public SlideWeekIntervalsMenuViewModel(SlideWeekIntervalsViewModel context)
		{
			Context = context;
		}

		public SlideWeekIntervalsViewModel Context { get; private set; }
	}
}