using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using FiltersModule.ViewModels;
using Microsoft.Practices.Prism.Modularity;

namespace FiltersModule
{
    public class FiltersModule : IModule
    {
        static FiltersViewModel journalViewModel;

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
            journalViewModel = new FiltersViewModel();
        }

        static void OnShowJournal(string obj)
        {
            journalViewModel.Initialize();
            ServiceFactory.Layout.Show(journalViewModel);
        }

        public static void Validate()
        {
        }
    }
}
