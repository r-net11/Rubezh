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
            alarm.Name = Name;

            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }

        static void AddTestAlarms()
        {
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, Name = " Сработал дымовой датчик ИП 212-64", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, Name = "Сработал ручной извещатель ИПР", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Fire, Name = " Сработал тепловой извещательИП 101-29", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Failure, Name = "Всрытие прибора Рубеж-2АМ", Time = DateTime.Now.ToString() });
            AddTestAlarm(new Alarm() { AlarmType = AlarmType.Info, Name = "Вход пользователя в систему", Time = DateTime.Now.ToString() });
        }

        static void AddTestAlarm(Alarm alarm)
        {
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }
    }
}