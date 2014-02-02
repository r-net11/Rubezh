using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecAPI;

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