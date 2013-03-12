using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
	public class JournalsViewModel : ViewPartViewModel
	{
		public void Initialize()
		{
			Journals = new ObservableCollection<FilteredJournalViewModel>();
			Journals.Add(new FilteredJournalViewModel(new JournalFilter() { Name = " Все события" }));
			SelectedJournal = Journals.FirstOrDefault();

			foreach (var journalFilter in FiresecManager.SystemConfiguration.JournalFilters)
			{
				var filteredJournalViewModel = new FilteredJournalViewModel(journalFilter);
				Journals.Add(filteredJournalViewModel);
			}

			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Unsubscribe(OnNewJournalRecords);
			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Subscribe(OnNewJournalRecords);
		}

		ObservableCollection<FilteredJournalViewModel> _journals;
		public ObservableCollection<FilteredJournalViewModel> Journals
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

		void OnNewJournalRecords(List<JournalRecord> journalRecords)
		{
			foreach (var journalRecord in journalRecords)
			{
				if (journalRecord.StateType == StateType.Fire && FiresecManager.CheckPermission(PermissionType.Oper_NoAlarmConfirm) == false)
				{
					var confirmationViewModel = new ConfirmationViewModel(journalRecord);
					ApplicationService.Invoke(() =>
					{
						DialogService.ShowWindow(confirmationViewModel);
					});
				}
			}
		}
	}
}