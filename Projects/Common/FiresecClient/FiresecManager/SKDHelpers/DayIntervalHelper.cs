using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecClient.SKDHelpers
{
	public static class DayIntervalHelper
	{
		public static bool Save(DayInterval dayInterval)
		{
			var operationResult = FiresecManager.FiresecService.SaveDayInterval(dayInterval);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(DayInterval dayInterval)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedDayInterval(dayInterval);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static DayInterval GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new DayIntervalFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetDayIntervals(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<DayInterval> Get(DayIntervalFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetDayIntervals(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}