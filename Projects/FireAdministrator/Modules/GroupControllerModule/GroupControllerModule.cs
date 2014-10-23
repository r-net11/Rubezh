using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans;
using GKModule.Validation;
using GKModule.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;

namespace GKModule
{
	public class GroupControllerModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		DevicesViewModel DevicesViewModel;
		ParameterTemplatesViewModel ParameterTemplatesViewModel;
		ZonesViewModel ZonesViewModel;
		DirectionsViewModel DirectionsViewModel;
		PumpStationsViewModel PumpStationsViewModel;
		MPTsViewModel MPTsViewModel;
		DelaysViewModel DelaysViewModel;
		CodesViewModel CodesViewModel;
		GuardZonesViewModel GuardZonesViewModel;
		DoorsViewModel DoorsViewModel;
		DaySchedulesViewModel DaySchedulesViewModel;
		SchedulesViewModel SchedulesViewModel;
		LibraryViewModel DeviceLidraryViewModel;
		InstructionsViewModel InstructionsViewModel;
		OPCDevicesViewModel OPCDevicesViewModel;
		OPCZonesViewModel OPCZonesViewModel;
		OPCDirectionsViewModel OPCDirectionsViewModel;
		DescriptorsViewModel DescriptorsViewModel;
		DiagnosticsViewModel DiagnosticsViewModel;
		GKPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<CreateGKZoneEvent>().Subscribe(OnCreateGKZone);
			ServiceFactory.Events.GetEvent<EditGKZoneEvent>().Subscribe(OnEditGKZone);
			ServiceFactory.Events.GetEvent<CreateGKGuardZoneEvent>().Subscribe(OnCreateGKGuardZone);
			ServiceFactory.Events.GetEvent<EditGKGuardZoneEvent>().Subscribe(OnEditGKGuardZone);
			ServiceFactory.Events.GetEvent<CreateGKDirectionEvent>().Subscribe(OnCreateGKDirection);
			ServiceFactory.Events.GetEvent<EditGKDirectionEvent>().Subscribe(OnEditGKDirection);
			ServiceFactory.Events.GetEvent<CreateGKDoorEvent>().Subscribe(OnCreateGKDoor);
			ServiceFactory.Events.GetEvent<EditGKDoorEvent>().Subscribe(OnEditGKDoor);

