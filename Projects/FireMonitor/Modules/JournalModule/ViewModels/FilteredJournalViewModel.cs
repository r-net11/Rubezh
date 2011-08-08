using System;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class FilteredJournalViewModel : RegionViewModel
    {
        static readonly int RecordsMaxCount = 100;

        public void Initialize()
        {
            JournalItems = new ObservableCollection<JournalRecordViewModel>();
            foreach (var journalRecord in FiresecManager.ReadJournal(0, RecordsMaxCount))
            {
                JournalRecordViewModel journalItemViewModel = new JournalRecordViewModel(journalRecord);
                JournalItems.Add(journalItemViewModel);
            }

            FiresecEventSubscriber.NewJournalItemEvent += new Action<JournalRecord>(OnNewJournalItemEvent);
        }

        void OnNewJournalItemEvent(JournalRecord journalRecord)
        {
            JournalRecordViewModel journalItemViewModel = new JournalRecordViewModel(journalRecord);
            if (JournalItems.Count > 0)
                JournalItems.Insert(0, journalItemViewModel);
            else
                JournalItems.Add(journalItemViewModel);

            if (JournalItems.Count > RecordsMaxCount)
                JournalItems.RemoveAt(RecordsMaxCount);
        }

        ObservableCollection<JournalRecordViewModel> _journalItems;
        public ObservableCollection<JournalRecordViewModel> JournalItems
        {
            get { return _journalItems; }
            set
            {
                _journalItems = value;
                OnPropertyChanged("JournalItems");
            }
        }

        JournalRecordViewModel _selectedItem;
        public JournalRecordViewModel SelectedItem
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
