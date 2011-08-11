using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class ArchiveFilterViewModel : DialogContent
    {
        ObservableCollection<JournalRecordViewModel> _journalRecords;

        public ArchiveFilterViewModel()
        {
            Title = "Фильтр журнала";

            ClearCommand = new RelayCommand(OnClear);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);

            StartDate = EndDate = StartTime = EndTime = DateTime.Now;
        }

        public void Initialize(ObservableCollection<JournalRecordViewModel> journalRecords)
        {
            _journalRecords = journalRecords;

            var stringJournalTypes = (from journalItem in _journalRecords
                                      select journalItem.ClassId).Distinct();

            JournalTypes = (from journalType in stringJournalTypes
                            select new ClassViewModel(journalType)).ToList();

            var stringJournalEvents = (from journalItem in _journalRecords
                                       select journalItem.Description).Distinct();

            JournalEvents = new List<EventViewModel>();
            foreach (var journalEvent in stringJournalEvents)
            {
                var jounalRecord = journalRecords.First(x => x.Description == journalEvent);
                JournalEvents.Add(new EventViewModel(jounalRecord.ClassId, jounalRecord.Description));
            }
        }

        public List<ClassViewModel> JournalTypes { get; private set; }
        public List<EventViewModel> JournalEvents { get; private set; }

        bool _useSystemDate;
        public bool UseSystemDate
        {
            get { return _useSystemDate; }
            set
            {
                _useSystemDate = value;
                OnPropertyChanged("UseSystemDate");
            }
        }

        DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }

        DateTime _endTime;
        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                OnPropertyChanged("EndTime");
            }
        }

        public RelayCommand ClearCommand { get; private set; }
        void OnClear()
        {
            StartDate = EndDate = StartTime = EndTime = DateTime.Now;
            UseSystemDate = false;

            JournalTypes.ForEach(x => x.IsEnable = false);
            JournalEvents.ForEach(x => x.IsEnable = false);
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            //var startDateTime = StartDate.Date + StartTime.TimeOfDay;
            //var endDateTime = EndDate.Date + EndTime.TimeOfDay;
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}