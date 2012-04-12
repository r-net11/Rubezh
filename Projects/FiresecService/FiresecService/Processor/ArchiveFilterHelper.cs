using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecService.Processor
{
    public static class ArchiveFilterHelper
    {
        public static IEnumerable<JournalRecord> RangeJournalByTime(this IEnumerable<JournalRecord> journal, ArchiveFilter archiveFilter)
        {
            return journal.SkipWhile(record => CheckHighDateBound(record, archiveFilter) == false).
                           TakeWhile(record => CheckLowDateBound(record, archiveFilter));
        }

        public static IEnumerable<JournalRecord> FilterJournalByEvents(this IEnumerable<JournalRecord> journal, ArchiveFilter archiveFilter)
        {
            if (archiveFilter.Descriptions.IsNotNullOrEmpty())
                return journal.Where(record => archiveFilter.Descriptions.Any(description => description == record.Description));
            return journal;
        }

        public static IEnumerable<JournalRecord> FilterJournalBySubsystems(this IEnumerable<JournalRecord> journal, ArchiveFilter archiveFilter)
        {
            if (archiveFilter.Subsystems.IsNotNullOrEmpty())
                return journal.Where(x => archiveFilter.Subsystems.Any(s => s == x.SubsystemType));
            return journal;
        }

        public static IEnumerable<JournalRecord> FilterJournalByDevices(this IEnumerable<JournalRecord> journal, ArchiveFilter archiveFilter)
        {
            if (archiveFilter.IsDeviceFilterOn && archiveFilter.DeviceDatabaseIds.IsNotNullOrEmpty())
                return journal.Where(record => archiveFilter.DeviceDatabaseIds.Any(id => id == record.PanelDatabaseId));
            return journal;
        }

        static bool CheckHighDateBound(JournalRecord record, ArchiveFilter archiveFilter)
        {
            return (archiveFilter.UseSystemDate ? record.SystemTime : record.DeviceTime) <= archiveFilter.EndDate;
        }

        static bool CheckLowDateBound(JournalRecord record, ArchiveFilter archiveFilter)
        {
            return (archiveFilter.UseSystemDate ? record.SystemTime : record.DeviceTime) >= archiveFilter.StartDate;
        }
    }
}