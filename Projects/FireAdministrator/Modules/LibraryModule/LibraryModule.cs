using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure;
using Infrastructure.Events;
using LibraryModule.ViewModels;

namespace LibraryModule
{
    public class LibraryModule : IModule
    {
        public LibraryModule()
        {
            ServiceFactory.Events.GetEvent<ShowLibraryEvent>().Subscribe(OnShowLibrary);
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
            libraryViewModel = new LibraryViewModel();
            libraryViewModel.Initialize();
        }

        static LibraryViewModel libraryViewModel;

        static void OnShowLibrary(string obj)
        {
            ServiceFactory.Layout.Show(libraryViewModel);
        }
    }
}
