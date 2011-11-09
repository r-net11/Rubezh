using System;
using System.Collections.Generic;
using System.Linq;
using Common;
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
                var journalRecords = new List<JournalRecord>();
                if (internalJournal != null && internalJournal.Journal.IsNotNullOrEmpty())
                {
                    foreach (var innerJournalRecord in internalJournal.Journal)
                    {
                        journalRecords.Add(JournalConverter.Convert(innerJournalRecord));
                    }
                }

                return journalRecords;
            }
        }

        public IEnumerable<JournalRecord> GetFilteredJournal(JournalFilter journalFilter)
        {
            return DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                Where(journal => journalFilter.CheckDaysConstraint(journal.SystemTime)).
                Where(journal => JournalFilterHelper.FilterRecord(journalFilter, journal)).
                Take(journalFilter.LastRecordsCount);
        }

        public IEnumerable<JournalRecord> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            var filterHelper = new ArchiveFilterHelper(archiveFilter);

            return DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                SkipWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.EndDate : journal.DeviceTime > archiveFilter.EndDate).
                TakeWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.StartDate : journal.DeviceTime > archiveFilter.StartDate).
                Where(journal => filterHelper.FilterByEvents(journal)).
                Where(journal => filterHelper.FilterBySubsystems(journal)).
                Where(journal => filterHelper.FilterByDevices(journal));
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
