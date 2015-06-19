using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans;
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
	public class GroupControllerModule : ModuleBase, ILayoutDeclarationModule
	{
		DoorsViewModel DoorsViewModel;
		SKDZonesViewModel SKDZonesViewModel;
		GKPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			//ServiceFactory.Events.GetEvent<CreateGKSKDZoneEvent>().Subscribe(OnCreateSKDZone);
			//ServiceFactory.Events.GetEvent<EditGKSKDZoneEvent>().Subscribe(OnEditSKDZone);
			//ServiceFactory.Events.GetEvent<CreateGKDoorEvent>().Subscribe(OnCreateGKDoor);
			//ServiceFactory.Events.GetEvent<EditGKDoorEvent>().Subscribe(OnEditGKDoor);

			//DevicesViewModel = new DevicesViewModel();
			//DoorsViewModel = new DoorsViewModel();
			//SKDZonesViewModel = new SKDZonesViewModel();
			//_planExtension = new GKPlanExtension(DevicesViewModel, SKDZonesViewModel, DoorsViewModel);
		}

		public override void Initialize()
		{
			//DevicesViewModel.Initialize();
			//DoorsViewModel.Initialize();
			//SKDZonesViewModel.Initialize();

			//_planExtension.Initialize();
			//ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			//_planExtension.Cache.BuildAllSafe();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			//return new List<NavigationItem>()
			//{
			//	new NavigationItem(ModuleType.ToDescription(), "Tree", new List<NavigationItem>()
			//	{
			//		new NavigationItem<ShowGKDeviceEvent, Guid>(DevicesViewModel, "Устройства", "Tree", null, null, Guid.Empty),
			//	}) {IsExpanded = true},
			//};
			return new List<NavigationItem>();
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
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Journal/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Selectation/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "SKD/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
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
			//LoadingService.DoStep("Загрузка конфигурации ГК");
			//GKDriversCreator.Create();
			//GKManager.UpdateConfiguration();
			return true;
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Indicator, 110, "Индикаторы", "Панель индикаторов состояния", "BAlarm.png", false, new LayoutPartSize() { PreferedSize = new Size(1000, 100) });
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.ConnectionIndicator, 111, "Индикатор связи", "Панель индикаторов связи", "BConnectionIndicator.png", true, new LayoutPartSize() { PreferedSize = new Size(50, 30) });
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.GDevices, 113, "Устройства", "Панель с устройствами", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Doors, 119, "Точки доступа", "Панель точек досткпа", "BMPT.png");
		}

		#endregion
	}
}