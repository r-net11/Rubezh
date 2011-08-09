using FiltersModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;

namespace FiltersModule
{
    public class FiltersModule : IModule
    {
        static FiltersViewModel filtersViewModel;

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
            filtersViewModel = new FiltersViewModel();
            filtersViewModel.Initialize();
        }

        static void OnShowJournal(string obj)
        {
            ServiceFactory.Layout.Show(filtersViewModel);
        }

        public static void Validate()
        {
        }

        public static void Save()
        {
            filtersViewModel.Save();
        }
    }
}