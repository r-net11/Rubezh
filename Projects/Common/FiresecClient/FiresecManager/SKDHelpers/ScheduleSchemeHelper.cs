using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class ScheduleSchemeHelper
	{
		public static bool Save(ScheduleScheme scheduleScheme, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SaveScheduleScheme(scheduleScheme, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(ScheduleScheme item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(ScheduleScheme item)
		{
			return Restore(item.UID, item.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedScheduleScheme(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.RestoreScheduleScheme(uid, name);
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