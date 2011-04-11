using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Collections.ObjectModel;
using ClientApi;

namespace JournalModule.ViewModels
{
    public class JournalViewModel : RegionViewModel
    {
        public void Initialize()
        {
            ServiceClient serviceClient = new ServiceClient();
            serviceClient.Start();

            JournalItemViewModels = new ObservableCollection<JournalItemViewModel>();
            List<Firesec.ReadEvents.journalType> journalItems = serviceClient.ReadJournal(0, 100);
            foreach (Firesec.ReadEvents.journalType journalItem in journalItems)
            {
                JournalItemViewModel journalItemViewModel = new JournalItemViewModel();
                journalItemViewModel.Initialize(journalItem);
                JournalItemViewModels.Add(journalItemViewModel);
            }
        }

        ObservableCollection<JournalItemViewModel> journalItemViewModels;
        public ObservableCollection<JournalItemViewModel> JournalItemViewModels
        {
            get { return journalItemViewModels; }
            set
            {
                journalItemViewModels = value;
                OnPropertyChanged("JournalItemViewModels");
            }
        }

        public override void Dispose()
        {
        }
    }
}
