using FiresecAPI.Models;
using FiresecService.Converters;
using FiresecService.DatabaseConverter;

namespace FiresecService
{
    public static class DatabaseHelper
    {
        public static void AddJournalRecord(JournalRecord journalRecord)
        {
            using (var dataContext = new FiresecDbConverterDataContext())
            {
                var journal = JournalConverter.JournalRecordToDataBaseJournal(journalRecord);
                dataContext.Journal.InsertOnSubmit(journal);
                dataContext.SubmitChanges();
            }
        }
    }
}
