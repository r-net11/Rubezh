using System;
using System.Collections.Generic;
using Common.GK;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client;
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
        static DirectionsViewModel DirectionsViewModel;
		static JournalsViewModel JournalsViewModel;
        static ArchiveViewModel ArchiveViewModel;
		NavigationItem _zonesNavigationItem;
		NavigationItem _directionsNavigationItem;

		public GKModuleLoader()
		{
			ServiceFactory.Layout.AddToolbarItem(new GKConnectionIndicatorViewModel());
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Subscribe(OnShowXDeviceDetails);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
            JournalsViewModel = new JournalsViewModel();
            ArchiveViewModel = new ArchiveViewModel();
		}

		void OnShowXDeviceDetails(Guid deviceUID)
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(deviceUID));
		}

		public override void Initialize()
		{
			_zonesNavigationItem.IsVisible = XManager.DeviceConfiguration.Zones.Count > 0;
			_directionsNavigationItem.IsVisible = XManager.DeviceConfiguration.Directions.Count > 0;
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
            DirectionsViewModel.Initialize();
			JournalsViewModel.Initialize();
            ArchiveViewModel.Initialize();

            JournalWatcherManager.Start();
            JournalWatcherManager.GetLastJournalItems(100);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_zonesNavigationItem = new NavigationItem<ShowXZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty);
			_directionsNavigationItem = new NavigationItem<ShowXDirectionEvent, Guid>(DirectionsViewModel, "Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty);
			return new List<NavigationItem>()
			{
				new NavigationItem("ГК", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowXDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					_zonesNavigationItem,
					_directionsNavigationItem,
					new NavigationItem<ShowXJournalEvent, object>(JournalsViewModel, "Журнал", "/Controls;component/Images/book.png"),
                    new NavigationItem<ShowXArchiveEvent, object>(ArchiveViewModel, "Архив", "/Controls;component/Images/archive.png")
				}),
			};
		}

		public override string Name
		{
			get { return "Групповой контроллер"; }
		}
	}
}