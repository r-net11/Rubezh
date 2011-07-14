using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Events;
using JournalModule.ViewModels;
using FiresecClient.Models;

namespace JournalModule
{
    public class JournalModule : IModule
    {
        public JournalModule()
        {
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
            ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(OnShowArchive);
        }

        public void Initialize()
        {
            RegisterResources();
            CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void OnShowJournal(object obj)
        {
            UnreadCount = 0;
            JournalViewModel journalViewModel = new JournalViewModel();
            journalViewModel.Initialize();
            ServiceFactory.Layout.Show(journalViewModel);
        }

        static void OnShowArchive(object obj)
        {
            ArchiveViewModel archiveViewModel = new ArchiveViewModel();
            archiveViewModel.Initialize();
            ServiceFactory.Layout.Show(archiveViewModel);
        }

        static int UnreadCount = 0;

        static void CurrentStates_NewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            UnreadCount++;
        }
    }
}
