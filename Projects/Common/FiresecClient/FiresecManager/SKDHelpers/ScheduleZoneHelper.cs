using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class ScheduleZoneHelper
	{
		public static bool Save(ScheduleZone scheduleZone, string name)
		{
			var operationResult = FiresecManager.FiresecService.SaveScheduleZone(scheduleZone, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(ScheduleZone scheduleZone, string name)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedScheduleZone(scheduleZone, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static ScheduleZone GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new ScheduleZoneFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetScheduleZones(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<ScheduleZone> Get(ScheduleZoneFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetScheduleZones(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}