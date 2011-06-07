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
using Firesec;


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

            CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);

            FiresecManager.States.DeviceStateChanged += new Action<string>(CurrentStates_DeviceStateChanged);
            DeviceState.AlarmAdded += new Action<AlarmType, string>(DeviceState_AlarmAdded);
            DeviceState.AlarmRemoved += new Action<AlarmType, string>(DeviceState_AlarmRemoved);
        }

        void DeviceState_AlarmAdded(AlarmType alarmType, string id)
        {
            var deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == id);
            var device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == id);
            Alarm alarm = new Alarm();
            alarm.AlarmType = alarmType;
            alarm.DeviceId = id;
            alarm.Name = AlarmToString(alarmType);
            alarm.Description = "Устройство " + device.Driver.name + " - " + device.Address;
            alarm.Time = DateTime.Now.ToString();
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }

        public string AlarmToString(AlarmType alarmType)
        {
            switch (alarmType)
            {
                case AlarmType.Attention:
                    return "Внимание";

                case AlarmType.Auto:
                    return "Автоматика отключена";

                case AlarmType.Failure:
                    return "Неисправность";

                case AlarmType.Fire:
                    return "Пожар";

                case AlarmType.Info:
                    return "Информация";

                case AlarmType.Off:
                    return "Отключенное оборудование";

                case AlarmType.Service:
                    return "Требуется обслуживание";

                default:
                    return "";
            }
        }

        void DeviceState_AlarmRemoved(AlarmType alarmType, string id)
        {
            Alarm alarm = new Alarm();
            alarm.AlarmType = alarmType;
            alarm.DeviceId = id;
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Publish(alarm);
        }

        void CurrentStates_DeviceStateChanged(string obj)
        {
        }

        void CurrentStates_NewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            //Alarm.CreateFromJournalEvent(journalItem);
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
            alarmListViewModel.Initialize(alarms, null);
            ServiceFactory.Layout.Show(alarmListViewModel);
        }

        public override void Dispose()
        {
        }
    }
}
