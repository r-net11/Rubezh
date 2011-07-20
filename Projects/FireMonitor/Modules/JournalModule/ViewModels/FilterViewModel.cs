using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace JournalModule.ViewModels
{
    public class FilterViewModel : DialogContent
    {
        ObservableCollection<JournalItemViewModel> _journalItems;

        public FilterViewModel()
        {
            Title = "Фильтр журнала";

            ClearCommand = new RelayCommand(OnClear);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);

            StartDate = EndDate = StartTime = EndTime = DateTime.Now;
        }

        public void Initialize(ObservableCollection<JournalItemViewModel> journalItems)
        {
            _journalItems = journalItems;

            var stringJournalTypes = (from journalItem in _journalItems
                                      select journalItem.State).Distinct();

            JournalTypes = (from journalType in stringJournalTypes
                            select new ClassViewModel(journalType)).ToList();

            var stringJournalEvents = (from journalItem in _journalItems
                                       select journalItem.Description).Distinct();

            JournalEvents = (from journalEvent in stringJournalEvents
                             select new EventViewModel(journalEvent)).ToList();
        }

        public List<ClassViewModel> JournalTypes { get; set; }
        public List<EventViewModel> JournalEvents { get; set; }

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

            JournalTypes.ForEach(x => x.IsEnabled = false);
            JournalEvents.ForEach(x => x.IsEnabled = false);
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            var startDateTime = StartDate.Date + StartTime.TimeOfDay;
            var endDateTime = EndDate.Date + EndTime.TimeOfDay;
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
