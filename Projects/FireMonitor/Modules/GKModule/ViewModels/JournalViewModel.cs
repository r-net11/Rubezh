using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.GK;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Infrastructure.Events;

namespace GKModule.ViewModels
{
	public class JournalViewModel : ViewPartViewModel
	{
		const int MaxCount = 100;

		public JournalViewModel()
		{
			ServiceFactory.Events.GetEvent<NewJournalEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewJournalEvent>().Subscribe(OnNewJournal);
			JournalItems = new ObservableCollection<JournalItem>();
		}

		public void Initialize()
		{
		}

		void OnNewJournal(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (JournalItems.Count > 0)
					JournalItems.Insert(0, journalItem);
				else
					JournalItems.Add(journalItem);

				if (JournalItems.Count > MaxCount)
					JournalItems.RemoveAt(MaxCount);
			}

			if (SelectedJournal == null)
				SelectedJournal = JournalItems.FirstOrDefault();
		}

		ObservableCollection<JournalItem> _journalItems;
		public ObservableCollection<JournalItem> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
			}
		}

		JournalItem _selectedJournal;
		public JournalItem SelectedJournal
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