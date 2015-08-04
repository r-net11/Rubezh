using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using SKDModule.Model;

namespace SKDModule.Helpers
{
	public static class TimeTrackingHelper
	{
		public static List<TimeTrackZone> GetMergedZones(ShortEmployee employee)
		{
			var schedule = ScheduleHelper.GetSingle(employee.ScheduleUID);
			if (schedule == null) return SKDManager.Zones.Select(x => new TimeTrackZone(x)).ToList();

			return SKDManager.Zones.Select(zone => schedule.Zones.Any(x => x.ZoneUID == zone.UID)
				? new TimeTrackZone(zone) { IsURV = true }
				: new TimeTrackZone(zone)).ToList();
		}
	}
}
