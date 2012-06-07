using System;
using System.Collections.Generic;
using DevicesModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;

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
			DialogService.ShowWindow(new DeviceDetailsViewModel(deviceUID));
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