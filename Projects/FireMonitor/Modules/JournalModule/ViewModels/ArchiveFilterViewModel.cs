using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class ArchiveFilterViewModel : DialogContent
    {
        public ArchiveFilterViewModel()
        {
            Initialize();
        }

        void Initialize()
        {
            Title = "Фильтр архива";

            IsClear = true;
            StartDate = EndDate = StartTime = EndTime = DateTime.Now;

            Subsystems = new List<SubsystemViewModel>();
            foreach (SubsystemType subsystem in Enum.GetValues(typeof(SubsystemType)))
            {
                Subsystems.Add(new SubsystemViewModel(subsystem));
            }

            JournalEvents = new List<EventViewModel>(
                FiresecClient.FiresecManager.GetDistinctRecords().
                Select(journalRecord => new EventViewModel(journalRecord.StateType, journalRecord.Description))
            );

            JournalTypes = new List<ClassViewModel>(
                JournalEvents.Select(x => x.ClassId).Distinct().
                Select(x => new ClassViewModel((StateType) x))
            );

            FiresecEventSubscriber.NewJournalRecordEvent +=
                new Action<JournalRecord>(OnNewJournaRecordEvent);

            ClearCommand = new RelayCommand(OnClear);
            SaveCommand = new RelayCommand(OnSave, CanSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public List<ClassViewModel> JournalTypes { get; private set; }
        public List<EventViewModel> JournalEvents { get; private set; }
        public List<SubsystemViewModel> Subsystems { get; private set; }

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
                _startDate = new DateTime(value.Year, value.Month, value.Day,
                                          _startDate.Hour, _startDate.Minute, _startDate.Second);
                OnPropertyChanged("StartDate");
            }
        }
        public DateTime StartTime
        {
            get { return _startDate; }
            set
            {
                _startDate = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day,
                                            value.Hour, value.Minute, value.Second);
                OnPropertyChanged("StartTime");
            }
        }

        DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = new DateTime(value.Year, value.Month, value.Day,
                                        _endDate.Hour, _endDate.Minute, _endDate.Second); ;
                OnPropertyChanged("EndDate");
            }
        }
        public DateTime EndTime
        {
            get { return _endDate; }
            set
            {
                _endDate = new DateTime(_endDate.Year, _endDate.Month, _endDate.Day,
                                          value.Hour, value.Minute, value.Second);
                OnPropertyChanged("EndTime");
            }
        }

        public bool IsClear { get; set; }

        void OnNewJournaRecordEvent(JournalRecord newRecord)
        {
            if (JournalEvents.Any(x => x.Name == newRecord.Description) == false)
            {
                JournalEvents.Add(new EventViewModel(newRecord.StateType, newRecord.Description));
                if (JournalTypes.Any(x => x.Id == newRecord.StateType) == false)
                    JournalTypes.Add(new ClassViewModel(newRecord.StateType));
            }
        }

        public void CopyTo(ArchiveFilterViewModel dest)
        {
            dest.JournalEvents = new List<EventViewModel>(
                JournalEvents.Select(x => new EventViewModel(x.ClassId, x.Name) { IsEnable = x.IsEnable })
            );
            dest.JournalTypes = new List<ClassViewModel>(
                JournalTypes.Select(x => new ClassViewModel(x.Id) { IsEnable = x.IsEnable })
            );
            dest.Subsystems = new List<SubsystemViewModel>(
                Subsystems.Select(x => new SubsystemViewModel(x.Subsystem) { IsEnable = x.IsEnable })
            );
            dest.UseSystemDate = UseSystemDate;
            dest.StartDate = StartDate;
            dest.StartTime = StartTime;
            dest.EndDate = EndDate;
            dest.EndTime = EndTime;
            dest.IsClear = IsClear;
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
            IsClear = false;
            Close(true);
        }

        bool CanSave()
        {
            return JournalEvents.Any(x => x.IsEnable == true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}