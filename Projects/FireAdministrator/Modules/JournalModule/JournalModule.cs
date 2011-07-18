using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using JournalModule.ViewModels;
using Microsoft.Practices.Prism.Modularity;

namespace JournalModule
{
    public class JournalModule : IModule
    {
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
            journalViewModel = new JournalViewModel();
            journalViewModel.Initialize();
        }

        static JournalViewModel journalViewModel;

        static void OnShowJournal(string obj)
        {
            ServiceFactory.Layout.Show(journalViewModel);
        }
    }
}
