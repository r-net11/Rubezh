using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace JournalModule.ViewModels
{
    public class JournalFilterViewModel
    {
        public JournalFilter JournalFilter { get; private set; }

        public JournalFilterViewModel(JournalFilter journalFilter)
        {
            JournalFilter = journalFilter;
        }

        public int RecordsCount
        {
            get { return JournalFilter.LastRecordsCount; }
        }

        public bool CheckDaysConstraint(JournalRecord journalRecord)
        {
            if (JournalFilter.IsLastDaysCountActive)
            {
                return (DateTime.Now.Date - journalRecord.DeviceTime.Date).Days < JournalFilter.LastDaysCount;
            }

            return true;
        }

        public bool FilterRecord(JournalRecord journalRecord)
        {
            bool retval = true;
            if (JournalFilter.Categories != null && JournalFilter.Categories.Count > 0)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(
                        x => x.DatabaseId == journalRecord.DeviceDatabaseId ||
                             x.DatabaseId == journalRecord.PanelDatabaseId);

                if (retval = device != null)
                {
                    retval = JournalFilter.Categories.Any(x => x == device.Driver.Category);
                }
            }

            if (retval && JournalFilter.Events != null && JournalFilter.Events.Count > 0)
            {
                retval = JournalFilter.Events.Any(x => x == journalRecord.StateType);
            }

            return retval;
        }
    }
}