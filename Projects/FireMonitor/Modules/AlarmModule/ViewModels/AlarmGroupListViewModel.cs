using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            AlarmGroups = new ObservableCollection<AlarmGroupViewModel>();
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Пожар", AlarmType = AlarmType.Fire });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Внимание", AlarmType = AlarmType.Attention });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Неисправность", AlarmType = AlarmType.Failure });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Отключение", AlarmType = AlarmType.Off });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Информация", AlarmType = AlarmType.Info });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Обслуживание", AlarmType = AlarmType.Service });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Автоматика", AlarmType = AlarmType.Auto });

            ServiceFactory.Events.GetEvent<ShowAllAlarmsEvent>().Subscribe(OnShowAllAlarms);
        }

        public ObservableCollection<AlarmGroupViewModel> AlarmGroups { get; set; }

        void OnShowAllAlarms(object obj)
        {
            List<Alarm> alarms = new List<Alarm>();
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