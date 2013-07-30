using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.GK;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class JournalViewModel : BaseViewModel
	{
		public XJournalFilter JournalFilter { get; private set; }

		public JournalViewModel(XJournalFilter journalFilter)
		{
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournal);

			JournalFilter = journalFilter;
			JournalItems = new ObservableCollection<JournalItemViewModel>();
		}

		ObservableCollection<JournalItemViewModel> _journalItems;
		public ObservableCollection<JournalItemViewModel> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
			}
		}

		JournalItemViewModel _selectedJournal;
		public JournalItemViewModel SelectedJournal
		{
			get { return _selectedJournal; }
			set
			{
				_selectedJournal = value;
				OnPropertyChanged("SelectedJournal");
			}
		}

		public void OnNewJournal(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (FilterStateClass(journalItem) == false)
					continue;
				if (FilterEventName(journalItem) == false)
					continue;

				var journalItemViewModel = new JournalItemViewModel(journalItem);
				if (JournalItems.Count > 0)
					JournalItems.Insert(0, journalItemViewModel);
				else
					JournalItems.Add(journalItemViewModel);

				if (JournalItems.Count > JournalFilter.LastRecordsCount)
					JournalItems.RemoveAt(JournalFilter.LastRecordsCount);
			}

			if (SelectedJournal == null)
				SelectedJournal = JournalItems.FirstOrDefault();
		}

		bool FilterStateClass(JournalItem journalItem)
		{
			if (JournalFilter.StateClasses.Count > 0)
			{
				return JournalFilter.StateClasses.Contains(journalItem.StateClass);
			}
			return true;
		}

		bool FilterEventName(JournalItem journalItem)
		{
			if (JournalFilter.EventNames.Count > 0)
			{
				return JournalFilter.EventNames.Contains(journalItem.Name);
			}
			return true;
		}
	}
}