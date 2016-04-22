using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhClient.SKDHelpers
{
	public static class ScheduleHelper
	{
		public static bool Save(Schedule scheme, bool isNew)
		{
			var operationResult = ClientManager.FiresecService.SaveSchedule(scheme, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Schedule item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(Schedule item)
		{
			return Restore(item.UID, item.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var operationResult = ClientManager.FiresecService.MarkDeletedSchedule(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = ClientManager.FiresecService.RestoreSchedule(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Schedule GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new ScheduleFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = ClientManager.FiresecService.GetSchedules(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<Schedule> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.FiresecService.GetSchedules(new ScheduleFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<Schedule> Get(ScheduleFilter filter, bool isShowError = true)
		{
			var operationResult = ClientManager.FiresecService.GetSchedules(filter);
			return Common.ShowErrorIfExists(operationResult, isShowError);
		}
	}
}