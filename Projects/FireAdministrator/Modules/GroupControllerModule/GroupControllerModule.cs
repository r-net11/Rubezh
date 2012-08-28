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
        private FiltersViewModel _filtersViewModel;

		public GroupControllerModule()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Subscribe(OnShowXDevices);
			ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Subscribe(OnShowXZones);
			ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Subscribe(OnShowXDirections);
            ServiceFactory.Events.GetEvent<ShowXJournalFilterEvent>().Subscribe(OnShowXJournalFilter);

			_devicesViewModel = new DevicesViewModel();
			_zonesViewModel = new ZonesViewModel();
			_directionsViewModel = new DirectionsViewModel();
            _filtersViewModel = new FiltersViewModel();
		}

		private void OnShowXDevices(Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
				_devicesViewModel.Select(deviceUID);
			ServiceFactory.Layout.Show(_devicesViewModel);
		}
		private void OnShowXZones(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
				_zonesViewModel.SelectedZone = _zonesViewModel.Zones.FirstOrDefault(x => x.XZone.UID == zoneUID);
			ServiceFactory.Layout.Show(_zonesViewModel);
		}
		private void OnShowXDirections(Guid directionUID)
		{
			if (directionUID != Guid.Empty)
				_directionsViewModel.SelectedDirection = _directionsViewModel.Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			ServiceFactory.Layout.Show(_directionsViewModel);
		}
        private void OnShowXJournalFilter(object obj)
        {
            ServiceFactory.Layout.Show(_filtersViewModel);
        }

		public override void Initialize()
		{
			_devicesViewModel.Initialize();
			_zonesViewModel.Initialize();
			_directionsViewModel.Initialize();
            _filtersViewModel.Initialize();

			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(new GKPlanExtension());
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("ГК", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowXDeviceEvent, Guid>("Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowXZoneEvent, Guid>("Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty),
					new NavigationItem<ShowXDirectionEvent, Guid>("Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty),
                    new NavigationItem<ShowXJournalFilterEvent, object>("Фильтры", "/Controls;component/Images/filter.png")
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