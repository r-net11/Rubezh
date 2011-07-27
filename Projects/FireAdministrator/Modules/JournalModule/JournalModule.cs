using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using JournalModule.ViewModels;
using Microsoft.Practices.Prism.Modularity;

namespace JournalModule
{
    public class JournalModule : IModule
    {
        static JournalsViewModel journalViewModel;

        public JournalModule()
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
            journalViewModel = new JournalsViewModel();
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
