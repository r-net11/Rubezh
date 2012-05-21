using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using JournalModule.ViewModels;
using Infrastructure.Common.Navigation;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace JournalModule
{
	public class JournalModuleLoader : ModuleBase
	{
		private NavigationItem _journalItem;
		private int _unreadJournalCount;
		JournalsViewModel JournalsViewModel;
		ArchiveViewModel ArchiveViewModel;

		public JournalModuleLoader()
		{
			ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(OnShowArchive);
			ServiceFactory.Events.GetEvent<NewJournalRecordEvent>().Subscribe(OnNewJournalRecord);
		}

		private int UnreadJournalCount
		{
			get { return _unreadJournalCount; }
			set
			{
				_unreadJournalCount = value;
				if (_journalItem != null)
					_journalItem.Title = UnreadJournalCount == 0 ? "Журнал событий" : string.Format("Журнал событий {0}", UnreadJournalCount);
			}
		}

		void CreateViewModels()
		{
			JournalsViewModel = new JournalsViewModel();
			ArchiveViewModel = new ArchiveViewModel();
		}

		void OnShowJournal(object obj)
		{
			UnreadJournalCount = 0;
			JournalsViewModel.SelectedJournal = JournalsViewModel.Journals[0];
			ServiceFactory.Layout.Show(JournalsViewModel);
		}
		void OnShowArchive(object obj)
		{
			//ArchiveViewModel.Initialize();
			ServiceFactory.Layout.Show(ArchiveViewModel);
		}
		void OnNewJournalRecord(JournalRecord journalItem)
		{
			if (_journalItem == null || !_journalItem.IsSelected)
				++UnreadJournalCount;
		}

		public override void Initialize()
		{
			CreateViewModels();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_journalItem = new NavigationItem<ShowJournalEvent>("Журнал событий", "/Controls;component/Images/book.png");
			UnreadJournalCount = 0;
			return new List<NavigationItem>()
			{
				_journalItem,
				new NavigationItem<ShowArchiveEvent>("Архив", "/Controls;component/Images/archive.png")
			};
		}
	}
}