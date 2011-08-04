using FiresecAPI.Models;
using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
        public FilterViewModel(JournalFilter journalFilter)
        {
            JournalFilter = journalFilter;
        }

        JournalFilter _journalFilter;
        public JournalFilter JournalFilter
        {
            get { return _journalFilter; }
            set
            {
                _journalFilter = value;
                OnPropertyChanged("JournalFilter");
            }
        }
    }
}
