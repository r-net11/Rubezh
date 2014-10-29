using System.Collections.Generic;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class DayIntervalPartHelper
	{
		public static bool Save(DayIntervalPart dayIntervalPart, string name)
		{
			var operationResult = FiresecManager.FiresecService.SaveDayIntervalPart(dayIntervalPart, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Remove(DayIntervalPart dayIntervalPart, string name)
		{
			var operationResult = FiresecManager.FiresecService.RemoveDayIntervalPart(dayIntervalPart, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<DayIntervalPart> Get(DayIntervalPartFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetDayIntervalParts(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}