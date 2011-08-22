using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;
using FiresecService.DatabaseConverter;

namespace FiresecService
{
    public static class JournalDataConverter
    {
        public static void Convert()
        {
            using (var dataContext = new FiresecDbConverterDataContext())
            {
                dataContext.Journal.DeleteAllOnSubmit(from j in dataContext.Journal select j);

                List<JournalRecord> journalRecords = ReadAllJournal();
                journalRecords.Reverse();

                foreach (var journalItem in journalRecords)
                {
                    var journal = new Journal()
                    {
                        DeviceTime = journalItem.DeviceTime,
                        SystemTime = journalItem.SystemTime,
                        ZoneName = journalItem.ZoneName,
                        Description = journalItem.Description,
                        DeviceName = journalItem.DeviceName,
                        PanelName = journalItem.PanelName,
                        DeviceDatabaseId = journalItem.DeviceDatabaseId,
                        PanelDatabaseId = journalItem.PanelDatabaseId,
                        UserName = journalItem.User,
                        StateType = (int) journalItem.StateType
                    };

                    dataContext.Journal.InsertOnSubmit(journal);
                }

                dataContext.SubmitChanges();
            }
        }

        public static void Select()
        {
            using (var dataContext = new FiresecDbConverterDataContext())
            {
                List<Journal> journals = (from journal in dataContext.Journal
                                          where journal.StateType == 6
                                          select journal).ToList();
            }
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