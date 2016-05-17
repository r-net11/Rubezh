using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhClient.SKDHelpers
{
	public static class DayIntervalHelper
	{
		public static bool Save(DayInterval dayInterval, bool isNew)
		{
			var operationResult = ClientManager.RubezhService.SaveDayInterval(dayInterval, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(DayInterval item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(DayInterval item)
		{
			return Restore(item.UID, item.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var operationResult = ClientManager.RubezhService.MarkDeletedDayInterval(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = ClientManager.RubezhService.RestoreDayInterval(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<DayInterval> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.RubezhService.GetDayIntervals(new DayIntervalFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<DayInterval> Get(DayIntervalFilter filter, bool isShowError = true)
		{
			var operationResult = ClientManager.RubezhService.GetDayIntervals(filter);
			return Common.ShowErrorIfExists(operationResult, isShowError);
		}

		public static DayInterval GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new DayIntervalFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = ClientManager.RubezhService.GetDayIntervals(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}
	}
}