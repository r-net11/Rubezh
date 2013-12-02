using System;
using System.Linq;
using System.Collections.Generic;
using GKProcessor;
using FiresecAPI.Models;
using FiresecClient;
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
using XFiresecAPI;
using Infrastructure.Common.Services;

namespace GKModule
{
	public class GKModuleLoader : ModuleBase, IReportProviderModule, ILayoutProviderModule
	{
		static DevicesViewModel DevicesViewModel;
		static ZonesViewModel ZonesViewModel;
		static DirectionsViewModel DirectionsViewModel;
		static DelaysViewModel DelaysViewModel;
		NavigationItem _journalNavigationItem;
		static JournalsViewModel JournalsViewModel;
		static ArchiveViewModel ArchiveViewModel;
		static AlarmsViewModel AlarmsViewModel;
		NavigationItem _zonesNavigationItem;
		NavigationItem _directionsNavigationItem;
		NavigationItem _delaysNavigationItem;
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
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Unsubscribe(OnShowXDeviceDetails);
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Subscribe(OnShowXDeviceDetails);
			ServiceFactory.Events.GetEvent<ShowXJournalEvent>().Unsubscribe(OnShowJournal);
			ServiceFactory.Events.GetEvent<ShowXJournalEvent>().Subscribe(OnShowJournal);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Unsubscribe(OnNewJournalRecord);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournalRecord);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
			DelaysViewModel = new DelaysViewModel();
			JournalsViewModel = new JournalsViewModel();
			ArchiveViewModel = new ArchiveViewModel();
			AlarmsViewModel = new AlarmsViewModel();
			ServiceFactory.Events.GetEvent<ShowXAlarmsEvent>().Unsubscribe(OnShowAlarms);
			ServiceFactory.Events.GetEvent<ShowXAlarmsEvent>().Subscribe(OnShowAlarms);
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Unsubscribe(OnShowArchive);
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Subscribe(OnShowArchive);
			ServiceFactory.Events.GetEvent<ShowGKDebugEvent>().Unsubscribe(OnShowGKDebug);
			ServiceFactory.Events.GetEvent<ShowGKDebugEvent>().Subscribe(OnShowGKDebug);
		}

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
			_delaysNavigationItem.IsVisible = false;
#if DEBUG
			_delaysNavigationItem.IsVisible = true;
