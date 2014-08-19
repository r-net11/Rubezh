using System.Collections.Generic;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class SheduleDayIntervalHelper
	{
		public static bool Save(ScheduleDayInterval scheduleDayInterval)
		{
			var operationResult = FiresecManager.FiresecService.SaveSheduleDayInterval(scheduleDayInterval);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(ScheduleDayInterval scheduleDayInterval)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedSheduleDayInterval(scheduleDayInterval);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ScheduleDayInterval> Get(ScheduleDayIntervalFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetSheduleDayIntervals(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}