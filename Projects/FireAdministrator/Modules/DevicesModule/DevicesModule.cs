using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure;
using DevicesModule.ViewModels;
using Infrastructure.Events;

namespace DevicesModule
{
    public class DevicesModule : IModule
    {
        public DevicesModule()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(OnShowDevice);
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
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/MenuStyle.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/TreeExpanderStyle.xaml"));
        }

        static void CreateViewModels()
        {
            devicesViewModel = new DevicesViewModel();
            devicesViewModel.OnConnect(null);
            //devicesViewModel.Initialize();
        }

        static DevicesViewModel devicesViewModel;

        static void OnShowDevice(string path)
        {
            ServiceFactory.Layout.Show(devicesViewModel);
        }
    }
}
