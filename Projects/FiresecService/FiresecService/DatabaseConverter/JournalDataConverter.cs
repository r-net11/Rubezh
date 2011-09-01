using System.Collections.Generic;
using System.Linq;
using Common;
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
                dataContext.Journal.InsertAllOnSubmit(ReadAllJournal());
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

        public static IEnumerable<Journal> ReadAllJournal()
        {
            var internalJournal = FiresecInternalClient.ReadEvents(0, 100000);
            if (internalJournal != null && internalJournal.Journal.IsNotNullOrEmpty())
            {
                return internalJournal.Journal.Select(
                    innerJournaRecord => JournalConverter.ConvertToDataBaseJournal(innerJournaRecord));
            }

            return new List<Journal>();
        }
    }
}