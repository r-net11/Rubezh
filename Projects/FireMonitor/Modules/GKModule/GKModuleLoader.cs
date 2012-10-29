using System;
using System.Collections.Generic;
using Common.GK;
using FiresecClient;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule
{
	public class GKModuleLoader : ModuleBase
	{
		static DevicesViewModel DevicesViewModel;
		static ZonesViewModel ZonesViewModel;
        static DirectionsViewModel DirectionsViewModel;
        NavigationItem _journalNavigationItem;
		static JournalsViewModel JournalsViewModel;
        static ArchiveViewModel ArchiveViewModel;
        static AlarmsViewModel AlarmsViewModel;
		NavigationItem _zonesNavigationItem;
		NavigationItem _directionsNavigationItem;

		public GKModuleLoader()
		{
			ServiceFactory.Layout.AddAlarmGroups(new AlarmGroupsViewModel());
			ServiceFactory.Layout.AddToolbarItem(new GKConnectionIndicatorViewModel());
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Subscribe(OnShowXDeviceDetails);
            ServiceFactory.Events.GetEvent<ShowXJournalEvent>().Subscribe(OnShowJournal);
            ServiceFactory.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournalRecord);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
            JournalsViewModel = new JournalsViewModel();
            ArchiveViewModel = new ArchiveViewModel();
            AlarmsViewModel = new AlarmsViewModel();
			ServiceFactory.Events.GetEvent<ShowXAlarmsEvent>().Subscribe(OnShowAlarms);
		}

		void OnShowAlarms(XAlarmType? alarmType)
		{
			AlarmsViewModel.Sort(alarmType);
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
			_zonesNavigationItem.IsVisible = XManager.DeviceConfiguration.Zones.Count > 0;
			_directionsNavigationItem.IsVisible = XManager.DeviceConfiguration.Directions.Count > 0;
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
            DirectionsViewModel.Initialize();
			JournalsViewModel.Initialize();
            ArchiveViewModel.Initialize();

            JournalWatcherManager.Start();
            JournalWatcherManager.GetLastJournalItems(100);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_zonesNavigationItem = new NavigationItem<ShowXZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty);
			_directionsNavigationItem = new NavigationItem<ShowXDirectionEvent, Guid>(DirectionsViewModel, "Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty);
            _journalNavigationItem = new NavigationItem<ShowXJournalEvent>(JournalsViewModel, "Журнал событий", "/Controls;component/Images/book.png");
            UnreadJournalCount = 0;
			return new List<NavigationItem>()
			{
				new NavigationItem("ГК", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowXAlarmsEvent, XAlarmType?>(AlarmsViewModel, "Состояния", "/Controls;component/Images/Alarm.png") { SupportMultipleSelect = true},
					new NavigationItem<ShowXDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					_zonesNavigationItem,
					_directionsNavigationItem,
					_journalNavigationItem,
                    new NavigationItem<ShowXArchiveEvent, object>(ArchiveViewModel, "Архив", "/Controls;component/Images/archive.png")
				}),
			};
		}

		public override string Name
		{
			get { return "Групповой контроллер"; }
		}
	}
}