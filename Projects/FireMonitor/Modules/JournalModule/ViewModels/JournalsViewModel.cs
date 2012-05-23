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
		}

		public void Initialize()
		{
			Journals = new List<FilteredJournalViewModel>();

			Journals.Add(new FilteredJournalViewModel(new JournalFilter() { Name = " Все события" }));
			SelectedJournal = Journals[0];

			foreach (var journalFilter in FiresecManager.SystemConfiguration.JournalFilters)
			{
				var filteredJournalViewModel = new FilteredJournalViewModel(journalFilter);
				Journals.Add(filteredJournalViewModel);
			}
		}

		List<FilteredJournalViewModel> _journals;
		public List<FilteredJournalViewModel> Journals
		{
			get { return _journals; }
			set
			{
				_journals = value;
				OnPropertyChanged("Journals");
			}
		}

		FilteredJournalViewModel _selectedJournal;
		public FilteredJournalViewModel SelectedJournal
		{
			get { return _selectedJournal; }
			set
			{
				_selectedJournal = value;
				OnPropertyChanged("SelectedJournal");
			}
		}
	}
}