using Infrastructure.Common.Windows;
using RubezhAPI.GK;
using System.Collections.Generic;
using System.Linq;

namespace RubezhClient.SKDHelpers
{
	public static class GKScheduleHelper
	{
		public static List<GKSchedule> GetSchedules(bool isShowError = true)
		{
			var operationResult = ClientManager.RubezhService.GetGKSchedules();
			if (operationResult.Result != null)
				operationResult.Result.ForEach(x => x.ScheduleParts = x.ScheduleParts.OrderBy(y => y.DayNo).ToList());
			return Common.ShowErrorIfExists(operationResult, isShowError);
		}

		public static bool SaveSchedule(GKSchedule item, bool isNew)
		{
			var operationResult = ClientManager.RubezhService.SaveGKSchedule(item, isNew);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}

		public static bool DeleteSchedule(GKSchedule item)
		{
			var operationResult = ClientManager.RubezhService.DeleteGKSchedule(item);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}

		public static List<GKDaySchedule> GetDaySchedules(bool isShowError = true)
		{
			var operationResult = ClientManager.RubezhService.GetGKDaySchedules();
			return Common.ShowErrorIfExists(operationResult, isShowError);
		}

		public static bool SaveDaySchedule(GKDaySchedule item, bool isNew)
		{
			var operationResult = ClientManager.RubezhService.SaveGKDaySchedule(item, isNew);
			if (operationResult == null)
				return false;
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}

		public static bool DeleteDaySchedule(GKDaySchedule item)
		{
			var operationResult = ClientManager.RubezhService.DeleteGKDaySchedule(item);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}
	}
}