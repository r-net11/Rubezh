using Infrastructure.Common.Windows.ViewModels;

namespace FiltersModule.ViewModels
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