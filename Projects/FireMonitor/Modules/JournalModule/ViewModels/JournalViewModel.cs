using System.Collections.Generic;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using FiresecAPI.Models;
using System;

namespace JournalModule.ViewModels
{
    public class JournalViewModel : RegionViewModel
    {
        public void Initialize()
        {
            JournalItems = new ObservableCollection<JournalItemViewModel>();
            List<JournalItem> journalItems = FiresecManager.ReadJournal(0, 100);
            foreach (var journalItem in journalItems)
            {
                JournalItemViewModel journalItemViewModel = new JournalItemViewModel(journalItem);
                JournalItems.Add(journalItemViewModel);
            }

            FiresecEventSubscriber.NewJournalItemEvent += new Action<JournalItem>(OnNewJournalItemEvent);
        }

        void OnNewJournalItemEvent(JournalItem journalItem)
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
    }
}
