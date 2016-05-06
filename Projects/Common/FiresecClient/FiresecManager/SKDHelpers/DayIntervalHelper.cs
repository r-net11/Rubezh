using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows;

namespace FiresecClient.SKDHelpers
{
	public static class DayIntervalHelper
	{
		public static bool Save(DayInterval dayInterval, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SaveDayInterval(dayInterval, isNew);
			if (operationResult.HasError)
			{
				MessageBoxService.ShowWarning(message: operationResult.Error, width: 400, height: 180);
				return false;
			}
			return true;
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
			var operationResult = FiresecManager.FiresecService.MarkDeletedDayInterval(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.RestoreDayInterval(uid, name);
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


		public static DayInterval GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new DayIntervalFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetDayIntervals(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}
	}
}