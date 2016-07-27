using System;
using System.Collections.Generic;
using StrazhAPI.Models;
using StrazhAPI.SKD;


namespace FiresecClient.SKDHelpers
{
	public static class PassJournalHelper
	{
		public static bool CheckForCanForseCloseInterval(Guid openedIntervalGuid)
		{
			return FiresecManager.FiresecService.CheckForCanForseCloseInterval(openedIntervalGuid).Result;
		}

		public static IEnumerable<DayTimeTrackPart> GetIntersectionIntervals(DayTimeTrackPart currentDayTimeTrackPart,
			ShortEmployee currentEmployee)
		{
			var result = FiresecManager.FiresecService.GetIntersectionIntervals(currentDayTimeTrackPart, currentEmployee);
			return Common.ShowErrorIfExists(result);
		}

		public static bool DeletePassJournal(Guid uid)
		{
			var result = FiresecManager.FiresecService.DeletePassJournal(uid);
			return Common.ShowErrorIfExists(result);
		}

		public static bool DeleteAllPassJournalItems(DayTimeTrackPart dayTimeTrackPart)
		{
			var result = FiresecManager.FiresecService.DeleteAllPassJournalItems(dayTimeTrackPart);
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

		public static bool SaveAllTimeTracks(IEnumerable<DayTimeTrackPart> collectionToSave, ShortEmployee employee, User currentUser, IEnumerable<DayTimeTrackPart> removeDayTimeTrackParts)
		{
			var result = FiresecManager.FiresecService.SaveAllTimeTracks(collectionToSave, employee, currentUser, removeDayTimeTrackParts);
			return Common.ShowErrorIfExists(result);
		}
	}
}