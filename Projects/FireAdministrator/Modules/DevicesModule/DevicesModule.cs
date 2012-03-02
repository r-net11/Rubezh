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
        static GuardViewModel _guardViewModel;

        public DevicesModule()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Unsubscribe(OnShowDevice);
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Unsubscribe(OnShowZone);
            ServiceFactory.Events.GetEvent<ShowDirectionsEvent>().Unsubscribe(OnShowDirections);
            ServiceFactory.Events.GetEvent<ShowGuardEvent>().Unsubscribe(OnShowGuardDevices);
            ServiceFactory.Events.GetEvent<CreateZoneEvent>().Unsubscribe(OnCreateZone);

            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(OnShowDevice);
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(OnShowZone);
            ServiceFactory.Events.GetEvent<ShowDirectionsEvent>().Subscribe(OnShowDirections);
            ServiceFactory.Events.GetEvent<ShowGuardEvent>().Subscribe(OnShowGuardDevices);
            ServiceFactory.Events.GetEvent<CreateZoneEvent>().Subscribe(OnCreateZone);

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
            _guardViewModel = new GuardViewModel();
        }

        static void OnShowDevice(Guid deviceUID)
        {
            if (deviceUID != Guid.Empty)
                _devicesViewModel.Select(deviceUID);
            ServiceFactory.Layout.Show(_devicesViewModel);
        }

        static void OnShowZone(ulong zoneNo)
        {
            _zonesViewModel.SelectedZone = _zonesViewModel.Zones.FirstOrDefault(x => x.No == zoneNo);
            ServiceFactory.Layout.Show(_zonesViewModel);
        }

        static void OnShowDirections(int? directionId)
        {
            if (directionId.HasValue)
                _directionsViewModel.SelectedDirection = _directionsViewModel.Directions.FirstOrDefault(x => x.Direction.Id == directionId);
            ServiceFactory.Layout.Show(_directionsViewModel);
        }

        static void OnShowGuardDevices(object obj)
        {
            ServiceFactory.Layout.Show(_guardViewModel);
        }

        static void OnCreateZone(CreateZoneEventArg createZoneEventArg)
        {
            _zonesViewModel.CreateZone(createZoneEventArg);
        }
    }
}