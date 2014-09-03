using System;
using System.Collections.Generic;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class DayIntervalHelper
	{
		public static bool Save(DayInterval dayInterval)
		{
			var operationResult = FiresecManager.FiresecService.SaveDayInterval(dayInterval);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedDayInterval(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<DayInterval> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetDayIntervals(new DayIntervalFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<DayInterval> Get(DayIntervalFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetDayIntervals(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}