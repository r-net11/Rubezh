using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using JournalModule.ViewModels;

namespace JournalModule
{
    public class JournalModuleLoader
    {
        static JournalsViewModel JournalsViewModel;
        static ArchiveViewModel ArchiveViewModel;

        public JournalModuleLoader()
        {
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
            ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(OnShowArchive);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        public static void CreateViewModels()
        {
            JournalsViewModel = new JournalsViewModel();
            ArchiveViewModel = new ArchiveViewModel();
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