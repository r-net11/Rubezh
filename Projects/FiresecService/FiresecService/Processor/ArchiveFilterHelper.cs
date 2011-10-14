using System.Linq;
using Common;
using FiresecAPI.Models;

namespace FiresecService.Processor
{
    public class ArchiveFilterHelper
    {
        ArchiveFilter _archiveFilter;

        public ArchiveFilterHelper(ArchiveFilter archiveFilter)
        {
            _archiveFilter = archiveFilter;
        }

        public bool FilterByEvents(JournalRecord record)
        {
            if (_archiveFilter.Descriptions.IsNotNullOrEmpty())
                return _archiveFilter.Descriptions.Any(description => description == record.Description);
            else
                return true;
        }

        public bool FilterBySubsystems(JournalRecord record)
        {
            if (_archiveFilter.Subsystems.IsNotNullOrEmpty())
                return _archiveFilter.Subsystems.Any(subsystem => subsystem == record.SubsystemType);
            else
                return true;
        }

        public bool FilterByDevices(JournalRecord record)
        {
            if (_archiveFilter.IsDeviceFilterOn && _archiveFilter.DeviceDatabaseIds.IsNotNullOrEmpty())
                return _archiveFilter.DeviceDatabaseIds.Any(id => id == record.PanelDatabaseId);
            else
                return true;
        }
    }
}