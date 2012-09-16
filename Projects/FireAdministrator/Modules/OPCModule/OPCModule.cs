using System;
using System.Collections.Generic;
using Infrastructure.Client;
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
			OPCDevicesViewModel = new OPCDevicesViewModel();
			OPCZonesViewModel = new OPCZonesViewModel();
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
					new NavigationItem<ShowOPCDeviceEvent, Guid>(OPCDevicesViewModel, "Устройства","/Controls;component/Images/Tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowOPCZoneEvent, int>(OPCZonesViewModel, "Зоны","/Controls;component/Images/Zones.png", null, null, 0),
				}),
			};
		}
		public override string Name
		{
			get { return "OPC сервер"; }
		}
	}
}