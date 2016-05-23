using Infrastructure.Common.Windows;
using RubezhAPI.GK;
using System.Collections.Generic;
using System.Linq;
using RubezhClient;

namespace GKWebService.DataProviders.SKD
{
    public static class GKScheduleHelper
	{
		public static List<GKSchedule> GetSchedules()
		{
			var operationResult = ClientManager.RubezhService.GetGKSchedules();
			if (operationResult.Result != null)
				operationResult.Result.ForEach(x => x.ScheduleParts = x.ScheduleParts.OrderBy(y => y.DayNo).ToList());
			return Common.ThrowErrorIfExists(operationResult);
		}

		public static bool SaveSchedule(GKSchedule item, bool isNew)
		{
			var operationResult = ClientManager.RubezhService.SaveGKSchedule(item, isNew);
		    Common.ThrowErrorIfExists(operationResult);
			return operationResult.Result;
		}

		public static bool DeleteSchedule(GKSchedule item)
		{
			var operationResult = ClientManager.RubezhService.DeleteGKSchedule(item);
            MessageBoxService.ShowWarning(operationResult.Error);
            return operationResult.Result;
		}

		public static List<GKDaySchedule> GetDaySchedules()
		{
			var operationResult = ClientManager.RubezhService.GetGKDaySchedules();
			return Common.ThrowErrorIfExists(operationResult);
		}

		public static bool SaveDaySchedule(GKDaySchedule item, bool isNew)
		{
			var operationResult = ClientManager.RubezhService.SaveGKDaySchedule(item, isNew);
            MessageBoxService.ShowWarning(operationResult.Error);
            return operationResult.Result;
		}

		public static bool DeleteDaySchedule(GKDaySchedule item)
		{
			var operationResult = ClientManager.RubezhService.DeleteGKDaySchedule(item);
            MessageBoxService.ShowWarning(operationResult.Error);
            return operationResult.Result;
		}
	}
}