			DevicesViewModel = new DevicesViewModel();
			ParameterTemplatesViewModel = new ParameterTemplatesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
			PumpStationsViewModel = new PumpStationsViewModel();
			MPTsViewModel = new MPTsViewModel();
			DelaysViewModel = new DelaysViewModel();
			CodesViewModel = new CodesViewModel();
			GuardZonesViewModel = new GuardZonesViewModel();
			DoorsViewModel = new DoorsViewModel();
			DaySchedulesViewModel = new DaySchedulesViewModel();
			SchedulesViewModel = new SchedulesViewModel();
			DeviceLidraryViewModel = new LibraryViewModel();
			InstructionsViewModel = new InstructionsViewModel();
			OPCDevicesViewModel = new OPCDevicesViewModel();
			OPCZonesViewModel = new OPCZonesViewModel();
			OPCDirectionsViewModel = new OPCDirectionsViewModel();
			DescriptorsViewModel = new DescriptorsViewModel();
			DiagnosticsViewModel = new DiagnosticsViewModel();
			_planExtension = new GKPlanExtension(DevicesViewModel, ZonesViewModel, DirectionsViewModel, GuardZonesViewModel, DoorsViewModel);
		}

		public override void Initialize()
		{
			DevicesViewModel.Initialize();
			ParameterTemplatesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DirectionsViewModel.Initialize();
			PumpStationsViewModel.Initialize();
			MPTsViewModel.Initialize();
			DelaysViewModel.Initialize();
			CodesViewModel.Initialize();
			GuardZonesViewModel.Initialize();
			DoorsViewModel.Initialize();
			DaySchedulesViewModel.Initialize();
			SchedulesViewModel.Initialize();
			InstructionsViewModel.Initialize();
			OPCDevicesViewModel.Initialize();
			OPCZonesViewModel.Initialize();
			OPCDirectionsViewModel.Initialize();

			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			_planExtension.Cache.BuildAllSafe();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem(ModuleType.ToDescription(), null, new List<NavigationItem>()
				{
					new NavigationItem<ShowXDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/Tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowGKParameterTemplatesEvent, Guid>(ParameterTemplatesViewModel, "Шаблоны","/Controls;component/Images/Briefcase.png", null, null, Guid.Empty),
					new NavigationItemEx<ShowGKZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/Zones.png", null, null, Guid.Empty),
					new NavigationItem<ShowGKDirectionEvent, Guid>(DirectionsViewModel, "Направления", "/Controls;component/Images/Direction.png", null, null, Guid.Empty),
					new NavigationItem<ShowGKPumpStationEvent, Guid>(PumpStationsViewModel, "НС", "/Controls;component/Images/PumpStation.png", null, null, Guid.Empty),
					new NavigationItem<ShowGKMPTEvent, Guid>(MPTsViewModel, "МПТ", "/Controls;component/Images/MPT.png", null, null, Guid.Empty),
					new NavigationItem<ShowXDelayEvent, Guid>(DelaysViewModel, "Задержки", "/Controls;component/Images/Watch.png", null, null, Guid.Empty),

#if DEBUG
					new NavigationItem("Охрана", "/Controls;component/Images/tree.png",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowGKGuardEvent, Guid>(CodesViewModel, "Коды", "/Controls;component/Images/User.png", null, null, Guid.Empty),
							new NavigationItemEx<ShowGKGuardZoneEvent, Guid>(GuardZonesViewModel, "Зоны", "/Controls;component/Images/Zones.png", null, null, Guid.Empty),
						}),
                    new NavigationItem("СКД", "/Controls;component/Images/tree.png",
						new NavigationItem("СКД", "/Controls;component/Images/tree.png",
						new List<NavigationItem>()
						{
							new NavigationItemEx<ShowGKDoorEvent, Guid>(DoorsViewModel, "Точки доступа", "/Controls;component/Images/DoorW.png", null, null, Guid.Empty),
							new NavigationItem<ShowGKDaySchedulesEvent, Guid>(DaySchedulesViewModel, "Дневные графики", "/Controls;component/Images/ShedulesDaylyW.png", null, null, Guid.Empty),
							new NavigationItem<ShowGKScheduleEvent, Guid>(SchedulesViewModel, "Графики", "/Controls;component/Images/ShedulesW.png", null, null, Guid.Empty),
						}),
#endif

					new NavigationItem<ShowGKInstructionsEvent, Guid>(InstructionsViewModel, "Инструкции", "/Controls;component/Images/information.png", null, null, Guid.Empty),
					#if DEBUG
					new NavigationItem("OPC Сервер", "/Controls;component/Images/tree.png",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowGKOPCDevicesEvent, Guid>(OPCDevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
							new NavigationItem<ShowGKOPCZonesEvent, Guid>(OPCZonesViewModel, "Зоны", "/Controls;component/Images/Zones.png", null, null, Guid.Empty),
							new NavigationItem<ShowGKOPCDirectionsEvent, Guid>(OPCDirectionsViewModel, "Направления", "/Controls;component/Images/Direction.png", null, null, Guid.Empty),
						}),
					new NavigationItem<ShowGKDeviceLidraryEvent, object>(DeviceLidraryViewModel, "Библиотека", "/Controls;component/Images/Book.png"),
					new NavigationItem<ShowGKDescriptorsEvent, object>(DescriptorsViewModel, "Дескрипторы", "/Controls;component/Images/Descriptors.png"),
					new NavigationItem<ShowGKDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "/Controls;component/Images/Bug.png"),
					#endif
				}) {IsExpanded = true},
			};
		}
        public override ModuleType ModuleType
		{
			get { return ModuleType.GK; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Delays/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Descriptors/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DeviceLibrary/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Diagnostics/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Directions/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Guard/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Instructions/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Journal/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "MPTs/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "OPC/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Parameters/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "PumpStation/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Selectation/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "SKD/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
		}

		#region IValidationModule Members
		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}
		#endregion

		private void OnCreateGKZone(CreateGKZoneEventArg createZoneEventArg)
		{
			ZonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditGKZone(Guid zoneUID)
		{
			ZonesViewModel.EditZone(zoneUID);
		}

		private void OnCreateGKGuardZone(CreateGKGuardZoneEventArg createZoneEventArg)
		{
			GuardZonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditGKGuardZone(Guid zoneUID)
		{
			GuardZonesViewModel.EditZone(zoneUID);
		}

		private void OnCreateGKDirection(CreateGKDirectionEventArg createDirectionEventArg)
		{
			DirectionsViewModel.CreateDirection(createDirectionEventArg);
		}
		private void OnEditGKDirection(Guid directionUID)
		{
			DirectionsViewModel.EditDirection(directionUID);
		}

		private void OnCreateGKDoor(CreateGKDoorEventArg createGKDoorEventArg)
		{
			DoorsViewModel.CreateDoor(createGKDoorEventArg);
		}
		private void OnEditGKDoor(Guid doorUID)
		{
			DoorsViewModel.EditDoor(doorUID);
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			LoadingService.DoStep("Загрузка конфигурации ГК");
			GKDriversCreator.Create();
			GKManager.UpdateConfiguration();
			return true;
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Indicator, 110, "Индикаторы", "Панель индикаторов состояния", "BAlarm.png", false, new LayoutPartSize() { PreferedSize = new Size(1000, 100) });
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.ConnectionIndicator, 111, "Индикатор связи", "Панель индикаторов связи", "BConnectionIndicator.png", true, new LayoutPartSize() { PreferedSize = new Size(50, 30) });
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Alarms, 112, "Состояния", "Панель состояний", "BAlarm.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.GDevices, 113, "Устройства", "Панель с устройствами", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Zones, 114, "Зоны", "Панель зон", "BZones.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.GuardZones, 115, "Охранные зоны", "Панель охранных зон", "BZones.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Directions, 116, "Направления", "Панель направления", "BDirection.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.PumpStations, 117, "НС", "Панель НС", "BPumpStation.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.MPTs, 118, "МПТ", "Панель МПТ", "BMPT.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Doors, 119, "Точки доступа", "Панель точек досткпа", "BMPT.png");
		}

		#endregion
	}
}