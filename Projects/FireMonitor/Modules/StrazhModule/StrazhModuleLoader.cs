using StrazhAPI.Enums;
using StrazhAPI.Events;
using StrazhAPI.GK;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Designer;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using StrazhModule.Plans;
using StrazhModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrazhModule
{
	public class StrazhModuleLoader : ModuleBase, ILayoutProviderModule
	{
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		DoorsViewModel DoorsViewModel;
		PlanPresenter PlanPresenter;

		public StrazhModuleLoader()
		{
			PlanPresenter = new PlanPresenter();
		}

		public override void CreateViewModels()
		{
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DoorsViewModel = new DoorsViewModel();

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
				new NavigationItem("Страж", "SKDW",
					new List<NavigationItem>()
					{
						new NavigationItem<ShowSKDDeviceEvent, Guid>(DevicesViewModel, "Устройства", "Tree", null, PermissionType.Oper_Strazh_Devices_View, Guid.Empty) {IsVisible = SKDManager.Devices.Count > 1},
						new NavigationItem<ShowSKDZoneEvent, Guid>(ZonesViewModel, "Зоны", "Zones", null, PermissionType.Oper_Strazh_Zones_View, Guid.Empty) {IsVisible = SKDManager.Zones.Count > 0},
						new NavigationItem<ShowSKDDoorEvent, Guid>(DoorsViewModel, "Точки доступа", "DoorW", null, PermissionType.Oper_Strazh_Doors_View, Guid.Empty) {IsVisible = SKDManager.Doors.Count > 0},
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
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
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
						device.State.AccessState = remoteDeviceState.AccessState;
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
						door.State.AccessState = remoteDoorState.AccessState;
						door.State.OnStateChanged();
					}
				}
			}
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDevices, "Устройства Страж", "Tree.png", (p) => DevicesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDZones, "Зоны Страж", "Tree.png", (p) => ZonesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDoors, "Точки доступа Страж", "Tree.png", (p) => DoorsViewModel);
		}
		#endregion
	}
}