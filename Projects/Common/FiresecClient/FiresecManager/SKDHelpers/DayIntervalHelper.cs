using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecClient.SKDHelpers
{
	public static class DayIntervalHelper
	{
		public static bool Save(DayInterval dayInterval)
		{
			return Save(new List<DayInterval> { dayInterval });
		}
		public static bool Save(IEnumerable<DayInterval> dayIntervals)
		{
			var operationResult = FiresecManager.FiresecService.SaveDayIntervals(dayIntervals);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(DayInterval dayInterval)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedDayIntervals(new List<DayInterval> { dayInterval });
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
