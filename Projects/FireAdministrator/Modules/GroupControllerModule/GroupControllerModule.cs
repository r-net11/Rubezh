using System.Collections.Generic;
using Commom.GK;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using FiresecAPI.Models;
using GKModule.Plans.Designer;
using GKModule.Plans;

namespace GKModule
{
	public class GroupControllerModule : ModuleBase
	{
		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;

		public GroupControllerModule()
		{
			ServiceFactory.Events.GetEvent<ShowXDevicesEvent>().Subscribe(OnShowXDevices);
			ServiceFactory.Events.GetEvent<ShowXZonesEvent>().Subscribe(OnShowXZones);

			_devicesViewModel = new DevicesViewModel();
			_zonesViewModel = new ZonesViewModel();
		}

		private void OnShowXDevices(object obj)
		{
			ServiceFactory.Layout.Show(_devicesViewModel);
		}
		private void OnShowXZones(object obj)
		{
			ServiceFactory.Layout.Show(_zonesViewModel);
		}

		public override void Initialize()
		{
			GKDriversConverter.Convert();
			XManager.DeviceConfiguration = FiresecManager.FiresecService.GetXDeviceConfiguration();
			XManager.UpdateConfiguration();

			_devicesViewModel.Initialize();
			_zonesViewModel.Initialize();

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
			ResourceService resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
		}

	}
}