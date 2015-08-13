using System;
using System.Collections.Generic;
using System.Windows.Documents;
using FiresecAPI.SKD;


namespace FiresecClient.SKDHelpers
{
	public static class PassJournalHelper
	{
		public static Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>> FindConflictIntervals(List<DayTimeTrackPart> dayTimeTracks, Guid employeeGuid, DateTime currentDate)
		{
			var result = FiresecManager.FiresecService.FindConflictIntervals(dayTimeTracks, employeeGuid, currentDate);
			return Common.ShowErrorIfExists(result);
		}

		public static bool AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime? enterTime, DateTime? exitTime,
			DateTime? adjustmentDate, Guid correctedBy, bool notTakeInCalculations, bool isAddedManually, DateTime? enterTimeOriginal, DateTime? exitTimeOriginal, bool isRemoveAllIntersections)
		{
			var result = FiresecManager.FiresecService.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime,
				adjustmentDate, correctedBy, notTakeInCalculations, isAddedManually, enterTimeOriginal, exitTimeOriginal, isRemoveAllIntersections);
			return Common.ShowErrorIfExists(result);
		}
		public static bool EditPassJournal(Guid uid, Guid zoneUID, DateTime? enterTime, DateTime? exitTime,
			bool isNeedAdjustment, DateTime? adjustmentDate, Guid correctedBy, bool notTakeInCalculations, bool isAddedManually, bool isRemoveAllIntersections, Guid employeeGuid)
		{
			var result = FiresecManager.FiresecService.EditPassJournal(uid, zoneUID, enterTime, exitTime,
				isNeedAdjustment, adjustmentDate, correctedBy, notTakeInCalculations, isAddedManually, isRemoveAllIntersections, employeeGuid);
			return Common.ShowErrorIfExists(result);
		}
		public static bool DeletePassJournal(Guid uid)
		{
			var result = FiresecManager.FiresecService.DeletePassJournal(uid);
			return Common.ShowErrorIfExists(result);
		}
		public static bool DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			var result = FiresecManager.FiresecService.DeleteAllPassJournalItems(uid, enterTime, exitTime);
			return Common.ShowErrorIfExists(result);
		}
		public static DateTime? GetMinPassJournalDate()
		{
			var result = FiresecManager.FiresecService.GetPassJournalMinDate();
			return Common.ShowErrorIfExists(result);
		}
		public static DateTime? GetMinJournalDate()
		{
			var result = FiresecManager.FiresecService.GetMinJournalDateTime();
			return Common.ShowErrorIfExists(result);
		}
	}
}