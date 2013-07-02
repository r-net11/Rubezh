using System;
using System.Collections.Generic;
using System.Diagnostics;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans;
using GKModule.Plans.Designer;
using GKModule.Validation;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using XFiresecAPI;

namespace GKModule
{
	public class GroupControllerModule : ModuleBase, IValidationModule
	{
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		DirectionsViewModel DirectionsViewModel;
		FiltersViewModel FiltersViewModel;
		LibraryViewModel DeviceLidraryViewModel;
		InstructionsViewModel InstructionsViewModel;
		DiagnosticsViewModel DiagnosticsViewModel;
		GKPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<CreateXZoneEvent>().Subscribe(OnCreateXZone);
			ServiceFactory.Events.GetEvent<EditXZoneEvent>().Subscribe(OnEditXZone);
			ServiceFactory.Events.GetEvent<CreateXDirectionEvent>().Subscribe(OnCreateXDirection);
			ServiceFactory.Events.GetEvent<EditXDirectionEvent>().Subscribe(OnEditXDirection);

			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
			FiltersViewModel = new FiltersViewModel();
			DeviceLidraryViewModel = new LibraryViewModel();
			InstructionsViewModel = new InstructionsViewModel();
			DiagnosticsViewModel = new DiagnosticsViewModel();
			_planExtension = new GKPlanExtension(DevicesViewModel, ZonesViewModel, DirectionsViewModel);
		}

		public override void Initialize()
		{
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DirectionsViewModel.Initialize();
			FiltersViewModel.Initialize();
			InstructionsViewModel.Initialize();

			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			Helper.BuildMap();

			foreach (var driver in XManager.DriversConfiguration.XDrivers)
			{
				if (driver.AvailableStates.Contains(XStateType.Fire2))
				{
					Trace.WriteLine("Driver contains Fire2: " + driver.ShortName);
				}
			}
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("ГК", "/Controls;component/Images/tree.png", new List<NavigationItem>()
				{
					new NavigationItem<ShowXDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowXZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty),
					new NavigationItem<ShowXDirectionEvent, Guid>(DirectionsViewModel, "Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty),
                    new NavigationItem<ShowXJournalFilterEvent, object>(FiltersViewModel, "Фильтры", "/Controls;component/Images/filter.png"),
                    new NavigationItem<ShowXDeviceLidraryViewModelEvent, object>(DeviceLidraryViewModel, "Библиотека", "/Controls;component/Images/book.png"),
                    new NavigationItem<ShowXInstructionsEvent, Guid>(InstructionsViewModel, "Инструкции", "/Controls;component/Images/information.png", null, null, Guid.Empty),
					new NavigationItem<ShowXDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "/Controls;component/Images/Bug.png"),
				}) { IsExpanded = true },
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
			return true;
		}
	}
}