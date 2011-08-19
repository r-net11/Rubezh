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
            ServiceFactory.Events.GetEvent<ShowGuardEvent>().Subscribe(OnShowGuard);
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

            guardViewModel = new GuardViewModel();
            guardViewModel.Initialize();
        }

        static DevicesViewModel devicesViewModel;
        static ZonesViewModel zonesViewModel;
        static DirectionsViewModel directionsViewModel;
        static GuardViewModel guardViewModel;

        static void OnShowDevice(string id)
        {
            if (string.IsNullOrEmpty(id) == false)
            {
                devicesViewModel.Select(id);
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

        static void OnShowGuard(string obj)
        {
            ServiceFactory.Layout.Show(guardViewModel);
        }

        public static bool HasChanges { get; set; }

        public static void Validate()
        {
            var validationErrorsViewModel = new ValidationErrorsViewModel();
            ServiceFactory.Layout.ShowValidationArea(validationErrorsViewModel);
        }
    }
}