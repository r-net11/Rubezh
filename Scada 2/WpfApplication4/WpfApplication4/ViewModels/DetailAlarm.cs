using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace WpfApplication4
{
    public class DetailAlarm : BaseViewModel
    {
        public DetailAlarm()
        {
            ShowCommand = new RelayCommand(OnShow);
            ResetCommand = new RelayCommand(OnReset);
            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
        }

        public void Initialize(Alarm alarm)
        {
            AlarmType = alarm.AlarmType;
            Name = alarm.Name;
            Description = alarm.Description;
        }

        public RelayCommand ShowCommand { get; private set; }
        void OnShow(object obj)
        {
            AlarmView alarmView = new AlarmView();
            alarmView.DataContext = this;
            ViewModel.Current.Content = alarmView;
        }

        public RelayCommand ResetCommand { get; private set; }
        void OnReset(object obj)
        {
            ViewModel.Current.RemoveAlarm(this);
            ViewModel.Current.Content = null;
        }

        public RelayCommand ShowOnPlanCommand { get; private set; }
        void OnShowOnPlan(object obj)
        {
            PlanView planView = new PlanView();
            ViewModel.Current.Content = planView;
        }

        public AlarmType AlarmType { get; set; }

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

        public string Description { get; set; }
    }
}
