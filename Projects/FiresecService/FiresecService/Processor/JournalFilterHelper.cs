using System.Linq;
using FiresecAPI.Models;

namespace FiresecService.Processor
{
    public class JournalFilterHelper
    {
		FiresecManager FiresecManager;

		public JournalFilterHelper(FiresecManager firesecManager)
		{
			FiresecManager = firesecManager;
		}

        public bool FilterRecord(JournalFilter journalFilter, JournalRecord journalRecord)
        {
            bool result = true;
            if (journalFilter.Categories.IsNotNullOrEmpty())
            {
                Device device = null;
                if (string.IsNullOrWhiteSpace(journalRecord.DeviceDatabaseId) == false)
                {
                    device = FiresecManager.ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(
                         x => x.DatabaseId == journalRecord.DeviceDatabaseId);
                }
                else
                {
                    device = FiresecManager.ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(
                           x => x.DatabaseId == journalRecord.PanelDatabaseId);
                }

                if ((result = (device != null)))
                {
                    result = journalFilter.Categories.Any(daviceCategory => daviceCategory == device.Driver.Category);
                }
            }

            if (result && journalFilter.Events.IsNotNullOrEmpty())
            {
                result = journalFilter.Events.Any(_event => _event == journalRecord.StateType);
            }

            return result;
        }
    }
}