using System;
using Firesec.ReadEvents;
using FiresecAPI.Models;
using FiresecService.DatabaseConverter;

namespace FiresecService.Converters
{
    public static class JournalConverter
    {
        public static JournalRecord ConvertToJournalRecord(journalType innerJournalRecord)
        {
            return new JournalRecord()
            {
                No = int.Parse(innerJournalRecord.IDEvents),
                DeviceTime = ConvertTime(innerJournalRecord.Dt),
                SystemTime = ConvertTime(innerJournalRecord.SysDt),
                ZoneName = innerJournalRecord.ZoneName,
                Description = innerJournalRecord.EventDesc,
                DeviceName = innerJournalRecord.CLC_Device,
                PanelName = innerJournalRecord.CLC_DeviceSource,
                DeviceDatabaseId = innerJournalRecord.IDDevices,
                PanelDatabaseId = innerJournalRecord.IDDevicesSource,
                User = innerJournalRecord.UserInfo,
                StateType = (StateType) int.Parse(innerJournalRecord.IDTypeEvents),
            };
        }

        public static Journal ConvertToDataBaseJournal(journalType innerJournalRecord)
        {
            return new Journal()
            {
                Id = int.Parse(innerJournalRecord.IDEvents),
                DeviceTime = ConvertTime(innerJournalRecord.Dt),
                SystemTime = ConvertTime(innerJournalRecord.SysDt),
                ZoneName = innerJournalRecord.ZoneName,
                Description = innerJournalRecord.EventDesc,
                DeviceName = innerJournalRecord.CLC_Device,
                PanelName = innerJournalRecord.CLC_DeviceSource,
                DeviceDatabaseId = innerJournalRecord.IDDevices,
                PanelDatabaseId = innerJournalRecord.IDDevicesSource,
                UserName = innerJournalRecord.UserInfo,
                StateType = int.Parse(innerJournalRecord.IDTypeEvents),
            };
        }

        public static JournalRecord DataBaseJournalToJournalRecord(Journal dataBaseJournal)
        {
            return new JournalRecord()
            {
                No = dataBaseJournal.Id,
                DeviceTime = (DateTime) dataBaseJournal.DeviceTime,
                SystemTime = (DateTime) dataBaseJournal.SystemTime,
                ZoneName = dataBaseJournal.ZoneName,
                Description = dataBaseJournal.Description,
                DeviceName = dataBaseJournal.DeviceName,
                PanelName = dataBaseJournal.PanelName,
                DeviceDatabaseId = dataBaseJournal.DeviceDatabaseId,
                PanelDatabaseId = dataBaseJournal.PanelDatabaseId,
                User = dataBaseJournal.UserName,
                StateType = (StateType) dataBaseJournal.StateType
            };
        }

        static DateTime ConvertTime(string firesecTime)
        {
            return new DateTime(
                int.Parse(firesecTime.Substring(0, 4)),
                int.Parse(firesecTime.Substring(4, 2)),
                int.Parse(firesecTime.Substring(6, 2)),
                int.Parse(firesecTime.Substring(9, 2)),
                int.Parse(firesecTime.Substring(12, 2)),
                int.Parse(firesecTime.Substring(15, 2))
            );
        }
    }
}