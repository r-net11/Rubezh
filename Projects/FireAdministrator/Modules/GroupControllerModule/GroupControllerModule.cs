using FiresecClient;
using GKModule.Converter;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace GKModule
{
	public class GroupControllerModule : ModuleBase
	{
		static DevicesViewModel _devicesViewModel;
		static ZonesViewModel _zonesViewModel;

		public GroupControllerModule()
		{
			if (ServiceFactory.AppSettings.ShowGK == false)
				return;

			ServiceFactory.Events.GetEvent<ShowXDevicesEvent>().Unsubscribe(OnShowXDevices);
			ServiceFactory.Events.GetEvent<ShowXZonesEvent>().Unsubscribe(OnShowXZones);
			ServiceFactory.Events.GetEvent<ShowXDevicesEvent>().Subscribe(OnShowXDevices);
			ServiceFactory.Events.GetEvent<ShowXZonesEvent>().Subscribe(OnShowXZones);
		}

		void CreateViewModels()
		{
			if (ServiceFactory.AppSettings.ShowGK == false)
				return;

			_devicesViewModel = new DevicesViewModel();
			_zonesViewModel = new ZonesViewModel();
		}

		static void OnShowXDevices(object obj)
		{
			ServiceFactory.Layout.Show(_devicesViewModel);
		}

		static void OnShowXZones(object obj)
		{
			ServiceFactory.Layout.Show(_zonesViewModel);
		}

		public override void Initialize()
		{
			DriversConverter.Convert();
			XManager.DeviceConfiguration = FiresecManager.FiresecService.GetXDeviceConfiguration();
			XManager.UpdateConfiguration();

			//XManager.SetEmptyConfiguration();

			CreateViewModels();
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