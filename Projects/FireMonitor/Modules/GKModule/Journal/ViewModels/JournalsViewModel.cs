using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class JournalsViewModel : ViewPartViewModel
	{
		public JournalsViewModel()
		{
			Journals = new List<JournalViewModel>();
			Journals.Add(new JournalViewModel(new XJournalFilter() { Name = " Все события" }));
			SelectedJournal = Journals.FirstOrDefault();
		}

		public void Initialize()
		{
			if (XManager.DeviceConfiguration.JournalFilters != null)
				foreach (var journalFilter in XManager.DeviceConfiguration.JournalFilters)
				{
					var filteredJournalViewModel = new JournalViewModel(journalFilter);
					Journals.Add(filteredJournalViewModel);
				}
		}

		List<JournalViewModel> _journals;
		public List<JournalViewModel> Journals
		{
			get { return _journals; }
			set
			{
				_journals = value;
				OnPropertyChanged("Journals");
			}
		}

		JournalViewModel _selectedJournal;
		public JournalViewModel SelectedJournal
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