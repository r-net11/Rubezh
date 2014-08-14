using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecClient.SKDHelpers
{
	public static class TimeIntervalHelper
	{
		public static bool Save(DayIntervalPart dayIntervalPart)
		{
			var operationResult = FiresecManager.FiresecService.SaveDayIntervalPart(dayIntervalPart);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(DayIntervalPart dayIntervalPart)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedDayIntervalPart(dayIntervalPart);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<DayIntervalPart> Get(DayIntervalPartFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetDayIntervalParts(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}