using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common;
using JournalModule.ViewModels;

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
