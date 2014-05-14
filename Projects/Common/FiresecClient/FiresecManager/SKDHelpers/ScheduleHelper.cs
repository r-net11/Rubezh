﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecClient.SKDHelpers
{
	public static class ScheduleHelper
	{
		public static bool Save(Schedule scheme)
		{
			var operationResult = FiresecManager.FiresecService.SaveSchedules(new List<Schedule> { scheme });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Schedule scheme)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedSchedules(new List<Schedule> { scheme });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Schedule GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new ScheduleFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetSchedules(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<Schedule> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetSchedules(new ScheduleFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<Schedule> Get(ScheduleFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetSchedules(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}