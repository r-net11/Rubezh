using System;
using AlarmModule.Events;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;

namespace AlarmModule.Imitator
{
    public class AlarmImitatorViewModel : BaseViewModel
    {
        public AlarmImitatorViewModel()
        {
            CreateCommand = new RelayCommand(OnCreate);
            //AddTestAlarms();
        }

        AlarmType _alarmType;
        public AlarmType AlarmType
        {
            get { return _alarmType; }
            set
            {
                _alarmType = value;
                OnPropertyChanged("AlarmType");
            }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        void OnCreate()
        {
            var alarm = new Alarm();
            alarm.AlarmType = AlarmType;
            alarm.StateName = Description;

            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }

        static void AddTestAlarms()
        {
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, StateName = " Сработал дымовой датчик ИП 212-64", Time = DateTime.Now });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, StateName = "Сработал ручной извещатель ИПР", Time = DateTime.Now });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, StateName = " Сработал тепловой извещательИП 101-29", Time = DateTime.Now });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Failure, StateName = "Всрытие прибора Рубеж-2АМ", Time = DateTime.Now });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Info, StateName = "Вход пользователя в систему", Time = DateTime.Now });
        }

        static void AddTestAlarm(Alarm alarm)
        {
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }
    }
}