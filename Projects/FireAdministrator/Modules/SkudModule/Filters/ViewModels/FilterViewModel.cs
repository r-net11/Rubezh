using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
		public SKDJournalFilter JournalFilter { get; set; }

        public FilterViewModel(SKDJournalFilter journalFilter)
		{
			JournalFilter = journalFilter;
		}

		public void Update()
		{
            OnPropertyChanged("JournalFilter");
		}
    }
}