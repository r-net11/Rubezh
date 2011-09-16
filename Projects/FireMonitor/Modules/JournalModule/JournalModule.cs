using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using JournalModule.ViewModels;
using Microsoft.Practices.Prism.Modularity;

namespace JournalModule
{
    public class JournalModule : IModule
    {
        static JournalsViewModel JournalsViewModel;
        static ArchiveViewModel ArchiveViewModel;

        public JournalModule()
        {
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
            ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(OnShowArchive);
        }

        public void Initialize()
        {
            JournalsViewModel = new JournalsViewModel();
            ArchiveViewModel = new ArchiveViewModel();

            RegisterResources();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void OnShowJournal(object obj)
        {
            JournalsViewModel.SelectedJournal = JournalsViewModel.Journals[0];
            ServiceFactory.Layout.Show(JournalsViewModel);
        }

        static void OnShowArchive(object obj)
        {
            ServiceFactory.Layout.Show(ArchiveViewModel);
        }
    }
}