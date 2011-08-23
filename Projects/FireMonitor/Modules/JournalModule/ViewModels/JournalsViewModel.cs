using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class JournalsViewModel : RegionViewModel
    {
        public JournalsViewModel()
        {
            Initialize();
        }

        void Initialize()
        {
            Journals = new List<FilteredJournalViewModel>();

            var defaulstFilter = new JournalFilter() { Name = " Все события" };
            Journals.Add(new FilteredJournalViewModel(defaulstFilter));
            SelectedJournal = Journals[0];

            FiresecManager.SystemConfiguration.JournalFilters.ForEach(
                journalFilter => Journals.Add(new FilteredJournalViewModel(journalFilter)));
        }

        public List<FilteredJournalViewModel> Journals { get; private set; }
        public FilteredJournalViewModel SelectedJournal { get; set; }
    }
}