using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure;
using Infrastructure.Events;
using DevicesModule.ViewModels;
using ClientApi;

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
            ClientManager.Start();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Brushes.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/DataGrid.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/TreeExpanderStyle.xaml"));
        }

        static void OnShowDevices(object obj)
        {
            DevicesViewModel devicesViewModel = new DevicesViewModel();
            devicesViewModel.Initilize();
            ServiceFactory.Layout.Show(devicesViewModel);
        }

        static void OnShowZones(object obj)
        {
            ZonesViewModel zonesViewModel = new ZonesViewModel();
            zonesViewModel.Initialize();
            ServiceFactory.Layout.Show(zonesViewModel);
        }
    }
}
