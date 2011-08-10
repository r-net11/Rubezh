using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class FilteredJournalViewModel : RegionViewModel
    {
        public static readonly int RecordsMaxCount = 100;
        readonly JournalFilterViewModel _journalFilter;

        public FilteredJournalViewModel(JournalFilter journalFilter)
        {
            Name = journalFilter.Name;
            _journalFilter = new JournalFilterViewModel(journalFilter);

            Initialize();
        }

        void Initialize()
        {
            JournalRecords = new ObservableCollection<JournalRecordViewModel>();
            FiresecEventSubscriber.NewJournalRecordEvent +=
                new Action<JournalRecord>(OnNewJournaRecordEvent);

            ThreadPool.QueueUserWorkItem(new WaitCallback(AplyFilter));
        }

        public string Name { get; private set; }

        ObservableCollection<JournalRecordViewModel> _journalRecords;
        public ObservableCollection<JournalRecordViewModel> JournalRecords
        {
            get { return _journalRecords; }
            set
            {
                _journalRecords = value;
                OnPropertyChanged("JournalRecords");
            }
        }

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

        void AplyFilter(Object stateInfo)
        {
            List<JournalRecord> journalRecords = null;
            bool isFiltered = false;
            int startIndex = 0;

            //while (isFiltered == false &&
            //    (journalRecords = FiresecManager.ReadJournal(startIndex, RecordsMaxCount)) != null)
            journalRecords = FiresecManager.ReadJournal(startIndex, 300);
            {
                foreach (var journalRecord in journalRecords)
                {
                    if (isFiltered = FilterRecord(journalRecord))
                        break;
                }

                startIndex += RecordsMaxCount;
            }
        }

        bool FilterRecord(JournalRecord journalRecord)
        {
            if (_journalFilter.CheckDaysConstraint(journalRecord))
            {
                if (_journalFilter.FilterRecord(journalRecord))
                {
                    Dispatcher.Invoke(new Action(() =>
                        JournalRecords.Add(new JournalRecordViewModel(journalRecord))), null);
                }

                return JournalRecords.Count >= _journalFilter.RecordsCount;
            }

            return false;
        }

        void OnNewJournaRecordEvent(JournalRecord journalRecord)
        {
            if (JournalRecords.Count > 0)
            {
                Dispatcher.Invoke(new Action(() =>
                    JournalRecords.Insert(0, new JournalRecordViewModel(journalRecord))), null);
            }
            else
            {
                Dispatcher.Invoke(new Action(() =>
                    JournalRecords.Add(new JournalRecordViewModel(journalRecord))), null);
            }

            if (JournalRecords.Count > _journalFilter.RecordsCount)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                    JournalRecords.RemoveAt(_journalFilter.RecordsCount)), null);
            }
        }
    }
}