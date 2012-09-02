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
		static JournalsViewModel JournalsViewModel;
		static DirectionsViewModel DirectionsViewModel;
		NavigationItem _zonesNavigationItem;
		NavigationItem _directionsNavigationItem;

		public GKModuleLoader()
		{
			ServiceFactory.Layout.AddToolbarItem(new GKConnectionIndicatorViewModel());
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Subscribe(OnShowXDevice);
			ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Subscribe(OnShowXZone);
			ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Subscribe(OnShowXDirection);
			ServiceFactory.Events.GetEvent<ShowXJournalEvent>().Subscribe(OnShowXJournalEvent);
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Subscribe(OnShowXDeviceDetails);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			JournalsViewModel = new JournalsViewModel();
			DirectionsViewModel = new DirectionsViewModel();
		}

		void OnShowXDevice(Guid deviceUID)
		{
			DevicesViewModel.Select(deviceUID);
			ServiceFactory.Layout.Show(DevicesViewModel);
		}
		void OnShowXZone(Guid zoneUID)
		{
			ZonesViewModel.Select(zoneUID);
			ServiceFactory.Layout.Show(ZonesViewModel);
		}
		void OnShowXDirection(Guid directionUID)
		{
			DirectionsViewModel.Select(directionUID);
			ServiceFactory.Layout.Show(DirectionsViewModel);
		}
		void OnShowXJournalEvent(object obj)
		{
			ServiceFactory.Layout.Show(JournalsViewModel);
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

			_zonesNavigationItem.IsVisible = XManager.DeviceConfiguration.Zones.Count > 0;
			_directionsNavigationItem.IsVisible = XManager.DeviceConfiguration.Directions.Count > 0;
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			JournalsViewModel.Initialize();
			DirectionsViewModel.Initialize();

			JournalWatcherManager.Start();
			JournalWatcherManager.GetLastJournalItems(100);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_zonesNavigationItem = new NavigationItem<ShowXZoneEvent, Guid>("Зоны", "/Controls;component/Images/zones.png", null, null, null, Guid.Empty);
			_directionsNavigationItem = new NavigationItem<ShowXDirectionEvent, Guid>("Направления", "/Controls;component/Images/direction.png", null, null, null, Guid.Empty);
			return new List<NavigationItem>()
			{
				new NavigationItem("ГК", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowXDeviceEvent, Guid>("Устройства", "/Controls;component/Images/tree.png", null, null, null, Guid.Empty),
					_zonesNavigationItem,
					_directionsNavigationItem,
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