using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace SKDModule.Intervals.Base.ViewModels
{
	public class BaseIntervalsMenuViewModel : BaseViewModel
	{
		public BaseIntervalsMenuViewModel(MenuViewPartViewModel context)
		{
			Context = context;
		}

		public MenuViewPartViewModel Context { get; private set; }
	}
}