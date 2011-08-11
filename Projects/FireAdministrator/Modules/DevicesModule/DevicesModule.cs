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
        }

        public void Initialize()
        {
            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        public static void CreateViewModels()
        {
            devicesViewModel = new DevicesViewModel();
            devicesViewModel.Initialize();

            zonesViewModel = new ZonesViewModel();
            zonesViewModel.Initialize();

            directionsViewModel = new DirectionsViewModel();
            directionsViewModel.Initialize();
        }

        static DevicesViewModel devicesViewModel;
        static ZonesViewModel zonesViewModel;
        static DirectionsViewModel directionsViewModel;

        static void OnShowDevice(string id)
        {
            if (string.IsNullOrEmpty(id) != null)
            {
                devicesViewModel.Select(id);
            }
            ServiceFactory.Layout.Show(devicesViewModel);
        }

        static void OnShowZone(string zoneNo)
        {
            zonesViewModel.SelectedZone = zonesViewModel.Zones.FirstOrDefault(x => x.No == zoneNo);
            ServiceFactory.Layout.Show(zonesViewModel);
        }

        static void OnShowDirections(string zoneNo)
        {
            ServiceFactory.Layout.Show(directionsViewModel);
        }

        public static bool HasChanges { get; set; }

        public static void Validate()
        {
            ValidationErrorsViewModel validationErrorsViewModel = new ValidationErrorsViewModel();
            ServiceFactory.Layout.ShowValidationArea(validationErrorsViewModel);
        }
    }
}