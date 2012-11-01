using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Events;
using JournalModule.Reports;
using JournalModule.ViewModels;

namespace JournalModule
{
	public class JournalModuleLoader : ModuleBase, IReportProviderModule
	{
		private NavigationItem _journalNavigationItem;
		JournalsViewModel JournalsViewModel;
		ArchiveViewModel ArchiveViewModel;

		public JournalModuleLoader()
		{
			ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Subscribe(OnNewJournalRecord);
			JournalsViewModel = new JournalsViewModel();
			ArchiveViewModel = new ArchiveViewModel();
		}

        int _unreadJournalCount;
		private int UnreadJournalCount
		{
			get { return _unreadJournalCount; }
			set
			{
				_unreadJournalCount = value;
				if (_journalNavigationItem != null)
					_journalNavigationItem.Title = UnreadJournalCount == 0 ? "Журнал событий" : string.Format("Журнал событий {0}", UnreadJournalCount);
			}
		}

		void OnShowJournal(object obj)
		{
			UnreadJournalCount = 0;
			JournalsViewModel.SelectedJournal = JournalsViewModel.Journals[0];
		}
        void OnNewJournalRecord(List<JournalRecord> journalRecords)
        {
            if (_journalNavigationItem == null || !_journalNavigationItem.IsSelected)
                UnreadJournalCount += journalRecords.Count;
        }

		public override void Initialize()
		{
			JournalsViewModel.Initialize();
			ArchiveViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_journalNavigationItem = new NavigationItem<ShowJournalEvent>(JournalsViewModel, "Журнал событий", "/Controls;component/Images/book.png");
			UnreadJournalCount = 0;
			return new List<NavigationItem>()
			{
				_journalNavigationItem,
				new NavigationItem<ShowArchiveEvent>(ArchiveViewModel, "Архив", "/Controls;component/Images/archive.png")
			};
		}
		public override string Name
		{
			get { return "Журнал событий и Архив"; }
		}

		#region IReportProviderModule Members
		public IEnumerable<IReportProvider> GetReportProviders()
		{
			return new List<IReportProvider>()
			{
				new JournalReport()
			};
		}
		#endregion
	}
}