#endif
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DirectionsViewModel.Initialize();
			DelaysViewModel.Initialize();
			JournalsViewModel.Initialize();
			ArchiveViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_zonesNavigationItem = new NavigationItem<ShowXZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty);
			_directionsNavigationItem = new NavigationItem<ShowXDirectionEvent, Guid>(DirectionsViewModel, "Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty);
			_delaysNavigationItem = new NavigationItem<ShowXDelayEvent, Guid>(DelaysViewModel, "Задержки", "/Controls;component/Images/Watch.png", null, null, Guid.Empty);
			_journalNavigationItem = new NavigationItem<ShowXJournalEvent>(JournalsViewModel, "Журнал событий", "/Controls;component/Images/book.png");
			UnreadJournalCount = 0;

			return new List<NavigationItem>()
				{
					new NavigationItem<ShowXAlarmsEvent, XAlarmType?>(AlarmsViewModel, "Состояния", "/Controls;component/Images/Alarm.png") { SupportMultipleSelect = true},
					new NavigationItem<ShowXDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					_zonesNavigationItem,
					_directionsNavigationItem,
					_delaysNavigationItem,
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
			if (!GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				WatcherManager.LastConfigurationReloadingTime = DateTime.Now;
				WatcherManager.IsConfigurationReloading = true;
			}
			LoadingService.DoStep("Загрузка конфигурации ГК");
			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DescriptorsManager.Create();
			foreach (var gkDatabase in DescriptorsManager.GkDatabases)
			{
				foreach (var delay in gkDatabase.Delays)
				{
					delay.DelayState = new XDelayState();
				}
			}
			if (!GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				WatcherManager.IsConfigurationReloading = false;
			}
			if (firstTime)
				UsersWatchManager.OnUserChanged(new UserChangedEventArgs() { IsReconnect = false });
			return true;
		}

		public override void AfterInitialize()
		{
			AlarmsViewModel.SubscribeShortcuts();
			if (!GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				WatcherManager.Start();
			}
			UsersWatchManager.Start();
			AutoActivationWatcher.Run();
			JournalsViewModel.GetTopLast();

			FiresecManager.FiresecService.NewJournalItems -= new Action<List<JournalItem>>(OnNewJournalItems);
			FiresecManager.FiresecService.NewJournalItems += new Action<List<JournalItem>>(OnNewJournalItems);

			if (!GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				GKProcessorManager.StartProgress -= new Action<string, int, bool>(OnStartProgress);
				GKProcessorManager.StartProgress += new Action<string, int, bool>(OnStartProgress);

				GKProcessorManager.DoProgress -= new Action<string>(OnDoProgress);
				GKProcessorManager.DoProgress += new Action<string>(OnDoProgress);

				GKProcessorManager.StopProgress -= new Action(OnStopProgress);
				GKProcessorManager.StopProgress += new Action(OnStopProgress);

				GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
				GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);
			}
			ServiceFactory.SubscribeGKEvents();
		}

		void OnNewJournalItems(List<JournalItem> journalItems)
		{
			ApplicationService.Invoke(() =>
			{
				ServiceFactory.Events.GetEvent<NewXJournalEvent>().Publish(journalItems);
			});
		}

		void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				if (gkCallbackResult.JournalItems.Count > 0)
				{
					ServiceFactory.Events.GetEvent<NewXJournalEvent>().Publish(gkCallbackResult.JournalItems);
				}
				if (gkCallbackResult.DeviceStates.Count > 0)
				{
					foreach (var remoteDeviceState in gkCallbackResult.DeviceStates)
					{
						var device = XManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
						if (device != null)
						{
							device.DeviceState.StateClasses = remoteDeviceState.StateClasses;
							device.DeviceState.StateClass = remoteDeviceState.StateClass;
							device.DeviceState.OnDelay = remoteDeviceState.OnDelay;
							device.DeviceState.HoldDelay = remoteDeviceState.HoldDelay;
							device.DeviceState.OffDelay = remoteDeviceState.OffDelay;
							device.DeviceState.OnStateChanged();
						}
					}
				}
				if (gkCallbackResult.ZoneStates.Count > 0)
				{
					foreach (var remoteZoneState in gkCallbackResult.ZoneStates)
					{
						var zone = XManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
						if (zone != null)
						{
							zone.ZoneState.StateClasses = remoteZoneState.StateClasses;
							zone.ZoneState.StateClass = remoteZoneState.StateClass;
							zone.ZoneState.OnDelay = remoteZoneState.OnDelay;
							zone.ZoneState.HoldDelay = remoteZoneState.HoldDelay;
							zone.ZoneState.OffDelay = remoteZoneState.OffDelay;
							zone.ZoneState.OnStateChanged();
						}
					}
				}
				if (gkCallbackResult.DirectionStates.Count > 0)
				{
					foreach (var remoteDirectionState in gkCallbackResult.DirectionStates)
					{
						var direction = XManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
						if (direction != null)
						{
							direction.DirectionState.StateClasses = remoteDirectionState.StateClasses;
							direction.DirectionState.StateClass = remoteDirectionState.StateClass;
							direction.DirectionState.OnDelay = remoteDirectionState.OnDelay;
							direction.DirectionState.HoldDelay = remoteDirectionState.HoldDelay;
							direction.DirectionState.OffDelay = remoteDirectionState.OffDelay;
							direction.DirectionState.OnStateChanged();
						}
					}
				}
				ServiceFactoryBase.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null);
			});
		}

		void OnStartProgress(string name, int count, bool canCancel = true)
		{
			ApplicationService.Invoke(() =>
			{
				LoadingService.Show(name, name, count, canCancel);
			});
		}

		void OnDoProgress(string name)
		{
			ApplicationService.Invoke(() =>
			{
				LoadingService.DoStep(name);
			});
		}

		void OnStopProgress()
		{
			ApplicationService.Invoke(() =>
			{
				LoadingService.Close();
			});
		}

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter()
			{
				Name = "Состояния",
				UID = LayoutPartIdentities.Alarms,
				IconSource = "/Controls;component/Images/Alarm.png",
				Content = AlarmsViewModel,
			};
			yield return new LayoutPartPresenter()
			{
				Name = "Устройства",
				UID = LayoutPartIdentities.GDevices,
				IconSource = "/Controls;component/Images/Tree.png",
				Content = DevicesViewModel,
			};
			//yield return new LayoutPartPresenter()
			//{
			//    Name = "Измерения",
			//    UID = LayoutPartIdentities.DeviceParameters,
			//    IconSource = "/Controls;component/Images/AllParameters.png",
			//    Content = DeviceParametersViewModel,
			//};
			yield return new LayoutPartPresenter()
			{
				Name = "Зоны",
				UID = LayoutPartIdentities.Zones,
				IconSource = "/Controls;component/Images/Zones.png",
				Content = ZonesViewModel,
			};
			yield return new LayoutPartPresenter()
			{
				Name = "Направления",
				UID = LayoutPartIdentities.Directions,
				IconSource = "/Controls;component/Images/Direction.png",
				Content = DirectionsViewModel,
			};
			yield return new LayoutPartPresenter()
			{
				Name = "Журнал событий",
				UID = LayoutPartIdentities.Journals,
				IconSource = "/Controls;component/Images/Book.png",
				Content = JournalsViewModel,
			};
			yield return new LayoutPartPresenter()
			{
				Name = "Архив",
				UID = LayoutPartIdentities.Archive,
				IconSource = "/Controls;component/Images/Archive.png",
				Content = ArchiveViewModel,
			};
		}

		#endregion
	}
}