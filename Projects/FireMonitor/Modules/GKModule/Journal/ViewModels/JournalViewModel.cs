using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.GK;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecClient;

using System.Windows.Data;
using FiresecAPI;


namespace GKModule.ViewModels
{
	public class JournalViewModel : BaseViewModel
	{
		public JournalFilterViewModel JournalFilterViewModel { get; private set; }

		public JournalViewModel(XJournalFilter journalFilter)
		{
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournal);

			JournalFilterViewModel = new JournalFilterViewModel(journalFilter);
			JournalItems = new ObservableCollection<JournalItemViewModel>();

		}

		public bool IsManyGK
		{
			get	{ return XManager.IsManyGK(); }
		}

		bool _showSubsystem;
		public bool ShowSubsystem
		{
			get { return _showSubsystem; }
			set
			{
				_showSubsystem = value;
				OnPropertyChanged("ShowSubsystem");
			}
		}

		bool _showIp;
		public bool ShowIp
		{
			get { return _showIp; }
			set
			{
				_showIp = value;
				OnPropertyChanged("ShowIp");
			}
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

		
	}

}