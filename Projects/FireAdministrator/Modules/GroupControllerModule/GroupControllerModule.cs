using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans;
using GKModule.Plans.Designer;
using GKModule.Validation;
using GKModule.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Client.Layout.ViewModels;
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
		DeviceParametersViewModel DeviceParametersViewModel;
		ParameterTemplatesViewModel ParameterTemplatesViewModel;
		ZonesViewModel ZonesViewModel;
		DirectionsViewModel DirectionsViewModel;
		GuardViewModel GuardViewModel;
		FiltersViewModel FiltersViewModel;
		LibraryViewModel DeviceLidraryViewModel;
		InstructionsViewModel InstructionsViewModel;
		OPCDevicesViewModel OPCDevicesViewModel;
		OPCZonesViewModel OPCZonesViewModel;
		OPCDirectionsViewModel OPCDirectionsViewModel;
		DiagnosticsViewModel DiagnosticsViewModel;
		GKPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<CreateXZoneEvent>().Subscribe(OnCreateXZone);
			ServiceFactory.Events.GetEvent<EditXZoneEvent>().Subscribe(OnEditXZone);
			ServiceFactory.Events.GetEvent<CreateXDirectionEvent>().Subscribe(OnCreateXDirection);
			ServiceFactory.Events.GetEvent<EditXDirectionEvent>().Subscribe(OnEditXDirection);

			DevicesViewModel = new DevicesViewModel();
			DeviceParametersViewModel = new DeviceParametersViewModel();
			ParameterTemplatesViewModel = new ParameterTemplatesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
			GuardViewModel = new GuardViewModel();
			FiltersViewModel = new FiltersViewModel();
			DeviceLidraryViewModel = new LibraryViewModel();
			InstructionsViewModel = new InstructionsViewModel();
			OPCDevicesViewModel = new OPCDevicesViewModel();
			OPCZonesViewModel = new OPCZonesViewModel();
			OPCDirectionsViewModel = new OPCDirectionsViewModel();
			DiagnosticsViewModel = new DiagnosticsViewModel();
			_planExtension = new GKPlanExtension(DevicesViewModel, ZonesViewModel, DirectionsViewModel);
		}

		public override void Initialize()
		{
			DevicesViewModel.Initialize();
			DeviceParametersViewModel.Initialize();
			ParameterTemplatesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DirectionsViewModel.Initialize();
			GuardViewModel.Initialize();
			FiltersViewModel.Initialize();
			InstructionsViewModel.Initialize();
			OPCDevicesViewModel.Initialize();
			OPCZonesViewModel.Initialize();
			OPCDirectionsViewModel.Initialize();

			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			Helper.BuildMap();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
				{
					new NavigationItem<ShowXDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowXDevicePropertiesEvent, Guid>(DeviceParametersViewModel, "Параметры","/Controls;component/Images/AllParameters.png", null, null, Guid.Empty),
					new NavigationItem<ShowXParameterTemplatesEvent, Guid>(ParameterTemplatesViewModel, "Шаблоны","/Controls;component/Images/briefcase.png", null, null, Guid.Empty),
					new NavigationItem<ShowXZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty),
					new NavigationItem<ShowXDirectionEvent, Guid>(DirectionsViewModel, "Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty),
					new NavigationItem<ShowXGuardEvent, Guid>(GuardViewModel, "Охрана", "/Controls;component/Images/user.png", null, null, Guid.Empty),
                    new NavigationItem<ShowXJournalFilterEvent, object>(FiltersViewModel, "Фильтры", "/Controls;component/Images/filter.png"),
                    new NavigationItem<ShowXDeviceLidraryViewModelEvent, object>(DeviceLidraryViewModel, "Библиотека", "/Controls;component/Images/book.png"),
					new NavigationItem<ShowXInstructionsEvent, Guid>(InstructionsViewModel, "Инструкции", "/Controls;component/Images/information.png", null, null, Guid.Empty),
					new NavigationItem("OPC Сервер", "/Controls;component/Images/tree.png",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowXOPCDevicesEvent, Guid>(OPCDevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
							new NavigationItem<ShowXOPCZonesEvent, Guid>(OPCZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty),
							new NavigationItem<ShowXOPCDirectionsEvent, Guid>(OPCDirectionsViewModel, "Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty),
						}),
					new NavigationItem<ShowXDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "/Controls;component/Images/Bug.png"),
				};
		}
		public override string Name
		{
			get { return "Групповой контроллер"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Parameters/DataTemplates/Dictionary.xaml"));
		}

		#region IValidationModule Members

		public IEnumerable<IValidationError> Validate()
		{
			return Validator.Validate();
		}

		#endregion

		private void OnCreateXZone(CreateXZoneEventArg createZoneEventArg)
		{
			ZonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditXZone(Guid zoneUID)
		{
			ZonesViewModel.EditZone(zoneUID);
		}

		private void OnCreateXDirection(CreateXDirectionEventArg createDirectionEventArg)
		{
			DirectionsViewModel.CreateDirection(createDirectionEventArg);
		}
		private void OnEditXDirection(Guid directionUID)
		{
			DirectionsViewModel.EditDirection(directionUID);
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			LoadingService.DoStep("Загрузка конфигурации ГК");
			GKDriversCreator.Create();
			XManager.UpdateConfiguration();
			GKDBHelper.AddMessage("Вход пользователя в систему", FiresecManager.CurrentUser.Name);
			return true;
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription()
			{
				Name = "Состояния",
				Description = "Панель состояний",
				Index = 110,
				UID = LayoutPartIdentities.Alarms,
				IconSource = "/Controls;component/Images/BAlarm.png",
				AllowMultiple = true,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BAlarm.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Устройства",
				Description = "Панель с устройствами",
				Index = 111,
				UID = LayoutPartIdentities.GDevices,
				IconSource = "/Controls;component/Images/BTree.png",
				AllowMultiple = true,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BTree.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Измерения",
				Description = "Панель измерений",
				Index = 112,
				UID = LayoutPartIdentities.DeviceParameters,
				IconSource = "/Controls;component/Images/BAllParameters.png",
				AllowMultiple = true,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BAllParameters.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Зоны",
				Description = "Панель зон",
				Index = 113,
				UID = LayoutPartIdentities.Zones,
				IconSource = "/Controls;component/Images/BZones.png",
				AllowMultiple = true,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BZones.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Направления",
				Description = "Панель направления",
				Index = 114,
				UID = LayoutPartIdentities.Directions,
				IconSource = "/Controls;component/Images/BDirection.png",
				AllowMultiple = true,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BDirection.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Журнал событий",
				Description = "Панель журнал событий",
				Index = 115,
				UID = LayoutPartIdentities.Journals,
				IconSource = "/Controls;component/Images/BBook.png",
				AllowMultiple = true,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BBook.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Архив",
				Description = "Панель архив",
				Index = 116,
				UID = LayoutPartIdentities.Archive,
				IconSource = "/Controls;component/Images/BArchive.png",
				AllowMultiple = true,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BArchive.png" },
			};
		}

		#endregion
	}
}