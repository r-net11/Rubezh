using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Service;

namespace FiresecService.Database
{
    public static class DatabaseHelper
    {
        public static void AddJournalRecords(List<JournalRecord> journalRecords)
        {
            try
            {
                using (var dataContext = ConnectionManager.CreateFiresecDataContext())
                {
                    dataContext.JournalRecords.InsertAllOnSubmit(journalRecords);
                    dataContext.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове DatabaseHelper.AddJournalRecords");
            }
        }

        static bool AddJournalRecord(JournalRecord journalRecord)
        {
            try
            {
                using (var dataContext = ConnectionManager.CreateFiresecDataContext())
                {
                    var query =
                    "SELECT * FROM Journal WHERE " +
                    "\n SystemTime = '" + journalRecord.SystemTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    "\n AND OldId = " + journalRecord.OldId.ToString();

                    var result = dataContext.ExecuteQuery<JournalRecord>(query);

                    if (result.Count() == 0)
                    {
                        dataContext.JournalRecords.InsertOnSubmit(journalRecord);
                        dataContext.SubmitChanges();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове DatabaseHelper.AddJournalRecord");
            }
            return false;
        }

        public static int GetLastOldId()
        {
            try
            {
                using (var dataContext = ConnectionManager.CreateFiresecDataContext())
                {
                    var query = "SELECT MAX(OldId) FROM Journal";
                    var result = dataContext.ExecuteQuery<int?>(query);
                    var firstResult = result.FirstOrDefault();
                    if (firstResult != null)
                        return firstResult.Value;
                    return -1;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове DatabaseHelper.GetLastOldId");
            }
            return -1;
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

            AddJournalRecord(journalRecord);
            ClientsCash.OnNewJournalRecord(new List<JournalRecord>() { journalRecord });
        }
    }
}