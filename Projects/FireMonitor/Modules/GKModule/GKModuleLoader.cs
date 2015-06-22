using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Reports;
using GKModule.ViewModels;
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
		public static DaySchedulesViewModel DaySchedulesViewModel;
		public static SchedulesViewModel SchedulesViewModel;
		NavigationItem _skdZonesNavigationItem;
		NavigationItem _doorsNavigationItem;

		public GKModuleLoader()
		{
		}

		public override void CreateViewModels()
		{
			//ServiceFactory.Layout.AddToolbarItem(new GKConnectionIndicatorViewModel());

			//DevicesViewModel = new DevicesViewModel();
			//SKDZonesViewModel = new SKDZonesViewModel();
			//DoorsViewModel = new DoorsViewModel();
			//DaySchedulesViewModel = new DaySchedulesViewModel();
			//SchedulesViewModel = new SchedulesViewModel();

			//SubscribeShowDelailsEvent();
		}

		#region ShowDelailsEvent
		void SubscribeShowDelailsEvent()
		{
			ServiceFactory.Events.GetEvent<ShowGKDeviceDetailsEvent>().Unsubscribe(OnShowDeviceDetails);

			ServiceFactory.Events.GetEvent<ShowGKDeviceDetailsEvent>().Subscribe(OnShowDeviceDetails);
		}

		void OnShowDeviceDetails(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				DialogService.ShowWindow(new DeviceDetailsViewModel(device));
			}
		}
		#endregion

		public override void Initialize()
		{
			//_planPresenter.Initialize();
			//ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(_planPresenter);
			//_skdZonesNavigationItem.IsVisible = GKManager.DeviceConfiguration.SKDZones.Count > 0;
			//DevicesViewModel.Initialize();
			//SKDZonesViewModel.Initialize();
			//DoorsViewModel.Initialize();
			//_doorsNavigationItem.IsVisible = GKManager.DeviceConfiguration.Doors.Count > 0;
			//DaySchedulesViewModel.Initialize();
			//SchedulesViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			//_skdZonesNavigationItem = new NavigationItem<ShowGKSKDZoneEvent, Guid>(SKDZonesViewModel, "Зоны СКД", "Zones", null, null, Guid.Empty);
			//_doorsNavigationItem = new NavigationItem<ShowGKDoorEvent, Guid>(DoorsViewModel, "Точки доступа", "DoorW", null, null, Guid.Empty);

			//return new List<NavigationItem>
			//	{
			//	new NavigationItem(ModuleType.ToDescription(), "tree",
			//		new List<NavigationItem>()
			//		{
			//			new NavigationItem<ShowGKDeviceEvent, Guid>(DevicesViewModel, "Устройства", "Tree", null, null, Guid.Empty),
			//			_skdZonesNavigationItem,
			//			_doorsNavigationItem,
			//			new NavigationItem("СКД", "tree",
			//				new List<NavigationItem>()
			//				{
			//					new NavigationItem<ShowGKDaySchedulesEvent, Guid>(DaySchedulesViewModel, "Дневные графики", "ShedulesDaylyW", null, null, Guid.Empty),
			//					new NavigationItem<ShowGKScheduleEvent, Guid>(SchedulesViewModel, "Графики", "ShedulesW", null, null, Guid.Empty),
			//				}) { IsVisible = FiresecManager.CheckPermission(PermissionType.Oper_GKSchedules) && GKManager.Doors.Count > 0 },
			//		})
			//};
			return new List<NavigationItem>();
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.SKD; }
		}

		#region IReportProviderModule Members
		public IEnumerable<IReportProvider> GetReportProviders()
		{
			return new List<IReportProvider>()
			{
				new DriverCounterReport(),
				new DeviceListReport(),
#if DEBUG
				new DeviceParametersReport()
#endif
			};
		}
		#endregion

		public override bool BeforeInitialize(bool firstTime)
		{
		//	SubscribeGK();
			return true;
		}

		public override void AfterInitialize()
		{
		//	GKAfterInitialize();
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.GDevices, "Устройства", "Tree.png", (p) => DevicesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.ConnectionIndicator, "Индикатор связи", "ConnectionIndicator.png", (p) => new GKConnectionIndicatorViewModel());
		}
		#endregion
	}
}