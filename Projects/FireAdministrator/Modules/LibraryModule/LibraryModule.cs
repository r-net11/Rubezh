using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using LibraryModule.ViewModels;
using Microsoft.Practices.Prism.Modularity;

namespace LibraryModule
{
    public class LibraryModule : IModule
    {
        static LibraryViewModel _libraryViewModel;

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
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Themes/ListBoxStyle.xaml"));
        }

        static void CreateViewModels()
        {
            _libraryViewModel = new LibraryViewModel();
            _libraryViewModel.Initialize();
        }

        static void OnShowLibrary(string obj)
        {
            ServiceFactory.Layout.Show(_libraryViewModel);
        }
    }
}