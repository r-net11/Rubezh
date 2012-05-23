using System;
using DevicesModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrastructure.Common.Navigation;
using System.Collections.Generic;

namespace DevicesModule
{
	public class DevicesModuleLoader : ModuleBase
	{
		static DevicesViewModel DevicesViewModel;
		static ZonesViewModel ZonesViewModel;

		public DevicesModuleLoader()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(OnShowDevice);
			ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(OnShowZone);
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Subscribe(OnShowDeviceDetails);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
		}

		void OnShowDevice(Guid deviceUID)
		{
			DevicesViewModel.Select(deviceUID);
			ServiceFactory.Layout.Show(DevicesViewModel);
		}
		void OnShowZone(ulong? zoneNo)
		{
			ZonesViewModel.Select(zoneNo);
			ServiceFactory.Layout.Show(ZonesViewModel);
		}
		void OnShowDeviceDetails(Guid deviceUID)
		{
			ServiceFactory.UserDialogs.ShowWindow(new DeviceDetailsViewModel(deviceUID), true);
		}

		public override void Initialize()
		{
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowDeviceEvent, Guid>("Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
				new NavigationItem<ShowZoneEvent, ulong?>("Зоны", "/Controls;component/Images/zones.png")
			};
		}
	}
}