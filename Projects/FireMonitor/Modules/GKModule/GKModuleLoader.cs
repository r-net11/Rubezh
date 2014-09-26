using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans;
using GKModule.Reports;
using GKModule.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;

namespace GKModule
{
	public partial class GKModuleLoader : ModuleBase, IReportProviderModule, ILayoutProviderModule
	{
		static DevicesViewModel DevicesViewModel;
		static ZonesViewModel ZonesViewModel;
		static GuardZonesViewModel GuardZonesViewModel;
		static DirectionsViewModel DirectionsViewModel;
		static DelaysViewModel DelaysViewModel;
		static PimsViewModel PimsViewModel;
		static PumpStationsViewModel PumpStationsViewModel;
		static MPTsViewModel MPTsViewModel;
		static DoorsViewModel DoorsViewModel;
		static JournalsViewModel JournalsViewModel;
		static ArchiveViewModel ArchiveViewModel;
		static AlarmsViewModel AlarmsViewModel;
		NavigationItem _zonesNavigationItem;
		NavigationItem _guardZonesNavigationItem;
		NavigationItem _directionsNavigationItem;
		NavigationItem _delaysNavigationItem;
		NavigationItem _pimsNavigationItem;
		NavigationItem _pumpStationsNavigationItem;
		NavigationItem _mptsNavigationItem;
		NavigationItem _doorsNavigationItem;
		NavigationItem _journalNavigationItem;
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
			ServiceFactory.Events.GetEvent<ShowXJournalEvent>().Unsubscribe(OnShowJournal);
			ServiceFactory.Events.GetEvent<ShowXJournalEvent>().Subscribe(OnShowJournal);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Unsubscribe(OnNewJournalRecord);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournalRecord);

			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			GuardZonesViewModel = new GuardZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
			DelaysViewModel = new DelaysViewModel();
			PimsViewModel = new PimsViewModel();
			PumpStationsViewModel = new PumpStationsViewModel();
			MPTsViewModel = new MPTsViewModel();
			DoorsViewModel = new DoorsViewModel();
			JournalsViewModel = new JournalsViewModel();
			ArchiveViewModel = new ArchiveViewModel();
			AlarmsViewModel = new AlarmsViewModel();
			ServiceFactory.Events.GetEvent<ShowXAlarmsEvent>().Unsubscribe(OnShowAlarms);
			ServiceFactory.Events.GetEvent<ShowXAlarmsEvent>().Subscribe(OnShowAlarms);
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Unsubscribe(OnShowArchive);
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Subscribe(OnShowArchive);
			ServiceFactory.Events.GetEvent<ShowGKDebugEvent>().Unsubscribe(OnShowGKDebug);
			ServiceFactory.Events.GetEvent<ShowGKDebugEvent>().Subscribe(OnShowGKDebug);

