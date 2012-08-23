using System.Collections.Generic;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans;
using GKModule.Validation;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Validation;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using System;
using System.Linq;

namespace GKModule
{
	public class GroupControllerModule : ModuleBase, IValidationModule
	{
		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;
		private DirectionsViewModel _directionsViewModel;

		public GroupControllerModule()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Subscribe(OnShowXDevices);
			ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Subscribe(OnShowXZones);
			ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Subscribe(OnShowXDirections);

			_devicesViewModel = new DevicesViewModel();
			_zonesViewModel = new ZonesViewModel();
			_directionsViewModel = new DirectionsViewModel();
		}

		private void OnShowXDevices(Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
				_devicesViewModel.Select(deviceUID);
			ServiceFactory.Layout.Show(_devicesViewModel);
		}
		private void OnShowXZones(ushort zoneNo)
		{
			if (zoneNo != 0)
				_zonesViewModel.SelectedZone = _zonesViewModel.Zones.FirstOrDefault(x => x.XZone.No == zoneNo);
			ServiceFactory.Layout.Show(_zonesViewModel);
		}
		private void OnShowXDirections(ushort directionNo)
		{
			if (directionNo != 0)
				_directionsViewModel.SelectedDirection = _directionsViewModel.Directions.FirstOrDefault(x => x.Direction.No == directionNo);
			ServiceFactory.Layout.Show(_directionsViewModel);
		}

		public override void Initialize()
		{
			_devicesViewModel.Initialize();
			_zonesViewModel.Initialize();
			_directionsViewModel.Initialize();

			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(new GKPlanExtension());
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("ГК", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowXDeviceEvent, Guid>("Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowXZoneEvent, ushort>("Зоны", "/Controls;component/Images/zones.png", null, null, 0),
					new NavigationItem<ShowXDirectionEvent, ushort>("Направления", "/Controls;component/Images/direction.png", null, null, 0)
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
	}
}