using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientApi;
using Infrastructure;
using AlarmModule.Events;

namespace AlarmModule
{
    public class Alarm
    {
        public AlarmType AlarmType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }

        public string Path { get; set; }
        public string ZoneNo { get; set; }

        public static void CreateFromJournalEvent(Firesec.ReadEvents.journalType journalItem)
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
    }
}
