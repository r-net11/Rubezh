using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common;
using AlarmModule.Events;
using System.Diagnostics;
using FiresecClient;


namespace AlarmModule.ViewModels
{
    public class AlarmGroupListViewModel : RegionViewModel
    {
        public AlarmGroupListViewModel()
        {
            AlarmGroups = new ObservableCollection<AlarmGroupViewModel>();
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Пожар", AlarmType = AlarmType.Alarm });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Внимание", AlarmType = AlarmType.Attention });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Неисправность", AlarmType = AlarmType.Failure });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Отключение", AlarmType = AlarmType.Off });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Информация", AlarmType = AlarmType.Info });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Обслуживание", AlarmType = AlarmType.Service });
            AlarmGroups.Add(new AlarmGroupViewModel() { Name = "Автоматика", AlarmType = AlarmType.Auto });

            ServiceFactory.Events.GetEvent<ShowAllAlarmsEvent>().Subscribe(OnShowAllAlarms);

            CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);

            FiresecManager.States.DeviceStateChanged += new Action<string>(CurrentStates_DeviceStateChanged);
        }

        void CurrentStates_NewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            Alarm.CreateFromJournalEvent(journalItem);
        }

        public ObservableCollection<AlarmGroupViewModel> AlarmGroups { get; set; }

        void OnShowAllAlarms(object obj)
        {
            List<Alarm> alarms = new List<Alarm>();
            foreach (var alarmGroupViewModel in AlarmGroups)
            {
                alarms.AddRange(alarmGroupViewModel.Alarms);
            }

            AlarmListViewModel alarmListViewModel = new AlarmListViewModel();
            alarmListViewModel.Initialize(alarms);
            ServiceFactory.Layout.Show(alarmListViewModel);
        }

        void CurrentStates_DeviceStateChanged(string obj)
        {
            List<string> deviceIds = new List<string>();

            foreach (var deviceState in FiresecManager.States.DeviceStates)
            {
                foreach (var state in deviceState.InnerStates)
                {
                    if ((state.IsAutomatic) && (state.IsManualReset))
                    {
                        deviceIds.Add(deviceState.Id);
                        Alarm alarm = new Alarm();
                        alarm.AlarmType = AlarmType.Auto;
                        alarm.DeviceId = deviceState.Id;
                        ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
                    }
                }
            }
        }

        public override void Dispose()
        {
        }
    }
}
