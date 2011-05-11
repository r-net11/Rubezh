using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;

namespace JournalModule.ViewModels
{
    public class JournalViewModel : RegionViewModel
    {
        public void Initialize()
        {
            JournalItems = new ObservableCollection<JournalItemViewModel>();
            List<Firesec.ReadEvents.journalType> journalItems = FiresecManager.ReadJournal(0, 100);
            foreach (Firesec.ReadEvents.journalType journalItem in journalItems)
            {
                JournalItemViewModel journalItemViewModel = new JournalItemViewModel(journalItem);
                JournalItems.Add(journalItemViewModel);
            }

            CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
        }

        void CurrentStates_NewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            JournalItemViewModel journalItemViewModel = new JournalItemViewModel(journalItem);
            if (JournalItems.Count > 0)
                JournalItems.Insert(0, journalItemViewModel);
            else
                JournalItems.Add(journalItemViewModel);

            if (JournalItems.Count > 100)
                JournalItems.RemoveAt(100);
        }

        ObservableCollection<JournalItemViewModel> _journalItems;
        public ObservableCollection<JournalItemViewModel> JournalItems
        {
            get { return _journalItems; }
            set
            {
                _journalItems = value;
                OnPropertyChanged("JournalItems");
            }
        }

        JournalItemViewModel _selectedItem;
        public JournalItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public override void Dispose()
        {
        }
    }
}
