using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Events;
using DevicesModule.ViewModels;
using FiresecClient;
using DevicesModule.Views;

namespace DevicesModule
{
    public class DevicesModule : IModule
    {
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
            devicesViewModel = new DevicesViewModel();
            devicesViewModel.Initialize();

            zonesViewModel = new ZonesViewModel();
            zonesViewModel.Initialize();
        }

        static DevicesViewModel devicesViewModel;
        static ZonesViewModel zonesViewModel;

        static void OnShowDevice(string id)
        {
            devicesViewModel.Select(id);
            ServiceFactory.Layout.Show(devicesViewModel);
        }

        static void OnShowZone(string zoneNo)
        {
            zonesViewModel.Select(zoneNo);
            ServiceFactory.Layout.Show(zonesViewModel);
        }

        static void OnShowDeviceDetails(string deviceId)
        {
            DeviceDetailsViewModel deviceDetailsViewModel = new DeviceDetailsViewModel(deviceId);
            ServiceFactory.UserDialogs.ShowModalWindow(deviceDetailsViewModel);
        }
    }
}
