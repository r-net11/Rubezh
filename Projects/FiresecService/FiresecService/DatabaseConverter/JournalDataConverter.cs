using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Converters;
using FiresecService.DatabaseConverter;

namespace FiresecService
{
    public static class JournalDataConverter
    {
        public static void Convert()
        {
            FiresecDbConverterDataContext dataContext = new FiresecDbConverterDataContext();
            dataContext.Journal.DeleteAllOnSubmit(from j in dataContext.Journal select j);

            List<JournalRecord> journalRecords = ReadAllJournal();
            journalRecords.Reverse();

            foreach (var journalItem in journalRecords)
            {
                Journal journal = new Journal();

                journal.DeviceTime = journalItem.DeviceTime;
                journal.SystemTime = journalItem.SystemTime;
                journal.ZoneName = journalItem.ZoneName;
                journal.Description = journalItem.Description;
                journal.DeviceName = journalItem.DeviceName;
                journal.PanelName = journalItem.PanelName;
                journal.DeviceDatabaseId = journalItem.DeviceDatabaseId;
                journal.PanelDatabaseId = journalItem.PanelDatabaseId;
                journal.UserName = journalItem.User;
                journal.StateType = (int)journalItem.StateType;

                dataContext.Journal.InsertOnSubmit(journal);
            }

            dataContext.SubmitChanges();
        }

        public static void Select()
        {
            FiresecDbConverterDataContext dataContext = new FiresecDbConverterDataContext();

            List<Journal> journals = (from journal in dataContext.Journal
                           where journal.StateType == 6
                           select journal).ToList();

            return;
        }

        public static List<JournalRecord> ReadAllJournal()
        {
            var internalJournal = FiresecInternalClient.ReadEvents(0, 100000);

            var journalRecords = new List<JournalRecord>();
            if (internalJournal != null && internalJournal.Journal != null)
            {
                foreach (var innerJournaRecord in internalJournal.Journal)
                {
                    journalRecords.Add(JournalConverter.Convert(innerJournaRecord));
                }
            }

            return journalRecords;
        }
    }
}
