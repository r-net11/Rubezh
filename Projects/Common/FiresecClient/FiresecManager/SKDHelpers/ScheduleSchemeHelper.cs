using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class ScheduleSchemeHelper
	{
		public static bool Save(ScheduleScheme scheduleScheme)
		{
			var operationResult = FiresecManager.FiresecService.SaveScheduleScheme(scheduleScheme);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedScheduleScheme(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.RestoreScheduleScheme(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static ScheduleScheme GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new ScheduleSchemeFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetScheduleSchemes(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<ScheduleScheme> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetScheduleSchemes(new ScheduleSchemeFilter
				{
					OrganisationUIDs = new List<System.Guid> { organisationUID }
				});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ScheduleScheme> Get(ScheduleSchemeFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetScheduleSchemes(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}