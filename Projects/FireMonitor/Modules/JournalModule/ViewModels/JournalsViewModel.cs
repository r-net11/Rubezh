using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class JournalsViewModel : ViewPartViewModel
	{
		public JournalsViewModel()
		{
			Journals = new List<FilteredJournalViewModel>();
			Journals.Add(new FilteredJournalViewModel(new JournalFilter() { Name = " Все события" }));
			SelectedJournal = Journals.FirstOrDefault();

			foreach (var journalFilter in FiresecManager.SystemConfiguration.JournalFilters)
			{
				var filteredJournalViewModel = new FilteredJournalViewModel(journalFilter);
				Journals.Add(filteredJournalViewModel);
			}
		}

		public void Initialize()
		{
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