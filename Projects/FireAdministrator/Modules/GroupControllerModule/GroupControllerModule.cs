using System.Collections.Generic;
using FiresecClient;
using GKModule.Converter;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;

namespace GKModule
{
	public class GroupControllerModule : ModuleBase
	{
		DevicesViewModel _devicesViewModel;
		ZonesViewModel _zonesViewModel;

		public GroupControllerModule()
		{
			ServiceFactory.Events.GetEvent<ShowXDevicesEvent>().Subscribe(OnShowXDevices);
			ServiceFactory.Events.GetEvent<ShowXZonesEvent>().Subscribe(OnShowXZones);

			_devicesViewModel = new DevicesViewModel();
			_zonesViewModel = new ZonesViewModel();
		}

		void OnShowXDevices(object obj)
		{
			ServiceFactory.Layout.Show(_devicesViewModel);
		}

		void OnShowXZones(object obj)
		{
			ServiceFactory.Layout.Show(_zonesViewModel);
		}

		public override void Initialize()
		{
			DriversConverter.Convert();
			XManager.DeviceConfiguration = FiresecManager.FiresecService.GetXDeviceConfiguration();
			XManager.UpdateConfiguration();

			_devicesViewModel.Initialize();
			_zonesViewModel.Initialize();
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
	}
}