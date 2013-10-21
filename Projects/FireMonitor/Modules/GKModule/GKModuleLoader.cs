using System;
using System.Collections.Generic;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans;
using GKModule.Reports;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using XFiresecAPI;
using GKProcessor;
using GKProcessor.Events;

namespace GKModule
{
	public class GKModuleLoader : ModuleBase, IReportProviderModule
	{
		static DevicesViewModel DevicesViewModel;
		static DeviceParametersViewModel DeviceParametersViewModel;
		static ZonesViewModel ZonesViewModel;
		static DirectionsViewModel DirectionsViewModel;
		NavigationItem _journalNavigationItem;
		static JournalsViewModel JournalsViewModel;
		static ArchiveViewModel ArchiveViewModel;
		static AlarmsViewModel AlarmsViewModel;
		NavigationItem _zonesNavigationItem;
		NavigationItem _directionsNavigationItem;
		private PlanPresenter _planPresenter;

		public GKModuleLoader()
		{
			_planPresenter = new PlanPresenter();
			GKDBHelper.ConnectionString = @"Data Source=" + AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf") + ";Persist Security Info=True;Max Database Size=4000";
		}

		public override void CreateViewModels()
		{
			ServiceFactory.Layout.AddAlarmGroups(new AlarmGroupsViewModel());
			ServiceFactory.Layout.AddToolbarItem(new GKConnectionIndicatorViewModel());
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Subscribe(OnShowXDeviceDetails);
			ServiceFactory.Events.GetEvent<ShowXJournalEvent>().Subscribe(OnShowJournal);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournalRecord);
			DevicesViewModel = new DevicesViewModel();
			DeviceParametersViewModel = new DeviceParametersViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
			JournalsViewModel = new JournalsViewModel();
			ArchiveViewModel = new ArchiveViewModel();
			AlarmsViewModel = new AlarmsViewModel();
			ServiceFactory.Events.GetEvent<ShowXAlarmsEvent>().Subscribe(OnShowAlarms);
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Subscribe(OnShowArchive);
		}

		void OnShowAlarms(XAlarmType? alarmType)
		{
			AlarmsViewModel.Sort(alarmType);
		}

		void OnShowArchive(ShowXArchiveEventArgs showXArchiveEventArgs)
		{
			if (showXArchiveEventArgs != null)
			{
				ArchiveViewModel.Sort(showXArchiveEventArgs);
			}
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
		void OnNewJournalRecord(List<JournalItem> journalItems)
		{
			if (_journalNavigationItem == null || !_journalNavigationItem.IsSelected)
				UnreadJournalCount += journalItems.Count;
		}

		void OnShowXDeviceDetails(Guid deviceUID)
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(deviceUID));
		}

		public override void Initialize()
		{
			_planPresenter.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan>>().Publish(_planPresenter);
			_zonesNavigationItem.IsVisible = XManager.Zones.Count > 0;
			_directionsNavigationItem.IsVisible = XManager.Directions.Count > 0;
			DevicesViewModel.Initialize();
			DeviceParametersViewModel.Initialize();
			ZonesViewModel.Initialize();
			DirectionsViewModel.Initialize();
			JournalsViewModel.Initialize();
			ArchiveViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_zonesNavigationItem = new NavigationItem<ShowXZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty);
			_directionsNavigationItem = new NavigationItem<ShowXDirectionEvent, Guid>(DirectionsViewModel, "Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty);
			_journalNavigationItem = new NavigationItem<ShowXJournalEvent>(JournalsViewModel, "Журнал событий", "/Controls;component/Images/book.png");
			UnreadJournalCount = 0;

			return new List<NavigationItem>()
				{
					new NavigationItem<ShowXAlarmsEvent, XAlarmType?>(AlarmsViewModel, "Состояния", "/Controls;component/Images/Alarm.png") { SupportMultipleSelect = true},
					new NavigationItem<ShowXDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowXDeviceParametersEvent, Guid>(DeviceParametersViewModel, "Измерения", "/Controls;component/Images/AllParameters.png", null, null, Guid.Empty),
					_zonesNavigationItem,
					_directionsNavigationItem,
					_journalNavigationItem,
					new NavigationItem<ShowXArchiveEvent, ShowXArchiveEventArgs>(ArchiveViewModel, "Архив", "/Controls;component/Images/archive.png")
				};
		}

		public override string Name
		{
			get { return "Групповой контроллер"; }
		}

		#region IReportProviderModule Members
		public IEnumerable<IReportProvider> GetReportProviders()
		{
			return new List<IReportProvider>()
			{
                new DriverCounterReport(),
                new DeviceListReport(),
				new JournalReport(),
#if DEBUG
				new DeviceParametersReport()
#endif
			};
		}
		#endregion

		public override bool BeforeInitialize(bool firstTime)
		{
			LoadingService.DoStep("Загрузка конфигурации ГК");
            XManager.UpdateConfiguration();
			XManager.CreateStates();
			DatabaseManager.Convert();
			if (firstTime)
				UsersWatchManager.OnUserChanged(new UserChangedEventArgs() { IsReconnect = false });
			return true;
		}

		public override void AfterInitialize()
		{
			AlarmsViewModel.SubscribeShortcuts();
			WatcherManager.Start();
			UsersWatchManager.Start();
			AutoActivationWatcher.Run();
			JournalsViewModel.GetTopLast();
		}
	}
}