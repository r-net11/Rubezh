using System;
using System.Linq;
using Common;
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
            bool result = true;
            if (JournalFilter.Categories.IsNotNullOrEmpty())
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(
                        x => x.DatabaseId == journalRecord.DeviceDatabaseId ||
                             x.DatabaseId == journalRecord.PanelDatabaseId);

                if (result = device != null)
                {
                    result = JournalFilter.Categories.Any(x => x == device.Driver.Category);
                }
            }

            if (result && JournalFilter.Events.IsNotNullOrEmpty())
            {
                result = JournalFilter.Events.Any(x => x == journalRecord.StateType);
            }

            return result;
        }
    }
}