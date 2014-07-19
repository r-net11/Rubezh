using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Journal;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using JournalModule.ViewModels;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Client.Layout;

namespace JournalModule
{
	public class JournalModuleLoader : ModuleBase, ILayoutProviderModule
	{
		private NavigationItem _journalNavigationItem;
		JournalViewModel JournalViewModel;
		ArchiveViewModel ArchiveViewModel;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournalItem);
			JournalViewModel = new JournalViewModel();
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
			JournalViewModel.SelectedJournal = JournalViewModel.JournalItems.FirstOrDefault();
		}
		void OnNewJournalItem(List<JournalItem> journalItems)
		{
			if (_journalNavigationItem == null || !_journalNavigationItem.IsSelected)
				UnreadJournalCount += journalItems.Count;
		}

		public override void Initialize()
		{
			JournalViewModel.Initialize();
			ArchiveViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_journalNavigationItem = new NavigationItem<ShowJournalEvent>(JournalViewModel, "Журнал событий", "/Controls;component/Images/book.png");
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

		public override void AfterInitialize()
		{
			SafeFiresecService.NewJournalItemEvent -= new Action<JournalItem>(OnNewJournalItem);
			SafeFiresecService.NewJournalItemEvent += new Action<JournalItem>(OnNewJournalItem);

			SafeFiresecService.GetFilteredSKDArchiveCompletedEvent -= new Action<IEnumerable<JournalItem>, Guid>(OnGetFilteredSKDArchiveCompletedEvent);
			SafeFiresecService.GetFilteredSKDArchiveCompletedEvent += new Action<IEnumerable<JournalItem>, Guid>(OnGetFilteredSKDArchiveCompletedEvent);

			var journalFilter = new JournalFilter();
			var result = FiresecManager.FiresecService.GetFilteredJournalItems(journalFilter);
			if (!result.HasError)
			{
				JournalViewModel.OnNewJournalItems(new List<JournalItem>(result.Result));
			}
		}

		void OnNewJournalItem(JournalItem journalItem)
		{
			ApplicationService.Invoke(() =>
			{
				var journalItems = new List<JournalItem>();
				journalItems.Add(journalItem);
				ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Publish(journalItems);
			});
		}

		void OnGetFilteredSKDArchiveCompletedEvent(IEnumerable<JournalItem> journalItems, Guid archivePortionUID)
		{
			ApplicationService.Invoke(() =>
			{
				var archiveResult = new SKDArchiveResult()
				{
					ArchivePortionUID = archivePortionUID,
					JournalItems = journalItems
				};
				ServiceFactory.Events.GetEvent<GetFilteredSKDArchiveCompletedEvent>().Publish(archiveResult);
			});
		}

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.Journal, "Журнал событий", "Book.png", (p) => JournalViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.Archive, "Архив", "Archive.png", (p) => ArchiveViewModel);
		}

		#endregion
	}
}