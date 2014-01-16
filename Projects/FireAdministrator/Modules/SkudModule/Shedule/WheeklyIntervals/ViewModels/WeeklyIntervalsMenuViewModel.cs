using Infrastructure.Common.Windows.ViewModels;

namespace SkudModule.ViewModels
{
	public class WeeklyIntervalsMenuViewModel : BaseViewModel
	{
		public WeeklyIntervalsMenuViewModel(WeeklyIntervalsViewModel context)
		{
			Context = context;
		}

		public WeeklyIntervalsViewModel Context { get; private set; }
	}
}