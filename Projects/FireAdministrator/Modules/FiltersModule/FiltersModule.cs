using FiltersModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;

namespace FiltersModule
{
    public class FiltersModule : IModule
    {
        static FiltersViewModel _filtersViewModel;

        public FiltersModule()
        {
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
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

        void CreateViewModels()
        {
            _filtersViewModel = new FiltersViewModel();
            _filtersViewModel.Initialize();
        }

        static void OnShowJournal(string obj)
        {
            ServiceFactory.Layout.Show(_filtersViewModel);
        }

        public static void Validate()
        {
        }

        public static void Save()
        {
            _filtersViewModel.Save();
        }
    }
}