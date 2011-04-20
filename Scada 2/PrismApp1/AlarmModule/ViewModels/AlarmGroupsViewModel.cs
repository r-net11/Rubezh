using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure;
using AlarmModule.Events;
using System.Diagnostics;
using ClientApi;

namespace AlarmModule.ViewModels
{
    public class AlarmGroupsViewModel : RegionViewModel
    {
        public AlarmGroupsViewModel()
        {
            MainAlarms = new ObservableCollection<AlarmGroupViewModel>();
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Пожар", AlarmType = AlarmType.Alarm });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Внимание", AlarmType = AlarmType.Attention });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Неисправность", AlarmType = AlarmType.Failure });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Отключение", AlarmType = AlarmType.Off });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Информация", AlarmType = AlarmType.Info });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Обслуживание", AlarmType = AlarmType.Service });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Автоматика", AlarmType = AlarmType.Auto });

            ServiceFactory.Events.GetEvent<ShowAllAlarmsEvent>().Subscribe(OnShowAllAlarms);

            ClientManager.Start();
            ServiceClient.CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
        }

        void CurrentStates_NewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            AlarmType alarmType;
            switch (journalItem.IDTypeEvents)
            {
                case "0":
                    alarmType = AlarmType.Alarm;
                    break;

                case "1":
                    alarmType = AlarmType.Attention;
                    break;

                case "2":
                    alarmType = AlarmType.Failure;
                    break;

                case "3":
                    alarmType = AlarmType.Service;
                    break;

                case "4":
                    alarmType = AlarmType.Info; // обход
                    break;

                case "5":
                    alarmType = AlarmType.Info;
                    break;

                case "6":
                    alarmType = AlarmType.Info;
                    break;

                case "7":
                    alarmType = AlarmType.Info;
                    break;

                default:
                    alarmType = AlarmType.Info;
                    break;
            }

            Alarm alarm = new Alarm();
            alarm.AlarmType = alarmType;
            alarm.Name = journalItem.EventDesc;
            alarm.Time = journalItem.SysDt;
            alarm.ZoneNo = journalItem.ZoneName;

            string databaseId = null;

            if (string.IsNullOrEmpty(journalItem.IDDevicesSource))
            {
                databaseId = journalItem.IDDevicesSource;
            }
            if (string.IsNullOrEmpty(journalItem.IDDevices))
            {
                databaseId = journalItem.IDDevices;
            }

            string path = null;
            if (ServiceClient.CurrentConfiguration.AllDevices.Any(x => x.DatabaseId == databaseId))
            {
                Device device = ServiceClient.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.DatabaseId == databaseId);
                path = device.Path;
            }

            alarm.Path = path;

            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }

        public ObservableCollection<AlarmGroupViewModel> MainAlarms { get; set; }

        void OnShowAllAlarms(object obj)
        {
            Trace.WriteLine("Show all alarms");

            List<Alarm> alarms = new List<Alarm>();
            foreach (AlarmGroupViewModel alarmGroupViewModel in MainAlarms)
            {
                alarms.AddRange(alarmGroupViewModel.Alarms);
            }

            AlarmGroupDetailsViewModel alarmGroupDetailsViewModel = new AlarmGroupDetailsViewModel();
            alarmGroupDetailsViewModel.Initialize(alarms);
            ServiceFactory.Layout.Show(alarmGroupDetailsViewModel);
        }

        public override void Dispose()
        {
        }
    }
}
