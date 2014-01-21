using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalsMenuViewModel : BaseViewModel
	{
		public TimeIntervalsMenuViewModel(TimeIntervalsViewModel context)
		{
			Context = context;
		}

		public TimeIntervalsViewModel Context { get; private set; }
	}
}