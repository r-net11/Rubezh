using FiresecClient;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using JournalModule.Reports;
using JournalModule.ViewModels;
using StrazhAPI.Enums;
using StrazhAPI.Journal;
using StrazhAPI.Models.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JournalModule
{
	public class JournalModuleLoader : ModuleBase, IReportProviderModule, ILayoutProviderModule
	{
		private NavigationItem _journalNavigationItem;
		private JournalViewModel _journalViewModel;
		private ArchiveViewModel _archiveViewModel;

		public override void CreateViewModels()
		{
			ServiceFactoryBase.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
			ServiceFactoryBase.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournalItem);
			_journalViewModel = new JournalViewModel();
			_archiveViewModel = new ArchiveViewModel();
			ServiceFactoryBase.Events.GetEvent<ShowArchiveEvent>().Unsubscribe(OnShowArchive);
			ServiceFactoryBase.Events.GetEvent<ShowArchiveEvent>().Subscribe(OnShowArchive);
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
			_journalViewModel.SelectedJournal = _journalViewModel.JournalItems.FirstOrDefault();
		}
		void OnNewJournalItem(List<JournalItem> journalItems)
		{
			if (_journalNavigationItem == null || !_journalNavigationItem.IsSelected)
				UnreadJournalCount += journalItems.Count;
		}

		void OnShowArchive(ShowArchiveEventArgs showArchiveEventArgs)
		{
			if (showArchiveEventArgs != null)
			{
				_archiveViewModel.Sort(showArchiveEventArgs);
			}
		}

		public override void Initialize()
		{
			_journalViewModel.Initialize();
			_archiveViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_journalNavigationItem = new NavigationItem<ShowJournalEvent>(_journalViewModel, "Журнал событий", "Book");
			UnreadJournalCount = 0;
			return new List<NavigationItem>
			{
				_journalNavigationItem,
				new NavigationItem<ShowArchiveEvent, ShowArchiveEventArgs>(_archiveViewModel, "Архив", "Archive")
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Journal; }
		}

		#region IReportProviderModule Members
		public IEnumerable<IReportProvider> GetReportProviders()
		{
			return new List<IReportProvider>
			{
#if DEBUG
				new JournalReport(),
#endif
			};
		}
		#endregion

		public override void AfterInitialize()
		{
			SafeFiresecService.NewJournalItemEvent -= OnNewJournalItem;
			SafeFiresecService.NewJournalItemEvent += OnNewJournalItem;

			SafeFiresecService.GetFilteredArchiveCompletedEvent -= OnGetFilteredArchiveCompletedEvent;
			SafeFiresecService.GetFilteredArchiveCompletedEvent += OnGetFilteredArchiveCompletedEvent;

			var journalFilter = new JournalFilter();
			var result = FiresecManager.FiresecService.GetFilteredJournalItems(journalFilter);
			if (!result.HasError)
			{
				_journalViewModel.SetJournalItems(result.Result);
			}
			_archiveViewModel.Update();
		}

		static void OnNewJournalItem(JournalItem journalItem)
		{
			ApplicationService.Invoke(() =>
			{
				var journalItems = new List<JournalItem> {journalItem};
				ServiceFactoryBase.Events.GetEvent<NewJournalItemsEvent>().Publish(journalItems);
			});
		}

		static void OnGetFilteredArchiveCompletedEvent(IEnumerable<JournalItem> journalItems, Guid archivePortionUID)
		{
			ApplicationService.Invoke(() =>
			{
				var archiveResult = new ArchiveResult
				{
					ArchivePortionUID = archivePortionUID,
					JournalItems = journalItems
				};
				ServiceFactoryBase.Events.GetEvent<GetFilteredArchiveCompletedEvent>().Publish(archiveResult);
			});
		}

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.Journal, "Журнал событий", "Book.png", p =>
			{
				var layoutPartJournalProperties = p as LayoutPartReferenceProperties;
				var filter = FiresecManager.SystemConfiguration.JournalFilters .FirstOrDefault(x => layoutPartJournalProperties != null && x.UID == layoutPartJournalProperties.ReferenceUID)
					?? new JournalFilter();

				var journalViewModel = new JournalViewModel(filter);
				journalViewModel.Initialize();
				var result = FiresecManager.FiresecService.GetFilteredJournalItems(filter);
				if (!result.HasError)
				{
					journalViewModel.SetJournalItems(result.Result);
				}

				return journalViewModel;
			});
			yield return new LayoutPartPresenter(LayoutPartIdentities.Archive, "Архив", "Archive.png", p => _archiveViewModel);
		}

		#endregion
	}
}