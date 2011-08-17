using System.Collections.Generic;
using System.Linq;
using AlarmModule.Events;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

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

        public string Name { get; set; }
        public AlarmType AlarmType { get; set; }
        public List<Alarm> Alarms { get; set; }

        void OnAlarmAdded(Alarm alarm)
        {
            if (alarm.AlarmType == AlarmType)
            {
                Alarms.Add(alarm);
            }
            Update();
        }

        void OnResetAlarm(Alarm alarm)
        {
            if (alarm.AlarmType == this.AlarmType)
            {
                var removingAlarm = Alarms.FirstOrDefault(x => x.DeviceId == alarm.DeviceId);
                Alarms.Remove(removingAlarm);
            }
            Update();
        }

        public RelayCommand ShowCommand { get; private set; }
        void OnShow()
        {
            //'AlarmModule.ViewModels.AlarmGroupViewModel.OnShow()' скрывает наследуемый член 'Infrastructure.Common.RegionViewModel.OnShow()'
            //Чтобы текущий член переопределял эту реализацию, добавьте ключевое слово override. В противном случае добавьте ключевое слово new

            var alarmListViewModel = new AlarmListViewModel();
            alarmListViewModel.Initialize(Alarms, AlarmType);
            ServiceFactory.Layout.Show(alarmListViewModel);
            ServiceFactory.Events.GetEvent<ShowNothingEvent>().Publish(null);
        }

        public void Update()
        {
            OnPropertyChanged("Alarms");
            OnPropertyChanged("Count");
            OnPropertyChanged("HasAlarms");
        }

        public int Count
        {
            get { return Alarms.Count; }
        }

        public bool HasAlarms
        {
            get { return (Count > 0); }
        }
    }
}