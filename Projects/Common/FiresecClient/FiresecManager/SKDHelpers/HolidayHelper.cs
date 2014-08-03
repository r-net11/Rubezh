using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecClient.SKDHelpers
{
	public static class HolidayHelper
	{
		public static bool Save(Holiday holiday)
		{
			var operationResult = FiresecManager.FiresecService.SaveHoliday(holiday);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Holiday holiday)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedHoliday(holiday);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Holiday GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new HolidayFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetHolidays(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<Holiday> Get(HolidayFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetHolidays(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}