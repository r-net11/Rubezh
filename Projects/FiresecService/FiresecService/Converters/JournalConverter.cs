using System;
using Firesec.ReadEvents;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    //Refactored by Badaev Andrei. See how it was in file history
    public static class JournalConverter
    {
        public static JournalRecord Convert(journalType innerJournalRecord)
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