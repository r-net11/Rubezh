using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;

namespace RubezhClient.SKDHelpers
{
	public static class ScheduleSchemeHelper
	{
		public static bool Save(ScheduleScheme scheduleScheme, bool isNew)
		{
			var operationResult = ClientManager.RubezhService.SaveScheduleScheme(scheduleScheme, isNew);
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
			var operationResult = ClientManager.RubezhService.MarkDeletedScheduleScheme(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = ClientManager.RubezhService.RestoreScheduleScheme(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static ScheduleScheme GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new ScheduleSchemeFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = ClientManager.RubezhService.GetScheduleSchemes(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<ScheduleScheme> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.RubezhService.GetScheduleSchemes(new ScheduleSchemeFilter
				{
					OrganisationUIDs = new List<System.Guid> { organisationUID }
				});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ScheduleScheme> Get(ScheduleSchemeFilter filter, bool showError = true)
		{
			var operationResult = ClientManager.RubezhService.GetScheduleSchemes(filter);
			return Common.ShowErrorIfExists(operationResult, showError);
		}
	}
}