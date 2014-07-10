using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Journal;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
	public class JournalViewModel : ViewPartViewModel
	{
		public JournalViewModel()
		{
			JournalItems = new ObservableCollection<JournalItemViewModel>();
		}

		public void Initialize()
		{
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournalItems);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournalItems);
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

		public void OnNewJournalItems(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				if (JournalItems.Count > 0)
					JournalItems.Insert(0, journalItemViewModel);
				else
					JournalItems.Add(journalItemViewModel);

				if (JournalItems.Count > 100)
					JournalItems.RemoveAt(100);
			}

			if (SelectedJournal == null)
				SelectedJournal = JournalItems.FirstOrDefault();
		}
	}
}