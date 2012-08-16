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

namespace GKModule
{
	public class GroupControllerModule : ModuleBase, IValidationModule
	{
		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;
		private DirectionsViewModel _directionsViewModel;

		public GroupControllerModule()
		{
			ServiceFactory.Events.GetEvent<ShowXDevicesEvent>().Subscribe(OnShowXDevices);
			ServiceFactory.Events.GetEvent<ShowXZonesEvent>().Subscribe(OnShowXZones);
			ServiceFactory.Events.GetEvent<ShowXDirectionsEvent>().Subscribe(OnShowXDirections);

			_devicesViewModel = new DevicesViewModel();
			_zonesViewModel = new ZonesViewModel();
			_directionsViewModel = new DirectionsViewModel();
		}

		private void OnShowXDevices(object obj)
		{
			ServiceFactory.Layout.Show(_devicesViewModel);
		}
		private void OnShowXZones(object obj)
		{
			ServiceFactory.Layout.Show(_zonesViewModel);
		}
		private void OnShowXDirections(object obj)
		{
			ServiceFactory.Layout.Show(_directionsViewModel);
		}

		public override void Initialize()
		{
			GKDriversConverter.Convert();
			XManager.DeviceConfiguration = FiresecManager.FiresecService.GetXDeviceConfiguration();
			XManager.UpdateConfiguration();

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
					new NavigationItem<ShowXDevicesEvent>("Устройства", "/Controls;component/Images/tree.png"),
					new NavigationItem<ShowXZonesEvent>("Зоны", "/Controls;component/Images/zones.png"),
					new NavigationItem<ShowXDirectionsEvent>("Направления", "/Controls;component/Images/direction.png")
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