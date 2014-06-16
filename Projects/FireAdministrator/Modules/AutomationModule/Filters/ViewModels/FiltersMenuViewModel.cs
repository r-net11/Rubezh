using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class FiltersMenuViewModel : BaseViewModel
	{
		public FiltersMenuViewModel(FiltersViewModel context)
		{
			Context = context;
		}

		public FiltersViewModel Context { get; private set; }
	}
}