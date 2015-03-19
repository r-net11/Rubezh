using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.Windows;
using Infrastructure.Designer;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using SKDModule.Events;
using SKDModule.Plans;
using SKDModule.Reports;
using SKDModule.Reports.Providers;
using SKDModule.ViewModels;

namespace SKDModule
{
	public class SKDModuleLoader : ModuleBase, IReportProviderModule, ILayoutProviderModule, ISKDReportProviderModule
	{
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		DoorsViewModel DoorsViewModel;
		HRViewModel HRViewModel;
		DayIntervalsViewModel DayIntervalsViewModel;
		ScheduleSchemesViewModel ScheduleSchemesViewModel;
		HolidaysViewModel HolidaysViewModel;
		SchedulesViewModel SchedulesViewModel;
		TimeTrackingViewModel TimeTrackingViewModel;
		PlanPresenter PlanPresenter;

		public SKDModuleLoader()
		{
			PlanPresenter = new PlanPresenter();
		}

		public override void CreateViewModels()
		{
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DoorsViewModel = new DoorsViewModel();
			HRViewModel = new HRViewModel();
			DayIntervalsViewModel = new DayIntervalsViewModel();
			ScheduleSchemesViewModel = new ScheduleSchemesViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			SchedulesViewModel = new SchedulesViewModel();
			TimeTrackingViewModel = new TimeTrackingViewModel();

			SubscribeShowDelailsEvent();
		}

		#region ShowDelailsEvent
		void SubscribeShowDelailsEvent()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDeviceDetailsEvent>().Unsubscribe(OnShowDeviceDetails);
			ServiceFactory.Events.GetEvent<ShowSKDZoneDetailsEvent>().Unsubscribe(OnShowZoneDetails);
			ServiceFactory.Events.GetEvent<ShowSKDDoorDetailsEvent>().Unsubscribe(OnShowDoorDetails);

			ServiceFactory.Events.GetEvent<ShowSKDDeviceDetailsEvent>().Subscribe(OnShowDeviceDetails);
			ServiceFactory.Events.GetEvent<ShowSKDZoneDetailsEvent>().Subscribe(OnShowZoneDetails);
			ServiceFactory.Events.GetEvent<ShowSKDDoorDetailsEvent>().Subscribe(OnShowDoorDetails);
		}

