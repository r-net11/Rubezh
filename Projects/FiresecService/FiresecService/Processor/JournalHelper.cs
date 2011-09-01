using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecService.DatabaseConverter;

namespace FiresecService.Processor
{
    public class JournalHelper : IEqualityComparer<Journal>
    {
        public static bool FilterRecord(JournalFilter filter, Journal journal)
        {
            bool result = true;
            if (filter.Categories.IsNotNullOrEmpty())
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(
                        x => x.DatabaseId == journal.DeviceDatabaseId ||
                             x.DatabaseId == journal.PanelDatabaseId);

                if (result = device != null)
                {
                    result = filter.Categories.Any(daviceCategory => daviceCategory == device.Driver.Category);
                }
            }

            if (result && filter.Events.IsNotNullOrEmpty())
            {
                result = filter.Events.Any(_event => _event == (StateType) journal.StateType);
            }

            return result;
        }

        public bool Equals(Journal x, Journal y)
        {
            if (object.ReferenceEquals(x, y)) return true;

            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return x.Description == y.Description;
        }

        public int GetHashCode(Journal obj)
        {
            return obj.Description.GetHashCode();
        }
    }
}