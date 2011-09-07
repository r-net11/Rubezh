using System;
using System.Collections.Generic;
using System.Linq;
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
            var alarmTypes = Enum.GetValues(typeof(AlarmType)).Cast<AlarmType>().ToList();
            AlarmGroups = new List<AlarmGroupViewModel>();
            foreach (var alarmType in alarmTypes)
            {
                AlarmGroups.Add(new AlarmGroupViewModel() { AlarmType = alarmType });
            }

            ServiceFactory.Events.GetEvent<ShowAllAlarmsEvent>().Subscribe(OnShowAllAlarms);
        }

        public List<AlarmGroupViewModel> AlarmGroups { get; set; }

        void OnShowAllAlarms(object obj)
        {
            var alarms = new List<Alarm>();
            foreach (var alarmGroupViewModel in AlarmGroups)
            {
                alarms.AddRange(alarmGroupViewModel.Alarms);
            }

            var alarmListViewModel = new AlarmListViewModel();
            alarmListViewModel.Initialize(alarms, null);
            ServiceFactory.Layout.Show(alarmListViewModel);
        }
    }
}