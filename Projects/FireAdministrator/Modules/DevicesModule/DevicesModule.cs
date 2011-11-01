using System;
using System.Linq;
using DevicesModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;

namespace DevicesModule
{
    public class DevicesModule : IModule
    {
        public static bool HasChanges { get; set; }

        static DevicesViewModel _devicesViewModel;
        static ZonesViewModel _zonesViewModel;
        static DirectionsViewModel _directionsViewModel;
        static GuardUsersViewModel _guardUsersViewModel;
        static GuardLevelsViewModel _guardLevelsViewModel;
        static GuardDevicesViewModel _guardDevicesViewModel;

        public DevicesModule()
        {
            HasChanges = false;
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(OnShowDevice);
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(OnShowZone);
            ServiceFactory.Events.GetEvent<ShowDirectionsEvent>().Subscribe(OnShowDirections);
            ServiceFactory.Events.GetEvent<ShowGuardUsersEvent>().Subscribe(OnShowGuardUsers);
            ServiceFactory.Events.GetEvent<ShowGuardLevelsEvent>().Subscribe(OnShowGuardLevels);
            ServiceFactory.Events.GetEvent<ShowGuardDevicesEvent>().Subscribe(OnShowGuardDevices);
        }

        public void Initialize()
        {
            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Directions/DataTemplates/Dictionary.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Guard/DataTemplates/Dictionary.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Validation/DataTemplates/Dictionary.xaml"));
        }

        public static void CreateViewModels()
        {
            _devicesViewModel = new DevicesViewModel();
            _devicesViewModel.Initialize();

            _zonesViewModel = new ZonesViewModel();
            _zonesViewModel.Initialize();

            _directionsViewModel = new DirectionsViewModel();
            _directionsViewModel.Initialize();

            _guardUsersViewModel = new GuardUsersViewModel();
            _guardUsersViewModel.Initialize();

            _guardLevelsViewModel = new GuardLevelsViewModel();
            _guardLevelsViewModel.Initialize();

            _guardDevicesViewModel = new GuardDevicesViewModel();
            _guardDevicesViewModel.Initialize();
        }

        static void OnShowDevice(Guid deviceUID)
        {
            if (deviceUID != Guid.Empty)
                _devicesViewModel.Select(deviceUID);
            ServiceFactory.Layout.Show(_devicesViewModel);
        }

        static void OnShowZone(ulong? zoneNo)
        {
            if (zoneNo != null)
                _zonesViewModel.SelectedZone = _zonesViewModel.Zones.FirstOrDefault(x => x.No == zoneNo);
            ServiceFactory.Layout.Show(_zonesViewModel);
        }

        static void OnShowDirections(int? directionId)
        {
            if (directionId.HasValue)
                _directionsViewModel.SelectedDirection = _directionsViewModel.Directions.FirstOrDefault(x => x.Direction.Id == directionId);
            ServiceFactory.Layout.Show(_directionsViewModel);
        }

        static void OnShowGuardUsers(string obj)
        {
            ServiceFactory.Layout.Show(_guardUsersViewModel);
        }

        static void OnShowGuardDevices(string obj)
        {
            ServiceFactory.Layout.Show(_guardDevicesViewModel);
        }

        static void OnShowGuardLevels(string obj)
        {
            ServiceFactory.Layout.Show(_guardLevelsViewModel);
        }
    }
}