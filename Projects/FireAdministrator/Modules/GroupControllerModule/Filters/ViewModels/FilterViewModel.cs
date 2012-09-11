using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
		public XJournalFilter JournalFilter { get; set; }

        public FilterViewModel(XJournalFilter journalFilter)
		{
			JournalFilter = journalFilter;
		}

		public void Update()
		{
            OnPropertyChanged("JournalFilter");
		}
    }
}