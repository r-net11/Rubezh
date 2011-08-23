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
        public JournalFilterViewModel JournalFilterViewModel { get; private set; }

        public FilteredJournalViewModel(JournalFilter journalFilter)
        {
            if (journalFilter == null)
                throw new ArgumentNullException();

            JournalFilterViewModel = new JournalFilterViewModel(journalFilter);

            Initialize();
        }

        void Initialize()
        {
            JournalRecords = new ObservableCollection<JournalRecordViewModel>();
            FiresecEventSubscriber.NewJournalRecordEvent +=
                new Action<JournalRecord>(OnNewJournaRecordEvent);

            ThreadPool.QueueUserWorkItem(new WaitCallback(AplyFilter));
        }

        public string Name
        {
            get { return JournalFilterViewModel.JournalFilter.Name; }
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

        public static int RecordsMaxCount
        {
            get { return new JournalFilter().LastRecordsCount; }
        }

        void AplyFilter(Object stateInfo)
        {
            List<JournalRecord> journalRecords = null;
            bool isFiltered = false;
            int startIndex = 0;
            do
            {
                journalRecords = FiresecManager.ReadJournal(startIndex, RecordsMaxCount);
                foreach (var journalRecord in journalRecords)
                {
                    if (isFiltered = FilterRecord(journalRecord))
                        break;
                }
                startIndex += RecordsMaxCount;
            } while (isFiltered == false && journalRecords.Count == RecordsMaxCount);
        }

        bool FilterRecord(JournalRecord journalRecord)
        {
            if (JournalFilterViewModel.CheckDaysConstraint(journalRecord))
            {
                if (JournalFilterViewModel.FilterRecord(journalRecord))
                {
                    Dispatcher.Invoke(new Action(() =>
                        JournalRecords.Add(new JournalRecordViewModel(journalRecord))), null);
                }

                return JournalRecords.Count >= JournalFilterViewModel.RecordsCount;
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

            if (JournalRecords.Count > JournalFilterViewModel.RecordsCount)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                    JournalRecords.RemoveAt(JournalFilterViewModel.RecordsCount)), null);
            }
        }
    }
}