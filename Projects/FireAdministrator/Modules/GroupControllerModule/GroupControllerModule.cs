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
		SKDZonesViewModel SKDZonesViewModel;
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
			ServiceFactory.Events.GetEvent<CreateGKSKDZoneEvent>().Subscribe(OnCreateSKDZone);
			ServiceFactory.Events.GetEvent<EditGKSKDZoneEvent>().Subscribe(OnEditSKDZone);
			ServiceFactory.Events.GetEvent<CreateGKDirectionEvent>().Subscribe(OnCreateGKDirection);
			ServiceFactory.Events.GetEvent<EditGKDirectionEvent>().Subscribe(OnEditGKDirection);
			ServiceFactory.Events.GetEvent<CreateGKMPTEvent>().Subscribe(OnCreateGKMPT);
			ServiceFactory.Events.GetEvent<EditGKMPTEvent>().Subscribe(OnEditGKMPT);
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
			SKDZonesViewModel = new SKDZonesViewModel();
			DeviceLidraryViewModel = new LibraryViewModel();
			InstructionsViewModel = new InstructionsViewModel();
			OPCDevicesViewModel = new OPCDevicesViewModel();
			OPCZonesViewModel = new OPCZonesViewModel();
			OPCDirectionsViewModel = new OPCDirectionsViewModel();
			DescriptorsViewModel = new DescriptorsViewModel();
			DiagnosticsViewModel = new DiagnosticsViewModel();
			_planExtension = new GKPlanExtension(DevicesViewModel, ZonesViewModel, GuardZonesViewModel, SKDZonesViewModel, DirectionsViewModel, MPTsViewModel, DoorsViewModel);
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
			SKDZonesViewModel.Initialize();
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
				new NavigationItem(ModuleType.ToDescription(), "Tree", new List<NavigationItem>()
				{
					new NavigationItem<ShowGKDeviceEvent, Guid>(DevicesViewModel, "Устройства", "Tree", null, null, Guid.Empty),
					new NavigationItem<ShowGKParameterTemplatesEvent, Guid>(ParameterTemplatesViewModel, "Шаблоны","Briefcase", null, null, Guid.Empty),
					new NavigationItemEx<ShowGKZoneEvent, Guid>(ZonesViewModel, "Пожарные зоны", "Zones", null, null, Guid.Empty),
					new NavigationItem<ShowGKDirectionEvent, Guid>(DirectionsViewModel, "Направления", "Direction", null, null, Guid.Empty),
					new NavigationItem<ShowGKPumpStationEvent, Guid>(PumpStationsViewModel, "НС", "PumpStation", null, null, Guid.Empty),
					new NavigationItem<ShowGKMPTEvent, Guid>(MPTsViewModel, "МПТ", "MPT", null, null, Guid.Empty),
					new NavigationItem<ShowXDelayEvent, Guid>(DelaysViewModel, "Задержки", "Watch", null, null, Guid.Empty),

					new NavigationItem("Охрана", "tree",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowGKGuardEvent, Guid>(CodesViewModel, "Коды", "User", null, null, Guid.Empty),
							new NavigationItemEx<ShowGKGuardZoneEvent, Guid>(GuardZonesViewModel, "Зоны", "Zones", null, null, Guid.Empty),
						}),
					new NavigationItem("СКД", "tree",
						new List<NavigationItem>()
						{
							new NavigationItemEx<ShowGKDoorEvent, Guid>(DoorsViewModel, "Точки доступа", "DoorW", null, null, Guid.Empty),
							new NavigationItemEx<ShowGKSKDZoneEvent, Guid>(SKDZonesViewModel, "Зоны", "Zones", null, null, Guid.Empty),
						}),

					new NavigationItem<ShowGKInstructionsEvent, Guid>(InstructionsViewModel, "Инструкции", "information", null, null, Guid.Empty),
					new NavigationItem<ShowGKDescriptorsEvent, object>(DescriptorsViewModel, "Дескрипторы", "Descriptors"),
#if DEBUG
					new NavigationItem("OPC Сервер", "tree",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowGKOPCDevicesEvent, Guid>(OPCDevicesViewModel, "Устройства", "tree", null, null, Guid.Empty),
							new NavigationItem<ShowGKOPCZonesEvent, Guid>(OPCZonesViewModel, "Зоны", "Zones", null, null, Guid.Empty),
							new NavigationItem<ShowGKOPCDirectionsEvent, Guid>(OPCDirectionsViewModel, "Направления", "Direction", null, null, Guid.Empty),
						}),
					new NavigationItem<ShowGKDeviceLidraryEvent, object>(DeviceLidraryViewModel, "Библиотека", "Book"),
					new NavigationItem<ShowGKDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "Bug"),
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

		private void OnCreateGKMPT(CreateGKMPTEventArg createMPTEventArg)
		{
			MPTsViewModel.CreateMPT(createMPTEventArg);
		}
		private void OnEditGKMPT(Guid mptUID)
		{
			MPTsViewModel.EditMPT(mptUID);
		}

		private void OnCreateGKDoor(CreateGKDoorEventArg createGKDoorEventArg)
		{
			DoorsViewModel.CreateDoor(createGKDoorEventArg);
		}
		private void OnEditGKDoor(Guid doorUID)
		{
			DoorsViewModel.EditDoor(doorUID);
		}
		private void OnCreateSKDZone(CreateGKSKDZoneEventArg createZoneEventArg)
		{
			SKDZonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditSKDZone(Guid zoneUID)
		{
			SKDZonesViewModel.EditZone(zoneUID);
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