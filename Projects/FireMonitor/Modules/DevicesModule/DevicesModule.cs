using System;
using DevicesModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;

namespace DevicesModule
{
    public class DevicesModule : IModule
    {
        static DevicesViewModel DevicesViewModel;
        static ZonesViewModel ZonesViewModel;

        public DevicesModule()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(OnShowDevice);
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(OnShowZone);
            ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Subscribe(OnShowDeviceDetails);
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

        static void CreateViewModels()
        {
            DevicesViewModel = new DevicesViewModel();
            DevicesViewModel.Initialize();

            ZonesViewModel = new ZonesViewModel();
            ZonesViewModel.Initialize();
        }

        static void OnShowDevice(Guid deviceUID)
        {
            DevicesViewModel.Select(deviceUID);
            ServiceFactory.Layout.Show(DevicesViewModel);
        }

        static void OnShowZone(ulong? zoneNo)
        {
            ZonesViewModel.Select(zoneNo);
            ServiceFactory.Layout.Show(ZonesViewModel);
        }

        static void OnShowDeviceDetails(Guid deviceUID)
        {
            ServiceFactory.UserDialogs.ShowWindow(new DeviceDetailsViewModel(deviceUID));
        }
    }
}