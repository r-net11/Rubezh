﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common.Windows;

namespace RubezhClient.SKDHelpers
{
	public static class GKScheduleHelper
	{
		public static List<GKSchedule> GetSchedules()
		{
			var operationResult = ClientManager.FiresecService.GetGKSchedules();
			if (operationResult.Result != null)
				operationResult.Result.ForEach(x => x.ScheduleParts = x.ScheduleParts.OrderBy(y => y.DayNo).ToList());
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool SaveSchedule(GKSchedule item, bool isNew)
		{
			var operationResult = ClientManager.FiresecService.SaveGKSchedule(item, isNew);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}

		public static bool DeleteSchedule(GKSchedule item)
		{
			var operationResult = ClientManager.FiresecService.DeleteGKSchedule(item);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}

		public static List<GKDaySchedule> GetDaySchedules()
		{
			var operationResult = ClientManager.FiresecService.GetGKDaySchedules();
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool SaveDaySchedule(GKDaySchedule item, bool isNew)
		{
			var operationResult = ClientManager.FiresecService.SaveGKDaySchedule(item, isNew);
			if (operationResult == null)
				return false;
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}

		public static bool DeleteDaySchedule(GKDaySchedule item)
		{
			var operationResult = ClientManager.FiresecService.DeleteGKDaySchedule(item);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			return operationResult.Result;
		}
	}
}