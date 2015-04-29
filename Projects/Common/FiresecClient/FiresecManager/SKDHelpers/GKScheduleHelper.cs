using System.Collections.Generic;
using FiresecAPI.GK;

namespace FiresecClient.SKDHelpers
{
	public static class GKScheduleHelper
	{
		public static bool SaveSchedule(GKSchedule item, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SaveGKSchedule(item, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool DeleteSchedule(GKSchedule item)
		{
			var operationResult = FiresecManager.FiresecService.DeleteGKSchedule(item);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static List<GKSchedule> GetSchedules()
		{
			var operationResult = FiresecManager.FiresecService.GetGKSchedules();
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool SaveDaySchedule(GKDaySchedule item, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SaveGKDaySchedule(item, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool DeleteDaySchedule(GKDaySchedule item)
		{
			var operationResult = FiresecManager.FiresecService.DeleteGKDaySchedule(item);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static List<GKDaySchedule> GetDaySchedules()
		{
			var operationResult = FiresecManager.FiresecService.GetGKDaySchedules();
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