		void OnShowDeviceDetails(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				DialogService.ShowWindow(new DeviceDetailsViewModel(device));
			}
		}

		void OnShowZoneDetails(Guid zoneUID)
		{
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				DialogService.ShowWindow(new ZoneDetailsViewModel(zone));
			}
		}

		void OnShowDoorDetails(Guid doorUID)
		{
			var direction = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
			if (direction != null)
			{
				DialogService.ShowWindow(new DoorDetailsViewModel(direction));
			}
		}
		#endregion

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
				{
				new NavigationItem("���", "SKDW",
					new List<NavigationItem>()
					{
						new NavigationItem<ShowSKDDeviceEvent, Guid>(DevicesViewModel, "����������", "Tree", null, PermissionType.Oper_Strazh_Devices_View, Guid.Empty) {IsVisible = SKDManager.Devices.Count > 1},
						new NavigationItem<ShowSKDZoneEvent, Guid>(ZonesViewModel, "����", "Zones", null, PermissionType.Oper_Strazh_Zones_View, Guid.Empty) {IsVisible = SKDManager.Zones.Count > 0},
						new NavigationItem<ShowSKDDoorEvent, Guid>(DoorsViewModel, "����� �������", "DoorW", null, PermissionType.Oper_Strazh_Doors_View, Guid.Empty) {IsVisible = SKDManager.Doors.Count > 0},
						new NavigationItem<ShowHREvent>(HRViewModel, "���������", "Kartoteka2W"),
						new NavigationItem("���� �������� �������", "TimeTrackingW", new List<NavigationItem>()
						{
							new NavigationItem<ShowTimeIntervalsEvent, Guid>(DayIntervalsViewModel, "������� �������", "ShedulesDaylyW", null, PermissionType.Oper_SKD_TimeTrack_DaySchedules_View, Guid.Empty),
							new NavigationItem<ShowWeeklyIntervalsEvent, Guid>(ScheduleSchemesViewModel, "�������", "SheduleWeeklyW", null, PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_View, Guid.Empty),
							new NavigationItem<ShowHolidaysEvent, Guid>(HolidaysViewModel, "����������� ���", "HolidaysW", null, PermissionType.Oper_SKD_TimeTrack_Holidays_View, Guid.Empty),
							new NavigationItem<ShowShedulesEvent, Guid>(SchedulesViewModel, "������� �����", "ShedulesW", null, PermissionType.Oper_SKD_TimeTrack_Schedules_View, Guid.Empty),
							new NavigationItem<ShowTimeTrackingEvent>(TimeTrackingViewModel, "���� �������� �������", "TimeTrackingW", null, PermissionType.Oper_SKD_TimeTrack_Report_View),
						}),
					})
				};
		}

		public override void Initialize()
		{
			PlanPresenter.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(PlanPresenter);
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DoorsViewModel.Initialize();

			DayIntervalsViewModel.Initialize();
			ScheduleSchemesViewModel.Initialize();
			HolidaysViewModel.Initialize();
			SchedulesViewModel.Initialize();
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.SKD; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Doors/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "HR/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Employees/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Departments/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Positions/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "AdditionalColumns/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "AccessTemplates/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Organisations/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Verification/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Cards/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "TimeTrack/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "PassCard/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "PassCardDesigner/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Reports/DataTemplates/Dictionary.xaml"));
			DesignerLoader.RegisterResource();
		}

		#region IReportProviderModule Members
		public IEnumerable<IReportProvider> GetReportProviders()
		{
			return new List<IReportProvider>()
			{
				new DeviceListReport(),
			};
		}
		#endregion

		public override bool BeforeInitialize(bool firstTime)
		{
			SKDManager.CreateStates();
			var operationResult = FiresecManager.FiresecService.SKDGetStates();
			if (!operationResult.HasError)
				CopySKDStates(operationResult.Result);
			return true;
		}
		public override void AfterInitialize()
		{
			SafeFiresecService.SKDStatesEvent -= new Action<SKDStates>(OnSKDStates);
			SafeFiresecService.SKDStatesEvent += new Action<SKDStates>(OnSKDStates);

			ServiceFactoryBase.Events.GetEvent<SKDObjectsStateChangedEvent>().Publish(null);
		}

		void OnSKDStates(SKDStates skdStates)
		{
			ApplicationService.Invoke(() =>
			{
				CopySKDStates(skdStates);
				ServiceFactoryBase.Events.GetEvent<SKDObjectsStateChangedEvent>().Publish(null);
			});
		}
		void CopySKDStates(SKDStates skdStates)
		{
			foreach (var remoteDeviceState in skdStates.DeviceStates)
				if (remoteDeviceState != null)
				{
					var device = SKDManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
					if (device != null)
					{
						device.State.StateClasses = remoteDeviceState.StateClasses.ToList();
						device.State.StateClass = remoteDeviceState.StateClass;
						device.State.OpenAlwaysTimeIndex = remoteDeviceState.OpenAlwaysTimeIndex;
						device.State.OnStateChanged();
					}
				}
			foreach (var remoteZoneState in skdStates.ZoneStates)
			{
				if (remoteZoneState != null)
				{
					var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
					if (zone != null)
					{
						zone.State.StateClasses = remoteZoneState.StateClasses.ToList();
						zone.State.StateClass = remoteZoneState.StateClass;
						zone.State.OnStateChanged();
					}
				}
			}
			foreach (var remoteDoorState in skdStates.DoorStates)
			{
				if (remoteDoorState != null)
				{
					var door = SKDManager.Doors.FirstOrDefault(x => x.UID == remoteDoorState.UID);
					if (door != null)
					{
						door.State.StateClasses = remoteDoorState.StateClasses.ToList();
						door.State.StateClass = remoteDoorState.StateClass;
						door.State.OnStateChanged();
					}
				}
			}
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDevices, "��� ����������", "Tree.png", (p) => DevicesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDZones, "��� ����", "Tree.png", (p) => ZonesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDoors, "��� ����� �������", "Tree.png", (p) => DoorsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDHR, "���������", "Levels.png", (p) => HRViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDVerification, "�����������", "Tree.png", (p) => new VerificationViewModel(p as LayoutPartReferenceProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDayIntervals, "������� �������", "Tree.png", (p) => DayIntervalsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDScheduleSchemes, "�������", "Tree.png", (p) => ScheduleSchemesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDHolidays, "����������� ���", "Tree.png", (p) => HolidaysViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDSchedules, "������� �����", "Tree.png", (p) => SchedulesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDTimeTracking, "����� �������� �������", "Tree.png", (p) => TimeTrackingViewModel);
		}
		#endregion

		#region ISKDReportProviderModule Members

		public IEnumerable<ISKDReportProvider> GetSKDReportProviders()
		{
			yield return new EventsReportProvider();
			yield return new EmployeeRootReportProvider();
			yield return new CardsReportProvider();
			yield return new EmployeeAccessReportProvider();
			yield return new EmployeeDoorsReportProvider();
			yield return new DepartmentsReportProvider();
			yield return new PositionsReportProvider();
			yield return new EmployeeZonesReportProvider();
			yield return new EmployeeReportProvider();
			yield return new DisciplineReportProvider();
			yield return new SchedulesReportProvider();
			yield return new DocumentsReportProvider();
			yield return new WorkingTimeReportProvider();
			yield return new DoorsReportProvider();
		}

		#endregion
	}
}