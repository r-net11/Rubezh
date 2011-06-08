using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Common;
using AlarmModule.Events;
using Firesec;

namespace AlarmModule.Imitator
{
    public class AlarmImitatorViewModel : BaseViewModel
    {
        public AlarmImitatorViewModel()
        {
            CreateCommand = new RelayCommand(OnCreate);
            AddTestAlarms();
        }

        public RelayCommand CreateCommand { get; private set; }
        void OnCreate()
        {
            Alarm alarm = new Alarm();
            alarm.AlarmType = AlarmType;
            alarm.Name = Name;

            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }

        AlarmType alarmType;
        public AlarmType AlarmType
        {
            get { return alarmType; }
            set
            {
                alarmType = value;
                OnPropertyChanged("AlarmType");
            }
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        void AddTestAlarms()
        {
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, Name = " Сработал дымовой датчик ИП 212-64", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, Name = "Сработал ручной извещатель ИПР", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, Name = " Сработал тепловой извещательИП 101-29", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Failure, Name = "Всрытие прибора Рубеж-2АМ", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Info, Name = "Вход пользователя в систему", Time = DateTime.Now.ToString() });
        }

        void AddTestAlarm(Alarm alarm)
        {
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }
    }
}
