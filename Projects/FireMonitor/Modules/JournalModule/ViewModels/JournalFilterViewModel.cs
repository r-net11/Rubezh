using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace JournalModule.ViewModels
{
    public class JournalFilterViewModel
    {
        readonly JournalFilter _journalFilter;

        public JournalFilterViewModel(JournalFilter journalFilter)
        {
            _journalFilter = journalFilter;
        }

        public int RecordsCount
        {
            get { return _journalFilter.LastRecordsCount; }
        }

        public bool CheckDaysConstraint(JournalRecord journalRecord)
        {
            if (_journalFilter.IsLastDaysCountActive)
            {
                return (DateTime.Now.Date - journalRecord.DeviceTime.Date).Days < _journalFilter.LastDaysCount;
            }

            return true;
        }

        public bool FilterRecord(JournalRecord journalRecord)
        {
            bool retval = true;
            if (_journalFilter.Categories != null && _journalFilter.Categories.Count > 0)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(
                        x => x.DatabaseId == journalRecord.DeviceDatabaseId ||
                             x.DatabaseId == journalRecord.PanelDatabaseId);

                if (retval = device != null)
                {
                    retval = _journalFilter.Categories.Any(x => x.DeviceCategoryType == device.Driver.Category);
                }
            }

            if (retval && _journalFilter.Events != null && _journalFilter.Events.Count > 0)
            {
                retval = _journalFilter.Events.Any(x => x.Id == journalRecord.State.Id);
            }

            return retval;
        }
    }
}