using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using JournalModule.ViewModels;

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
			JournalsViewModel = new JournalsViewModel();
			ArchiveViewModel = new ArchiveViewModel();
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

		void OnShowJournal(object obj)
		{
			UnreadJournalCount = 0;
			JournalsViewModel.SelectedJournal = JournalsViewModel.Journals[0];
			ServiceFactory.Layout.Show(JournalsViewModel);
		}
		void OnShowArchive(object obj)
		{
			ServiceFactory.Layout.Show(ArchiveViewModel);
			//ArchiveViewModel.Update();
		}
		void OnNewJournalRecord(JournalRecord journalItem)
		{
			if (_journalItem == null || !_journalItem.IsSelected)
				++UnreadJournalCount;
		}

		public override void Initialize()
		{
			JournalsViewModel.Initialize();
			ArchiveViewModel.Initialize();
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