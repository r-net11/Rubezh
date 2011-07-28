using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using Firesec.ReadEvents;

namespace FiresecService.Converters
{
    public static class JournalConverter
    {
        public static JournalItem Convert(journalType innerJournalItem)
        {
            JournalItem journalItem = new JournalItem();
            journalItem.No = System.Convert.ToInt32(innerJournalItem.IDEvents);
            journalItem.DeviceTime = ConvertTime(innerJournalItem.Dt);
            journalItem.SystemTime = ConvertTime(innerJournalItem.SysDt);
            journalItem.ZoneName = innerJournalItem.ZoneName;
            journalItem.Description = innerJournalItem.EventDesc;
            journalItem.DeviceName = innerJournalItem.CLC_Device;
            journalItem.PanelName = innerJournalItem.CLC_DeviceSource;
            journalItem.DeviceDatabaseId = innerJournalItem.IDDevices;
            journalItem.PanelDatabaseId = innerJournalItem.IDDevicesSource;
            journalItem.User = innerJournalItem.UserInfo;
            int id = System.Convert.ToInt32(innerJournalItem.IDTypeEvents);
            journalItem.State = new State() { Id = id };
            journalItem.User = innerJournalItem.UserInfo;
            return journalItem;
        }

        static DateTime ConvertTime(string firesecTime)
        {
            int year = System.Convert.ToInt32(firesecTime.Substring(0, 4));
            int month = System.Convert.ToInt32(firesecTime.Substring(4, 2));
            int day = System.Convert.ToInt32(firesecTime.Substring(6, 2));
            int hour = System.Convert.ToInt32(firesecTime.Substring(9, 2));
            int minute = System.Convert.ToInt32(firesecTime.Substring(12, 2));
            int secunde = System.Convert.ToInt32(firesecTime.Substring(15, 2));
            DateTime dt = new DateTime(year, month, day, hour, minute, secunde);
            return dt;
        }
    }
}
