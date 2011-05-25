using Microsoft.Practices.Prism.Modularity;
using Infrastructure.Common;
using Infrastructure.Events;
using LibraryModule.ViewModels;
using Infrastructure;

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
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/DataGrid.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Styles.xaml"));
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
