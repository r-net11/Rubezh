using System;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class FilteredJournalViewModel : RegionViewModel
    {
        readonly JournalFilter _journalFilter;

        public FilteredJournalViewModel(JournalFilter journalFilter)
        {
            if (journalFilter == null)
                throw new ArgumentNullException();
            _journalFilter = journalFilter;

            Initialize();
        }

        void Initialize()
        {
            JournalRecords = new ObservableCollection<JournalRecordViewModel>();
            FiresecEventSubscriber.NewJournalRecordEvent +=
                new Action<JournalRecord>(OnNewJournaRecordEvent);

            var journalRecords = FiresecManager.GetFilteredJournal(_journalFilter);
            if (journalRecords != null)
            {
                foreach (var journalRecord in journalRecords)
                    JournalRecords.Add(new JournalRecordViewModel(journalRecord));
            }
        }

        public string Name
        {
            get { return _journalFilter.Name; }
        }

        public static int RecordsMaxCount
        {
            get { return new JournalFilter().LastRecordsCount; }
        }

        public ObservableCollection<JournalRecordViewModel> JournalRecords { get; private set; }

        JournalRecordViewModel _selectedRecord;
        public JournalRecordViewModel SelectedRecord
        {
            get { return _selectedRecord; }
            set
            {
                _selectedRecord = value;
                OnPropertyChanged("SelectedRecord");
            }
        }

        void OnNewJournaRecordEvent(JournalRecord journalRecord)
        {
            if (JournalRecords.Count > 0)
            {
                JournalRecords.Insert(0, new JournalRecordViewModel(journalRecord));
            }
            else
            {
                JournalRecords.Add(new JournalRecordViewModel(journalRecord));
            }

            if (JournalRecords.Count > _journalFilter.LastRecordsCount)
            {
                JournalRecords.RemoveAt(_journalFilter.LastRecordsCount);
            }
        }
    }
}