using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
    public class FilteredJournalViewModel : RegionViewModel
    {
        public JournalFilter JournalFilter { get; private set; }

        public FilteredJournalViewModel(JournalFilter journalFilter)
        {
            ServiceFactory.Events.GetEvent<NewJournalRecordEvent>().Subscribe(OnNewJournalRecord);

            if (journalFilter == null)
                return;
            JournalFilter = journalFilter;

            Initialize();
        }

        void Initialize()
        {
            try
            {
                var journalRecords = FiresecManager.GetFilteredJournal(JournalFilter);
                var journalRecordsViewModel = journalRecords.Select(journalRecord => new JournalRecordViewModel(journalRecord));
                JournalRecords = new ObservableCollection<JournalRecordViewModel>(journalRecordsViewModel);
            }
            catch(Exception)
            {
                return;
            }
        }

        public string Name
        {
            get { return JournalFilter.Name; }
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

        public bool FilterRecord(JournalRecord journalRecord)
        {
            bool result = true;
            if (JournalFilter.Categories.IsNotNullOrEmpty())
            {
                Device device = null;
                if (string.IsNullOrWhiteSpace(journalRecord.DeviceDatabaseId) == false)
                {
                    device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.DatabaseId == journalRecord.DeviceDatabaseId);
                }
                else
                {
                    device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.DatabaseId == journalRecord.PanelDatabaseId);
                }

                if ((result = (device != null)))
                {
                    result = JournalFilter.Categories.Any(daviceCategory => daviceCategory == device.Driver.Category);
                }
            }

            if (result && JournalFilter.Events.IsNotNullOrEmpty())
            {
                result = JournalFilter.Events.Any(_event => _event == journalRecord.StateType);
            }

            return result;
        }

        void OnNewJournalRecord(JournalRecord journalRecord)
        {
            if (!FilterRecord(journalRecord))
                return;

            if (JournalRecords.Count > 0)
                JournalRecords.Insert(0, new JournalRecordViewModel(journalRecord));
            else
                JournalRecords.Add(new JournalRecordViewModel(journalRecord));

            if (JournalRecords.Count > JournalFilter.LastRecordsCount)
                JournalRecords.RemoveAt(JournalFilter.LastRecordsCount);
        }
    }
}