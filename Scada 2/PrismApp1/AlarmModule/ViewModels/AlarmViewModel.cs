using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;

namespace AlarmModule.ViewModels
{
    public class AlarmViewModel : BaseViewModel
    {
        public AlarmViewModel()
        {
            ShowCommand = new RelayCommand(OnShow);
        }

        public Alarm alarm;
        public AlarmType AlarmType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public void Initialize(Alarm alarm)
        {
            this.alarm = alarm;
            AlarmType = alarm.AlarmType;
            Name = alarm.Name;
            Description = alarm.Description;
        }

        public RelayCommand ShowCommand { get; private set; }
        void OnShow()
        {
            AlarmDetailsViewModel alarmDetailsViewModel = new AlarmDetailsViewModel();
            alarmDetailsViewModel.Initialize(alarm, false);
            ServiceFactory.Layout.Show(alarmDetailsViewModel);
        }
    }
}
