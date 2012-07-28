using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using OPCModule.ViewModels;

namespace OPCModule
{
	public class OPCModule : ModuleBase
	{
		OPCDevicesViewModel OPCDevicesViewModel;
		OPCZonesViewModel OPCZonesViewModel;
		OPCSettingsViewModel OPCSettingsViewModel;

		public OPCModule()
		{
			ServiceFactory.Events.GetEvent<ShowOPCDeviceEvent>().Subscribe(OnShowOPCDevice);
			ServiceFactory.Events.GetEvent<ShowOPCZoneEvent>().Subscribe(OnShowOPCZone);
			ServiceFactory.Events.GetEvent<ShowOPCSettingsEvent>().Subscribe(OnShowOPCSettings);

			OPCDevicesViewModel = new OPCDevicesViewModel();
			OPCZonesViewModel = new OPCZonesViewModel();
			OPCSettingsViewModel = new OPCSettingsViewModel();
		}

		void OnShowOPCDevice(Guid deviceUID)
		{
			OPCDevicesViewModel.Initialize();
			if (deviceUID != Guid.Empty)
				OPCDevicesViewModel.Select(deviceUID);
			ServiceFactory.Layout.Show(OPCDevicesViewModel);
		}

		void OnShowOPCZone(int zoneNo)
		{
			OPCZonesViewModel.Initialize();
			if (zoneNo != 0)
				OPCZonesViewModel.SelectedZone = OPCZonesViewModel.Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
			ServiceFactory.Layout.Show(OPCZonesViewModel);
		}

		void OnShowOPCSettings(object obj)
		{
			ServiceFactory.Layout.Show(OPCSettingsViewModel);
		}

		public override void Initialize()
		{
			OPCDevicesViewModel.Initialize();
			OPCZonesViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("OPC сервер", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowOPCDeviceEvent, Guid>("Устройства","/Controls;component/Images/Tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowOPCZoneEvent, int>("Зоны","/Controls;component/Images/Zones.png", null, null, 0),
					new NavigationItem<ShowOPCSettingsEvent, object>("Настройки","/Controls;component/Images/Settings.png", null, null, null)
				}),
			};
		}
		public override string Name
		{
			get { return "OPC сервер"; }
		}
	}
}