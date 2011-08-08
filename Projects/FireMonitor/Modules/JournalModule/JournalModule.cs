using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using JournalModule.ViewModels;
using Microsoft.Practices.Prism.Modularity;

namespace JournalModule
{
    public class JournalModule : IModule
    {
        static FilteredJournalViewModel _journalViewModel;
        public JournalModule()
        {
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
            ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(OnShowArchive);
        }

        public void Initialize()
        {
            _journalViewModel = new FilteredJournalViewModel();
            _journalViewModel.Initialize();

            RegisterResources();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void OnShowJournal(object obj)
        {
            ServiceFactory.Layout.Show(_journalViewModel);
        }

        static void OnShowArchive(object obj)
        {
            ArchiveViewModel archiveViewModel = new ArchiveViewModel();
            archiveViewModel.Initialize();
            ServiceFactory.Layout.Show(archiveViewModel);
        }
    }
}
