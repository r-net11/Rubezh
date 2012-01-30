using System;
using System.Linq;
using DevicesModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace DevicesModule
{
    public class DevicesModule
    {
        static DevicesViewModel _devicesViewModel;
        static ZonesViewModel _zonesViewModel;
        static DirectionsViewModel _directionsViewModel;
        static UsersViewModel _usersViewModel;
        static LevelsViewModel _levelsViewModel;

        public DevicesModule()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(OnShowDevice);
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(OnShowZone);
            ServiceFactory.Events.GetEvent<ShowDirectionsEvent>().Subscribe(OnShowDirections);
            ServiceFactory.Events.GetEvent<ShowGuardUsersEvent>().Subscribe(OnShowGuardUsers);
            ServiceFactory.Events.GetEvent<ShowGuardLevelsEvent>().Subscribe(OnShowGuardLevels);

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
            _zonesViewModel = new ZonesViewModel();
            _directionsViewModel = new DirectionsViewModel();
            _usersViewModel = new UsersViewModel();
            _levelsViewModel = new LevelsViewModel();
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
            ServiceFactory.Layout.Show(_usersViewModel);
        }

        static void OnShowGuardLevels(string obj)
        {
            ServiceFactory.Layout.Show(_levelsViewModel);
        }
    }
}