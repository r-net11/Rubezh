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
            alarm.Description = Description;

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
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, Name = "ИП 212-64", Description = "Сработал дымовой датчик", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, Name = "ИПР", Description = "Сработал ручной извещатель", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, Name = "ИП 101-29", Description = "Сработал тепловой извещатель", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Failure, Name = "Вскрытие", Description = "Всрытие прибора Рубеж-2АМ", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Info, Name = "Вход", Description = "Вход пользователя в систему", Time = DateTime.Now.ToString() });
        }

        void AddTestAlarm(Alarm alarm)
        {
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }
    }
}
