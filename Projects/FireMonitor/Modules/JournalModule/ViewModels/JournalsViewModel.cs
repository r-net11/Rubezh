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
            Journals = new List<FilteredJournalViewModel>();
        }

        void Initialize()
        {
            var defaulstFilter = new JournalFilter();
            defaulstFilter.Name = " Все события";
            defaulstFilter.LastRecordsCount = FilteredJournalViewModel.RecordsMaxCount;
            Journals.Add(new FilteredJournalViewModel(defaulstFilter));
            SelectedJournal = Journals[0];

            foreach (var journalFilter in FiresecManager.SystemConfiguration.JournalFilters)
            {
                Journals.Add(new FilteredJournalViewModel(journalFilter));
            }
        }

        public List<FilteredJournalViewModel> Journals { get; private set; }
        public FilteredJournalViewModel SelectedJournal { get; set; }
    }
}