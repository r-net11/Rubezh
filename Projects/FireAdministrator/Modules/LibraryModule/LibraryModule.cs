using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using LibraryModule.ViewModels;

namespace LibraryModule
{
    public class LibraryModule
    {
        public static bool HasChanges { get; set; }
        static LibraryViewModel _libraryViewModel;

        public LibraryModule()
        {
            ServiceFactory.Events.GetEvent<ShowLibraryEvent>().Subscribe(OnShowLibrary);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Themes/ListBoxStyle.xaml"));
        }

        static void CreateViewModels()
        {
            _libraryViewModel = new LibraryViewModel();
        }

        static void OnShowLibrary(string obj)
        {
            ServiceFactory.Layout.Show(_libraryViewModel);
        }
    }
}