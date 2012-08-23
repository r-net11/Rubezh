using System;
using System.Collections.Generic;
using Common.GK;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;

namespace GKModule
{
	public class GKModuleLoader : ModuleBase
	{
		static DevicesViewModel DevicesViewModel;
		static ZonesViewModel ZonesViewModel;
		static JournalViewModel JournalViewModel;

		public GKModuleLoader()
		{
			ServiceFactory.Layout.AddToolbarItem(new GKConnectionIndicatorViewModel());
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Subscribe(OnShowXDevice);
			ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Subscribe(OnShowXZone);
			ServiceFactory.Events.GetEvent<ShowXJournalEvent>().Subscribe(OnShowXJournalEvent);
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Subscribe(OnShowXDeviceDetails);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			JournalViewModel = new JournalViewModel();
		}

		void OnShowXDevice(Guid deviceUID)
		{
			DevicesViewModel.Select(deviceUID);
			ServiceFactory.Layout.Show(DevicesViewModel);
		}
		void OnShowXZone(ushort? zoneNo)
		{
			ZonesViewModel.Select(zoneNo);
			ServiceFactory.Layout.Show(ZonesViewModel);
		}
		void OnShowXJournalEvent(object obj)
		{
			ServiceFactory.Layout.Show(JournalViewModel);
		}

		void OnShowXDeviceDetails(Guid deviceUID)
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(deviceUID));
		}

		public override void Initialize()
		{
			GKDriversCreator.Create();
			XManager.DeviceConfiguration = FiresecManager.FiresecService.GetXDeviceConfiguration();
			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DatabaseManager.Convert();

			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			JournalViewModel.Initialize();

            JournalWatcherManager.Start();
            JournalWatcherManager.GetLastJournalItems(100);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("ГК", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowXDeviceEvent, Guid>("Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowXZoneEvent, ushort?>("Зоны", "/Controls;component/Images/zones.png"),
					new NavigationItem<ShowXJournalEvent, object>("Журнал", "/Controls;component/Images/book.png")
				}),
			};
		}

		public override string Name
		{
			get { return "Групповой контроллер"; }
		}
	}
}