			SubscribeShowDelailsEvent();
		}

		#region ShowDelailsEvent
		void SubscribeShowDelailsEvent()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Unsubscribe(OnShowDeviceDetails);
			ServiceFactory.Events.GetEvent<ShowXZoneDetailsEvent>().Unsubscribe(OnShowZoneDetails);
			ServiceFactory.Events.GetEvent<ShowXDirectionDetailsEvent>().Unsubscribe(OnShowDirectionDetails);
			ServiceFactory.Events.GetEvent<ShowXMPTDetailsEvent>().Unsubscribe(OnShowMPTDetails);
			ServiceFactory.Events.GetEvent<ShowXPumpStationDetailsEvent>().Unsubscribe(OnShowPumpStationDetails);
			ServiceFactory.Events.GetEvent<ShowXDelayDetailsEvent>().Unsubscribe(OnShowDelayDetails);
			ServiceFactory.Events.GetEvent<ShowXPIMDetailsEvent>().Unsubscribe(OnShowPIMDetails);
			ServiceFactory.Events.GetEvent<ShowXGuardZoneDetailsEvent>().Unsubscribe(OnShowGuardZoneDetails);

			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Subscribe(OnShowDeviceDetails);
			ServiceFactory.Events.GetEvent<ShowXZoneDetailsEvent>().Subscribe(OnShowZoneDetails);
			ServiceFactory.Events.GetEvent<ShowXDirectionDetailsEvent>().Subscribe(OnShowDirectionDetails);
			ServiceFactory.Events.GetEvent<ShowXMPTDetailsEvent>().Subscribe(OnShowMPTDetails);
			ServiceFactory.Events.GetEvent<ShowXPumpStationDetailsEvent>().Subscribe(OnShowPumpStationDetails);
			ServiceFactory.Events.GetEvent<ShowXDelayDetailsEvent>().Subscribe(OnShowDelayDetails);
			ServiceFactory.Events.GetEvent<ShowXPIMDetailsEvent>().Subscribe(OnShowPIMDetails);
			ServiceFactory.Events.GetEvent<ShowXGuardZoneDetailsEvent>().Subscribe(OnShowGuardZoneDetails);
		}

		void OnShowDeviceDetails(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				DialogService.ShowWindow(new DeviceDetailsViewModel(device));
			}
		}

		void OnShowZoneDetails(Guid zoneUID)
		{
			var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				DialogService.ShowWindow(new ZoneDetailsViewModel(zone));
			}
		}

		void OnShowDirectionDetails(Guid directinUID)
		{
			var direction = XManager.Directions.FirstOrDefault(x => x.UID == directinUID);
			if (direction != null)
			{
				DialogService.ShowWindow(new DirectionDetailsViewModel(direction));
			}
		}

		void OnShowMPTDetails(Guid mptUID)
		{
			var mpt = XManager.MPTs.FirstOrDefault(x => x.UID == mptUID);
			if (mpt != null)
			{
				DialogService.ShowWindow(new MPTDetailsViewModel(mpt));
			}
		}

		void OnShowPumpStationDetails(Guid pumpStationUID)
		{
			var pumpStation = XManager.PumpStations.FirstOrDefault(x => x.UID == pumpStationUID);
			if (pumpStation != null)
			{
				DialogService.ShowWindow(new PumpStationDetailsViewModel(pumpStation));
			}
		}

		void OnShowDelayDetails(Guid delayUID)
		{
			var delay = XManager.Delays.FirstOrDefault(x => x.UID == delayUID);
			if (delay != null && !delay.IsAutoGenerated)
			{
				DialogService.ShowWindow(new DelayDetailsViewModel(delay));
			}
		}

		void OnShowPIMDetails(Guid pimUID)
		{
			var pim = XManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == pimUID);
			if (pim != null)
			{
				DialogService.ShowWindow(new PimDetailsViewModel(pim));
			}
		}
		void OnShowGuardZoneDetails(Guid guardZoneUID)
		{
			var guardZone = XManager.GuardZones.FirstOrDefault(x => x.UID == guardZoneUID);
			if (guardZone != null)
			{
				DialogService.ShowWindow(new GuardZoneDetailsViewModel(guardZone));
			}
		}
		#endregion

		void OnShowGKDebug(object obj)
		{
			var diagnosticsViewModel = new DiagnosticsViewModel();
			DialogService.ShowWindow(diagnosticsViewModel);
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
		void OnNewJournalRecord(List<XJournalItem> journalItems)
		{
			if (_journalNavigationItem == null || !_journalNavigationItem.IsSelected)
				UnreadJournalCount += journalItems.Count;
		}

		public override void Initialize()
		{
			_planPresenter.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(_planPresenter);
			_zonesNavigationItem.IsVisible = XManager.Zones.Count > 0;
			_guardZonesNavigationItem.IsVisible = XManager.DeviceConfiguration.GuardZones.Count > 0;
			_directionsNavigationItem.IsVisible = XManager.Directions.Count > 0;
			_pumpStationsNavigationItem.IsVisible = XManager.PumpStations.Count > 0;
			_mptsNavigationItem.IsVisible = XManager.MPTs.Count > 0;
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			GuardZonesViewModel.Initialize();
			DirectionsViewModel.Initialize();
			PumpStationsViewModel.Initialize();
			MPTsViewModel.Initialize();
			DelaysViewModel.Initialize();
			_delaysNavigationItem.IsVisible = DelaysViewModel.Delays.Count > 0;
			PimsViewModel.Initialize();
			_pimsNavigationItem.IsVisible = PimsViewModel.Pims.Count > 0;
			DoorsViewModel.Initialize();
			_doorsNavigationItem.IsVisible = XManager.DeviceConfiguration.Doors.Count > 0;
			JournalsViewModel.Initialize();
			ArchiveViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_zonesNavigationItem = new NavigationItem<ShowXZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/Zones.png", null, null, Guid.Empty);
			_guardZonesNavigationItem = new NavigationItem<ShowXGuardZoneEvent, Guid>(GuardZonesViewModel, "Охранные зоны", "/Controls;component/Images/Zones.png", null, null, Guid.Empty);
			_directionsNavigationItem = new NavigationItem<ShowXDirectionEvent, Guid>(DirectionsViewModel, "Направления", "/Controls;component/Images/Direction.png", null, null, Guid.Empty);
			_delaysNavigationItem = new NavigationItem<ShowXDelayEvent, Guid>(DelaysViewModel, "Задержки", "/Controls;component/Images/Watch.png", null, null, Guid.Empty);
			_pimsNavigationItem = new NavigationItem<ShowXPimEvent, Guid>(PimsViewModel, "ПИМ", "/Controls;component/Images/Pim_White.png", null, null, Guid.Empty);
			_pumpStationsNavigationItem = new NavigationItem<ShowXPumpStationEvent, Guid>(PumpStationsViewModel, "НС", "/Controls;component/Images/PumpStation.png", null, null, Guid.Empty);
			_mptsNavigationItem = new NavigationItem<ShowXMPTEvent, Guid>(MPTsViewModel, "МПТ", "/Controls;component/Images/MPT.png", null, null, Guid.Empty);
			_doorsNavigationItem = new NavigationItem<ShowXDoorEvent, Guid>(DoorsViewModel, "Точки доступа", "/Controls;component/Images/DoorW.png", null, null, Guid.Empty);
			_journalNavigationItem = new NavigationItem<ShowXJournalEvent>(JournalsViewModel, "Журнал событий", "/Controls;component/Images/book.png");
			UnreadJournalCount = 0;

			return new List<NavigationItem>
				{
				new NavigationItem(ModuleType.ToDescription(), "/Controls;component/Images/tree.png",
					new List<NavigationItem>()
					{
						new NavigationItem<ShowXAlarmsEvent, XAlarmType?>(AlarmsViewModel, "Состояния", "/Controls;component/Images/Alarm.png") { SupportMultipleSelect = true},
						new NavigationItem<ShowXDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/Tree.png", null, null, Guid.Empty),
						_zonesNavigationItem,
						_guardZonesNavigationItem,
						_directionsNavigationItem,
						_delaysNavigationItem,
						_pimsNavigationItem,
						_pumpStationsNavigationItem,
						_mptsNavigationItem,
						_doorsNavigationItem,
						_journalNavigationItem,
						new NavigationItem<ShowXArchiveEvent, ShowXArchiveEventArgs>(ArchiveViewModel, "Архив", "/Controls;component/Images/Archive.png")
					})
			};
		}

		protected override ModuleType ModuleType
		{
			get { return Infrastructure.Common.ModuleType.GK; }
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
			SubscribeGK();
			return true;
		}

		public override void AfterInitialize()
		{
			GKAfterInitialize();
			AlarmsViewModel.SubscribeShortcuts();
			AutoActivationWatcher.Run();
			JournalsViewModel.GetTopLast();
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.Alarms, "Состояния", "Alarm.png", (p) => AlarmsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.GDevices, "Устройства", "Tree.png", (p) => DevicesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.Zones, "Зоны", "Zones.png", (p) => ZonesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.GuardZones, "Охранные зоны", "Zones.png", (p) => GuardZonesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.Directions, "Направления", "Direction.png", (p) => DirectionsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.PumpStations, "НС", "PumpStation.png", (p) => PumpStationsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.MPTs, "МПТ", "BMPT.png", (p) => MPTsViewModel);
		}
		#endregion
	}
}