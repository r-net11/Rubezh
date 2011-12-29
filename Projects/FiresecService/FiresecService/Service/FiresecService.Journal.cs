using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;
using FiresecService.Processor;

namespace FiresecService
{
    public partial class FiresecService
    {
        public List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            lock (Locker)
            {
                var internalJournal = FiresecInternalClient.ReadEvents(startIndex, count);
                if (internalJournal != null && internalJournal.Journal.IsNotNullOrEmpty())
                {
                    return new List<JournalRecord>(
                        internalJournal.Journal.Select(x => JournalConverter.Convert(x))
                    );
                }
                return new List<JournalRecord>();
            }
        }

        public IEnumerable<JournalRecord> GetFilteredJournal(JournalFilter journalFilter)
        {
            var filteredJournal = 
                DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                Where(journal => journalFilter.CheckDaysConstraint(journal.SystemTime)).
                Where(journal => JournalFilterHelper.FilterRecord(journalFilter, journal)).
                Take(journalFilter.LastRecordsCount);
            return filteredJournal;
        }

        public IEnumerable<JournalRecord> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            return DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                    RangeJournalByTime(archiveFilter).
                    FilterJournalByEvents(archiveFilter).
                    FilterJournalBySubsystems(archiveFilter).
                    FilterJournalByDevices(archiveFilter);
        }

        public IEnumerable<JournalRecord> GetDistinctRecords()
        {
            return DataBaseContext.JournalRecords.AsEnumerable().
                Select(x => x).Distinct(new JournalRecord());
        }

        public DateTime GetArchiveStartDate()
        {
            return DataBaseContext.JournalRecords.AsEnumerable().First().SystemTime;
        }

        public void AddJournalRecord(JournalRecord journalRecord)
        {
            journalRecord.User = _userName;
            DatabaseHelper.AddJournalRecord(journalRecord);
            CallbackManager.OnNewJournalRecord(journalRecord);
        }
    }
}