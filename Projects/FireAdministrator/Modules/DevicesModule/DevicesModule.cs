using System;
using System.Collections.Generic;
using System.Linq;
using DevicesModule.Plans;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Infrastructure.Common.Validation;
using DevicesModule.Validation;
using FiresecClient;

namespace DevicesModule
{
	public class DevicesModule : ModuleBase, IValidationModule
	{
		private NavigationItem _guardNavigationItem;
		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;
		private DirectionsViewModel _directionsViewModel;
		private GuardViewModel _guardViewModel;
		private PlanExtension _planExtension;

		public DevicesModule()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(OnShowDevice);
			ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(OnShowZone);
			ServiceFactory.Events.GetEvent<ShowDirectionsEvent>().Subscribe(OnShowDirections);
			ServiceFactory.Events.GetEvent<ShowGuardEvent>().Subscribe(OnShowGuardDevices);
			ServiceFactory.Events.GetEvent<CreateZoneEvent>().Subscribe(OnCreateZone);
			ServiceFactory.Events.GetEvent<EditZoneEvent>().Subscribe(OnEditZone);

			_planExtension = new PlanExtension();
			_devicesViewModel = new DevicesViewModel(_planExtension);
			_zonesViewModel = new ZonesViewModel();
			_directionsViewModel = new DirectionsViewModel();
			_guardViewModel = new GuardViewModel();
		}

		void OnShowDevice(Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
				_devicesViewModel.Select(deviceUID);
			ServiceFactory.Layout.Show(_devicesViewModel);
		}

		void OnShowZone(int zoneNo)
		{
			if (zoneNo != 0)
				_zonesViewModel.SelectedZone = _zonesViewModel.Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
			ServiceFactory.Layout.Show(_zonesViewModel);
		}

		void OnShowDirections(int? directionId)
		{
			if (directionId.HasValue)
				_directionsViewModel.SelectedDirection = _directionsViewModel.Directions.FirstOrDefault(x => x.Direction.Id == directionId);
			ServiceFactory.Layout.Show(_directionsViewModel);
		}

		void OnShowGuardDevices(object obj)
		{
			ServiceFactory.Layout.Show(_guardViewModel);
		}

		void OnCreateZone(CreateZoneEventArg createZoneEventArg)
		{
			_zonesViewModel.CreateZone(createZoneEventArg);
		}
		void OnEditZone(int zoneNo)
		{
			_zonesViewModel.EditZone(zoneNo);
		}

		public override void RegisterResource()
		{
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Directions/DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Guard/DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
		}
		public override void Initialize()
		{
			_devicesViewModel.Initialize();
			_zonesViewModel.Initialize();
			_directionsViewModel.Initialize();
			_guardViewModel.Initialize();

			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_guardNavigationItem = new NavigationItem<ShowGuardEvent>("Охрана", "/Controls;component/Images/user.png") { IsVisible = false };
			ServiceFactory.Events.GetEvent<GuardVisibilityChangedEvent>().Subscribe(x => { _guardNavigationItem.IsVisible = x; });

			return new List<NavigationItem>()
			{
				new NavigationItem<ShowDeviceEvent, Guid>("Устройства","/Controls;component/Images/tree.png", null, null, Guid.Empty),
				new NavigationItem<ShowZoneEvent, int>("Зоны","/Controls;component/Images/zones.png", null, null, 0),
				new NavigationItem<ShowDirectionsEvent, int?>("Направления","/Controls;component/Images/direction.png"),
				_guardNavigationItem
			};
		}
		public override string Name
		{
			get { return "Устройства, Зоны, Направления"; }
		}

		#region IValidationModule Members

		public IEnumerable<IValidationError> Validate()
		{
			var devicesValidator = new Validator(FiresecManager.FiresecConfiguration);
			return devicesValidator.Validate();
		}

		#endregion
	}
}