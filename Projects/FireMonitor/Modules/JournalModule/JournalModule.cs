using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using JournalModule.ViewModels;
using Microsoft.Practices.Prism.Modularity;

namespace JournalModule
{
    public class JournalModule : IModule
    {
        static JournalsViewModel _journalsViewModel;
        static ArchiveViewModel _archiveViewModel;

        public JournalModule()
        {
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
            ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(OnShowArchive);
        }

        public void Initialize()
        {
            _journalsViewModel = new JournalsViewModel();
            _archiveViewModel = new ArchiveViewModel();

            RegisterResources();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void OnShowJournal(object obj)
        {
            _journalsViewModel.SelectedJournal = _journalsViewModel.Journals[0];
            ServiceFactory.Layout.Show(_journalsViewModel);
        }

        static void OnShowArchive(object obj)
        {
            ServiceFactory.Layout.Show(_archiveViewModel);
        }
    }
}