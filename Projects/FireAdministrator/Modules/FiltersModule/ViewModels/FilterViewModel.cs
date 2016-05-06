using StrazhAPI.Journal;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace FiltersModule.ViewModels
{
	public class FilterViewModel : BaseViewModel
	{
		public JournalFilter Filter { get; private set; }

		public FilterViewModel(JournalFilter filter)
		{
			Filter = filter;
		}

		public void Update(JournalFilter filter)
		{
			Filter = filter;
			OnPropertyChanged(() => Filter);
		}
	}
}