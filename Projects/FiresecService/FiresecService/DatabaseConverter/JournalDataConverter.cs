using System.Collections.Generic;
using System.Linq;
using Common;
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
                dataContext.JournalRecords.DeleteAllOnSubmit(from j in dataContext.JournalRecords select j);
                dataContext.SubmitChanges();
                dataContext.JournalRecords.InsertAllOnSubmit(ReadAllJournal());
                dataContext.SubmitChanges();
            }
        }

        public static IEnumerable<JournalRecord> ReadAllJournal()
        {
            var internalJournal = FiresecInternalClient.ReadEvents(0, 100000);
            if (internalJournal != null && internalJournal.Journal.IsNotNullOrEmpty())
            {
                return internalJournal.Journal.
                    Select(innerJournaRecord => JournalConverter.Convert(innerJournaRecord)).
                    OrderBy(JournalRecord => JournalRecord.DeviceTime);
            }
            return new List<JournalRecord>();
        }
    }
}