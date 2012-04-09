using System;
using FiresecAPI.Models;
using FiresecService.DatabaseConverter;

namespace FiresecService
{
    public static class DatabaseHelper
    {
        public static void AddJournalRecord(JournalRecord journalRecord)
        {
            try
            {
				using (var dataContext = ConnectionManager.CreateFiresecDataContext())
                {
                    dataContext.JournalRecords.InsertOnSubmit(journalRecord);
                    dataContext.SubmitChanges();
                }
            }
            catch { };
        }

        public static void AddInfoMessage(string userName, string mesage)
        {
            var journalRecord = new JournalRecord()
            {
                DeviceTime = DateTime.Now,
                SystemTime = DateTime.Now,
                StateType = StateType.Info,
                Description = mesage,
                User = userName,
                DeviceDatabaseId = "",
                DeviceName = "",
                PanelDatabaseId = "",
                PanelName = "",
                ZoneName = ""
            };

            lock (FiresecService.Locker)
            {
                CallbackManager.OnNewJournalRecord(journalRecord);
            }
            AddJournalRecord(journalRecord);
        }
    }
}