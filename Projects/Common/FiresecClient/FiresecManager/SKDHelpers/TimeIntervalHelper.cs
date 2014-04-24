using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecClient.SKDHelpers
{
	public static class TimeIntervalHelper
	{
		public static bool Save(TimeInterval timeInterval)
		{
			var operationResult = FiresecManager.FiresecService.SaveTimeIntervals(new List<TimeInterval> { timeInterval });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(TimeInterval timeInterval)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedTimeIntervals(new List<TimeInterval> { timeInterval });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static TimeInterval GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new TimeIntervalFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetTimeIntervals(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<TimeInterval> Get(TimeIntervalFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetTimeIntervals(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
