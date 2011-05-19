using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common;
using SoundsModule.ViewModels;

namespace SoundsModule
{
    public class SoundsModule : IModule
    {
        public SoundsModule()
        {
            ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Subscribe(OnShowSounds);
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
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/DataGrid.xaml"));
        }

        static void CreateViewModels()
        {
            soundsViewModel = new SoundsViewModel();
            soundsViewModel.Initialize();
        }

        static SoundsViewModel soundsViewModel;

        static void OnShowSounds(string obj)
        {
            ServiceFactory.Layout.Show(soundsViewModel);
        }
    }
}
