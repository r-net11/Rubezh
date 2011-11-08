using System;
using System.Collections.Generic;
using AlarmModule.Events;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;

namespace AlarmModule.ViewModels
{
    public class AlarmGroupListViewModel : RegionViewModel
    {
        public AlarmGroupListViewModel()
        {
            AlarmGroups = new List<AlarmGroupViewModel>();
            foreach (AlarmType alarmType in Enum.GetValues(typeof(AlarmType)))
            {
                AlarmGroups.Add(new AlarmGroupViewModel() { AlarmType = alarmType });
            }

            ServiceFactory.Events.GetEvent<ShowAllAlarmsEvent>().Subscribe(OnShowAllAlarms);
        }

        public List<AlarmGroupViewModel> AlarmGroups { get; set; }

        void OnShowAllAlarms(object obj)
        {
            var alarms = new List<Alarm>();
            AlarmGroups.ForEach(x => alarms.AddRange(x.Alarms));

            ServiceFactory.Layout.Show(new AlarmListViewModel(alarms, null));
        }
    }
}