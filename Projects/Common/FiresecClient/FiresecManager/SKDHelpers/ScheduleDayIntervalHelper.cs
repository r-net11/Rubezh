using System.Collections.Generic;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class SheduleDayIntervalHelper
	{
		public static bool Save(ScheduleDayInterval scheduleDayInterval, string name)
		{
			var operationResult = FiresecManager.FiresecService.SaveSheduleDayInterval(scheduleDayInterval, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Remove(ScheduleDayInterval scheduleDayInterval, string name)
		{
			var operationResult = FiresecManager.FiresecService.RemoveSheduleDayInterval(scheduleDayInterval, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ScheduleDayInterval> Get(ScheduleDayIntervalFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetSheduleDayIntervals(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}