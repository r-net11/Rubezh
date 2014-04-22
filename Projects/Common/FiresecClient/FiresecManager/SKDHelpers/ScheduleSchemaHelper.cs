using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecClient.SKDHelpers
{
	public static class ScheduleSchemaHelper
	{
		public static bool Save(ScheduleScheme scheduleScheme)
		{
			var operationResult = FiresecManager.FiresecService.SaveScheduleSchemes(new List<ScheduleScheme> { scheduleScheme });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(ScheduleScheme scheduleScheme)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedScheduleSchemes(new List<ScheduleScheme> { scheduleScheme });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static ScheduleScheme GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new ScheduleSchemeFilter();
			filter.Uids.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetScheduleSchemes(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<ScheduleScheme> GetByOrganisation(Guid? organisationUID)
		{
			if (organisationUID == null)
				return null;
			var filter = new ScheduleSchemeFilter();
			filter.OrganisationUIDs.Add(organisationUID.Value);
			var operationResult = FiresecManager.FiresecService.GetScheduleSchemes(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ScheduleScheme> Get(ScheduleSchemeFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetScheduleSchemes(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
