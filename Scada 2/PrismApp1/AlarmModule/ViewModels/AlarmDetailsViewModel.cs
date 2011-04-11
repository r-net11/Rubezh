using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using AlarmModule.Events;
using Infrastructure.Events;

namespace AlarmModule.ViewModels
{
    public class AlarmDetailsViewModel : RegionViewModel
    {
        public AlarmDetailsViewModel()
        {
            ResetCommand = new RelayCommand(OnReset);
            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
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

        public RelayCommand ResetCommand { get; private set; }
        void OnReset()
        {
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Publish(alarm);
        }

        public RelayCommand ShowOnPlanCommand { get; private set; }
        void OnShowOnPlan()
        {
            ServiceFactory.Events.GetEvent<ShowPlanEvent>().Publish(null);
        }

        public override void Dispose()
        {
        }
    }
}
