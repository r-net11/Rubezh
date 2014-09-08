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
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using SKDModule.Events;
using SKDModule.PassCardDesigner.ViewModels;
using SKDModule.Plans;
using SKDModule.ViewModels;
using Infrastructure.Designer;

namespace SKDModule
{
	public class SKDModuleLoader : ModuleBase, ILayoutProviderModule
	{
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		DoorsViewModel DoorsViewModel;
		HRViewModel HRViewModel;
		DayIntervalsViewModel DayIntervalsViewModel;
		ScheduleSchemesWeeklyViewModel ScheduleSchemesWeeklyViewModel;
		ScheduleSchemesMonthlyViewModel ScheduleSchemesMonthlyViewModel;
		ScheduleSchemesSlideViewModel ScheduleSchemesSlideViewModel;
		HolidaysViewModel HolidaysViewModel;
		SchedulesViewModel SchedulesViewModel;
		TimeTrackingViewModel _timeTrackingViewModel;
		PassCardsDesignerViewModel PassCardDesignerViewModel;
		PlanPresenter _planPresenter;

		public SKDModuleLoader()
		{
			_planPresenter = new PlanPresenter();
		}

		public override void CreateViewModels()
		{
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DoorsViewModel = new DoorsViewModel();
			HRViewModel = new HRViewModel();
			DayIntervalsViewModel = new DayIntervalsViewModel();
			ScheduleSchemesWeeklyViewModel = new ScheduleSchemesWeeklyViewModel();
			ScheduleSchemesSlideViewModel = new ScheduleSchemesSlideViewModel();
			ScheduleSchemesMonthlyViewModel = new ScheduleSchemesMonthlyViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			SchedulesViewModel = new SchedulesViewModel();
			PassCardDesignerViewModel = new PassCardsDesignerViewModel();
			_timeTrackingViewModel = new TimeTrackingViewModel();

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
				new NavigationItem("СКД", "/Controls;component/Images/SKDW.png",
					new List<NavigationItem>()
					{
						new NavigationItem<ShowSKDDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/Tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/Zones.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDDoorEvent, Guid>(DoorsViewModel, "Точки доступа", "/Controls;component/Images/DoorW.png", null, null, Guid.Empty),
						new NavigationItem<ShowPassCardDesignerEvent, Guid>(PassCardDesignerViewModel, "Дизайнер пропусков", "/Controls;component/Images/PassCardDesigner.png", null, null, Guid.Empty),
						new NavigationItem<ShowHREvent>(HRViewModel, "Картотека", "/Controls;component/Images/Kartoteka2W.png"),
						new NavigationItem("Учет рабочего времени", "/Controls;component/Images/TimeTrackingW.png", new List<NavigationItem>()
						{
							new NavigationItem<ShowTimeIntervalsEvent, Guid>(DayIntervalsViewModel, "Дневные графики", "/Controls;component/Images/ShedulesDaylyW.png", null, null, Guid.Empty),
							new NavigationItem<ShowWeeklyIntervalsEvent, Guid>(ScheduleSchemesWeeklyViewModel, "Недельные графики", "/Controls;component/Images/SheduleWeeklyW.png", null, null, Guid.Empty),
							new NavigationItem<ShowSlideDayIntervalsEvent, Guid>(ScheduleSchemesSlideViewModel, "Суточные графики", "/Controls;component/Images/SheduleSlideDaylyW.png", null, null, Guid.Empty),
							new NavigationItem<ShowMonthlyIntervalsEvent, Guid>(ScheduleSchemesMonthlyViewModel, "Месячные графики", "/Controls;component/Images/ShedulesMonthlyW.png", null, null, Guid.Empty),
							new NavigationItem<ShowHolidaysEvent, Guid>(HolidaysViewModel, "Праздничные дни", "/Controls;component/Images/HolidaysW.png", null, null, Guid.Empty),
							new NavigationItem<ShowShedulesEvent, Guid>(SchedulesViewModel, "Графики работ", "/Controls;component/Images/ShedulesW.png", null, null, Guid.Empty),
							new NavigationItem<ShowTimeTrackingEvent>(_timeTrackingViewModel, "Учет рабочего времени", "/Controls;component/Images/TimeTrackingW.png", null, null),
						}),
					})
				};
		}

		public override void Initialize()
		{
			_planPresenter.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(_planPresenter);
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DoorsViewModel.Initialize();
			PassCardDesignerViewModel.Initialize();
		}

		public override string Name
		{
			get { return "СКД"; }
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
			DesignerLoader.RegisterResource();
		}

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
			AutoActivationWatcher.Run();
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
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDevices, "СКД устройства", "Tree.png", (p) => DevicesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDZones, "СКД зоны", "Tree.png", (p) => ZonesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDHR, "Картотека", "Levels.png", (p) => HRViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDVerification, "Верификация", "Tree.png", (p) => new VerificationViewModel(p as LayoutPartReferenceProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDayIntervals, "Дневные графики", "Tree.png", (p) => DayIntervalsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDWeeklyScheduleSchemes, "Недельные графики", "Tree.png", (p) => ScheduleSchemesWeeklyViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDaylyScheduleSchemes, "Суточные графики", "Tree.png", (p) => ScheduleSchemesSlideViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDMonthlyScheduleSchemes, "Месячные графики", "BTree.png", (p) => ScheduleSchemesMonthlyViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDHolidays, "Праздничные дни", "Tree.png", (p) => HolidaysViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDTimeTracking, "Учета рабочего времени", "Tree.png", (p) => _timeTrackingViewModel);
		}

		#endregion
	}
}