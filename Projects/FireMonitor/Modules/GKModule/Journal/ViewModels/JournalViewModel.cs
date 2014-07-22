using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Models;


namespace GKModule.ViewModels
{
	public class JournalViewModel : BaseViewModel
	{
		public JournalFilterViewModel JournalFilterViewModel { get; private set; }

		public JournalViewModel(XJournalFilter journalFilter)
		{
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<XJournalSettingsUpdatedEvent>().Unsubscribe(OnSettingsChanged);
			ServiceFactory.Events.GetEvent<XJournalSettingsUpdatedEvent>().Subscribe(OnSettingsChanged);

			JournalFilterViewModel = new JournalFilterViewModel(journalFilter);
			JournalItems = new ObservableCollection<JournalItemViewModel>();
		}

		ObservableCollection<JournalItemViewModel> _journalItems;
		public ObservableCollection<JournalItemViewModel> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged(() => JournalItems);
			}
		}

		JournalItemViewModel _selectedJournal;
		public JournalItemViewModel SelectedJournal
		{
			get { return _selectedJournal; }
			set
			{
				_selectedJournal = value;
				OnPropertyChanged(() => SelectedJournal);
			}
		}

		public void OnNewJournal(List<XJournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (JournalFilterViewModel.FilterStateClass(journalItem) == false)
					continue;
				if (JournalFilterViewModel.FilterEventName(journalItem) == false)
					continue;

				var journalItemViewModel = new JournalItemViewModel(journalItem);
				if (JournalItems.Count > 0)
					JournalItems.Insert(0, journalItemViewModel);
				else
					JournalItems.Add(journalItemViewModel);

				if (JournalItems.Count > JournalFilterViewModel.JournalFilter.LastRecordsCount)
					JournalItems.RemoveAt(JournalFilterViewModel.JournalFilter.LastRecordsCount);
			}

			if (SelectedJournal == null)
				SelectedJournal = JournalItems.FirstOrDefault();
		}

		public List<XJournalColumnType> AdditionalColumns
		{
			get { return ClientSettings.ArchiveDefaultState.XAdditionalColumns; }
		}

		bool additionalColumnsChanged;
		public bool AdditionalColumnsChanged
		{
			get { return additionalColumnsChanged; }
			set
			{
				additionalColumnsChanged = value;
				OnPropertyChanged(() => AdditionalColumnsChanged);
			}
		}

		void OnSettingsChanged(object o)
		{
			AdditionalColumnsChanged = !AdditionalColumnsChanged;
		}
	}
}