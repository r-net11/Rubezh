using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure;
using AlarmModule.Events;
using Infrastructure.Common;

namespace AlarmModule.ViewModels
{
    public class AlarmGroupViewModel : RegionViewModel
    {
        public AlarmGroupViewModel()
        {
            Alarms = new List<Alarm>();
            ShowCommand = new RelayCommand(OnShow);
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Subscribe(OnAlarmAdded);
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Subscribe(OnResetAlarm);
        }

        public AlarmType AlarmType { get; set; }
        public string Name { get; set; }
        public List<Alarm> Alarms { get; set; }

        void OnAlarmAdded(Alarm alarm)
        {
            if (alarm.AlarmType == this.AlarmType)
            {
                Alarms.Add(alarm);
            }
            Update();
        }

        void OnResetAlarm(Alarm alarm)
        {
            if (alarm.AlarmType == this.AlarmType)
            {
                Alarms.Remove(alarm);
            }
            Update();
        }

        public RelayCommand ShowCommand { get; private set; }
        void OnShow()
        {
            AlarmListViewModel alarmListViewModel = new AlarmListViewModel();
            alarmListViewModel.Initialize(Alarms);
            ServiceFactory.Layout.Show(alarmListViewModel);
        }

        public void Update()
        {
            OnPropertyChanged("Alarms");
            OnPropertyChanged("Count");
            OnPropertyChanged("HasAlarms");
        }

        public int Count
        {
            get
            {
                return Alarms.Count;
            }
        }

        public bool HasAlarms
        {
            get
            {
                return (Count > 0);
            }
        }

        public override void Dispose()
        {
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Unsubscribe(OnAlarmAdded);
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Unsubscribe(OnResetAlarm);
        }
    }
}
