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

		public OPCModule()
		{
			ServiceFactory.Events.GetEvent<ShowOPCDeviceEvent>().Subscribe(OnShowOPCDevice);
			ServiceFactory.Events.GetEvent<ShowOPCZoneEvent>().Subscribe(OnShowOPCZone);

			OPCDevicesViewModel = new OPCDevicesViewModel();
			OPCZonesViewModel = new OPCZonesViewModel();
		}

		void OnShowOPCDevice(Guid deviceUID)
		{
			OPCDevicesViewModel.Initialize();
			if (deviceUID != Guid.Empty)
				OPCDevicesViewModel.Select(deviceUID);
			ServiceFactory.Layout.Show(OPCDevicesViewModel);
		}

		void OnShowOPCZone(ulong zoneNo)
		{
			OPCZonesViewModel.Initialize();
			if (zoneNo != 0)
				OPCZonesViewModel.SelectedZone = OPCZonesViewModel.Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
			ServiceFactory.Layout.Show(OPCZonesViewModel);
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
				new NavigationItem("OPC", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowOPCDeviceEvent, Guid>("Устройства","/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowOPCZoneEvent, ulong>("Зоны","/Controls;component/Images/zones.png", null, null, 0)
				}),
			};
		}
		public override string Name
		{
			get { return "OPC сервер"; }
		}
	}
}