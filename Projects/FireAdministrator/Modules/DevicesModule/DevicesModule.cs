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
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Directions/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Guard/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Validation/DataTemplates/Dictionary.xaml"));
        }

        public static void CreateViewModels()
        {
            devicesViewModel = new DevicesViewModel();
            devicesViewModel.Initialize();

            zonesViewModel = new ZonesViewModel();
            zonesViewModel.Initialize();

            directionsViewModel = new DirectionsViewModel();
            directionsViewModel.Initialize();

            guardUsersViewModel = new GuardUsersViewModel();
            guardUsersViewModel.Initialize();

            guardLevelsViewModel = new GuardLevelsViewModel();
            guardLevelsViewModel.Initialize();

            guardDevicesViewModel = new GuardDevicesViewModel();
            guardDevicesViewModel.Initialize();
        }

        static DevicesViewModel devicesViewModel;
        static ZonesViewModel zonesViewModel;
        static DirectionsViewModel directionsViewModel;
        static GuardUsersViewModel guardUsersViewModel;
        static GuardLevelsViewModel guardLevelsViewModel;
        static GuardDevicesViewModel guardDevicesViewModel;

        static void OnShowDevice(Guid deviceUID)
        {
            if (deviceUID != Guid.Empty)
            {
                devicesViewModel.Select(deviceUID);
            }
            ServiceFactory.Layout.Show(devicesViewModel);
        }

        static void OnShowZone(string zoneNo)
        {
            if (string.IsNullOrEmpty(zoneNo) == false)
            {
                zonesViewModel.SelectedZone = zonesViewModel.Zones.FirstOrDefault(x => x.No == zoneNo);
            }
            ServiceFactory.Layout.Show(zonesViewModel);
        }

        static void OnShowDirections(string obj)
        {
            ServiceFactory.Layout.Show(directionsViewModel);
        }

        static void OnShowGuardUsers(string obj)
        {
            ServiceFactory.Layout.Show(guardUsersViewModel);
        }

        static void OnShowGuardDevices(string obj)
        {
            ServiceFactory.Layout.Show(guardDevicesViewModel);
        }

        static void OnShowGuardLevels(string obj)
        {
            ServiceFactory.Layout.Show(guardLevelsViewModel);
        }

        public static bool HasChanges { get; set; }
    }
}