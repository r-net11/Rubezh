using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using GKModule.Plans;
using GKModule.Validation;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Validation;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Common.GK;
using FiresecClient;
using Infrastructure.Common.Windows;

namespace GKModule
{
	public class GroupControllerModule : ModuleBase, IValidationModule
	{
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		DirectionsViewModel DirectionsViewModel;
        FiltersViewModel FiltersViewModel;
        XLibraryViewModel DeviceLidraryViewModel;
        InstructionsViewModel InstructionsViewModel;
		GKPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<CreateXZoneEvent>().Subscribe(OnCreateXZone);
			ServiceFactory.Events.GetEvent<EditXZoneEvent>().Subscribe(OnEditXZone);
			
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
            FiltersViewModel = new FiltersViewModel();
            DeviceLidraryViewModel = new XLibraryViewModel();
            InstructionsViewModel = new InstructionsViewModel();
			_planExtension = new GKPlanExtension(DevicesViewModel);
		}

		public override void Initialize()
		{
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DirectionsViewModel.Initialize();
            FiltersViewModel.Initialize();
            InstructionsViewModel.Initialize();

			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("ГК", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowXDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowXZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty),
					new NavigationItem<ShowXDirectionEvent, Guid>(DirectionsViewModel, "Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty),
                    new NavigationItem<ShowXJournalFilterEvent, object>(FiltersViewModel, "Фильтры", "/Controls;component/Images/filter.png"),
                    new NavigationItem<ShowXDeviceLidraryViewModelEvent, object>(DeviceLidraryViewModel, "Библиотека", "/Controls;component/Images/book.png"),
                    new NavigationItem<ShowXInstructionsEvent, Guid>(InstructionsViewModel, "Инструкции", "/Controls;component/Images/information.png", null, null, Guid.Empty)
				}),
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

		public override bool BeforeInitialize(bool firstTime)
		{
			LoadingService.DoStep("Загрузка конфигурации ГК");
			GKDriversCreator.Create();
			XManager.GetConfiguration();
			return true;
		}
	}
}