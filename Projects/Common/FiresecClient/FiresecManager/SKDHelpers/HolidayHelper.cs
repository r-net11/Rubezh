using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecClient.SKDHelpers
{
	public static class HolidayHelper
	{
		public static bool Save(Holiday holiday)
		{
			var operationResult = FiresecManager.FiresecService.SaveHolidays(new List<Holiday> { holiday });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Holiday holiday)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedHolidays(new List<Holiday> { holiday });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Holiday GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new HolidayFilter();
			filter.Uids.Add(uid.Value);
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
