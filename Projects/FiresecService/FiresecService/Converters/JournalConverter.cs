using System;
using Firesec.ReadEvents;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class JournalConverter
    {
        public static JournalRecord Convert(journalType innerJournalItem)
        {
            JournalRecord journalRecord = new JournalRecord();
            journalRecord.No = int.Parse(innerJournalItem.IDEvents);
            journalRecord.DeviceTime = ConvertTime(innerJournalItem.Dt);
            journalRecord.SystemTime = ConvertTime(innerJournalItem.SysDt);
            journalRecord.ZoneName = innerJournalItem.ZoneName;
            journalRecord.Description = innerJournalItem.EventDesc;
            journalRecord.DeviceName = innerJournalItem.CLC_Device;
            journalRecord.PanelName = innerJournalItem.CLC_DeviceSource;
            journalRecord.DeviceDatabaseId = innerJournalItem.IDDevices;
            journalRecord.PanelDatabaseId = innerJournalItem.IDDevicesSource;
            journalRecord.User = innerJournalItem.UserInfo;
            journalRecord.StateType = (StateType)int.Parse(innerJournalItem.IDTypeEvents);
            journalRecord.User = innerJournalItem.UserInfo;

            return journalRecord;
        }

        static DateTime ConvertTime(string firesecTime)
        {
            int year = int.Parse(firesecTime.Substring(0, 4));
            int month = int.Parse(firesecTime.Substring(4, 2));
            int day = int.Parse(firesecTime.Substring(6, 2));
            int hour = int.Parse(firesecTime.Substring(9, 2));
            int minute = int.Parse(firesecTime.Substring(12, 2));
            int secunde = int.Parse(firesecTime.Substring(15, 2));

            return new DateTime(year, month, day, hour, minute, secunde);
        }
    }
}