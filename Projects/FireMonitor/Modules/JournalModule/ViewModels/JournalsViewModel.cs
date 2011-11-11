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
            Journals = new List<FilteredJournalViewModel>();

            Journals.Add(new FilteredJournalViewModel(new JournalFilter() { Name = " Все события" }));
            SelectedJournal = Journals[0];

            FiresecManager.SystemConfiguration.JournalFilters.ForEach(
                journalFilter => Journals.Add(new FilteredJournalViewModel(journalFilter)));
        }

        public List<FilteredJournalViewModel> Journals { get; private set; }
        public FilteredJournalViewModel SelectedJournal { get; set; }
    }
}