using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace WpfApplication4
{
    public class AlarmImitatorViewModel : BaseViewModel
    {
        public AlarmImitatorViewModel()
        {
            CreateCommand = new RelayCommand(OnCreate);
        }

        public RelayCommand CreateCommand { get; private set; }
        void OnCreate(object obj)
        {
            Alarm alarm = new Alarm();
            alarm.AlarmType = AlarmType;
            alarm.Name = Name;
            alarm.Description = Description;

            ViewModel.Current.AddAlarm(alarm);
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
    }
}
