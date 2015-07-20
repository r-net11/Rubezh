using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure.Common.Windows;

namespace FiresecClient.SKDHelpers
{
	public static class GKScheduleHelper
	{
		public static List<GKSchedule> GetSchedules()
		{
			var operationResult = FiresecManager.FiresecService.GetGKSchedules();
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool SaveSchedule(GKSchedule item, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SaveGKSchedule(item, isNew);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}

		public static bool DeleteSchedule(GKSchedule item)
		{
			var operationResult = FiresecManager.FiresecService.DeleteGKSchedule(item);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}

		public static List<GKDaySchedule> GetDaySchedules()
		{
			var operationResult = FiresecManager.FiresecService.GetGKDaySchedules();
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool SaveDaySchedule(GKDaySchedule item, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SaveGKDaySchedule(item, isNew);
			if (operationResult == null)
				return false;
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}

		public static bool DeleteDaySchedule(GKDaySchedule item)
		{
			var operationResult = FiresecManager.FiresecService.DeleteGKDaySchedule(item);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}
	}
}