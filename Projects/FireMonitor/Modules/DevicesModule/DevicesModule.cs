using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
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
            ServiceFactory.Events.GetEvent<ShowDevicesEvent>().Subscribe(OnShowDevices);
            ServiceFactory.Events.GetEvent<ShowZonesEvent>().Subscribe(OnShowZones);
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
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Brushes.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/DataGrid.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/TreeExpanderStyle.xaml"));
        }

        static void CreateViewModels()
        {
            devicesViewModel = new DevicesViewModel();
            devicesViewModel.Initilize();

            zonesViewModel = new ZonesViewModel();
            zonesViewModel.Initialize();
        }

        static DevicesViewModel devicesViewModel;
        static ZonesViewModel zonesViewModel;

        static void OnShowDevices(string path)
        {
            devicesViewModel.Select(path);
            ServiceFactory.Layout.Show(devicesViewModel);
        }

        static void OnShowZones(string zoneNo)
        {
            zonesViewModel.Select(zoneNo);
            ServiceFactory.Layout.Show(zonesViewModel);
        }
    }
}
