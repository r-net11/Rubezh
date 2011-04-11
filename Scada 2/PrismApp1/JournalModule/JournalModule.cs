using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure;
using Infrastructure.Events;
using JournalModule.ViewModels;

namespace JournalModule
{
    public class JournalModule : IModule
    {
        public JournalModule()
        {
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(ShowJournal);
        }

        public void Initialize()
        {
            RegisterResources();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Brushes.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/DataGrid.xaml"));
        }

        static void ShowJournal(object obj)
        {
            JournalViewModel journalViewModel = new JournalViewModel();
            //journalViewModel.Initialize();
            ServiceFactory.Layout.Show(journalViewModel);
        }
    }
}
