using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections.ObjectModel;
using Infrastructure;
using AlarmModule.Events;

namespace AlarmModule.ViewModels
{
    public class AlarmsViewModel : RegionViewModel
    {
        public AlarmsViewModel()
        {
            Alarms = new ObservableCollection<AlarmViewModel>();

            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Subscribe(OnResetAlarm);
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Subscribe(OnAlarmAdded);
        }

        void OnAlarmAdded(Alarm alarm)
        {
            AlarmViewModel detailAlarm = new AlarmViewModel();
            detailAlarm.Initialize(alarm);
            Alarms.Add(detailAlarm);
        }

        void OnResetAlarm(Alarm alarm)
        {
            AlarmViewModel detailAlarm = Alarms.FirstOrDefault(x=>x.alarm == alarm);
            Alarms.Remove(detailAlarm);
        }

        ObservableCollection<AlarmViewModel> alarms;
        public ObservableCollection<AlarmViewModel> Alarms
        {
            get { return alarms; }
            set
            {
                alarms = value;
                OnPropertyChanged("Alarms");
            }
        }

        public override void Dispose()
        {
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Unsubscribe(OnResetAlarm);
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Unsubscribe(OnAlarmAdded);
        }
    }
}
