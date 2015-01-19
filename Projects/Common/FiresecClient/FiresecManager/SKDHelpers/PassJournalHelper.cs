using System;

namespace FiresecClient.SKDHelpers
{
	public static class PassJournalHelper
	{
		public static bool AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			var result = FiresecManager.FiresecService.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime);
			return Common.ShowErrorIfExists(result);
		}
		public static bool EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			var result = FiresecManager.FiresecService.EditPassJournal(uid, zoneUID, enterTime, exitTime);
			return Common.ShowErrorIfExists(result);
		}
		public static bool DeletePassJournal(Guid uid)
		{
			var result = FiresecManager.FiresecService.DeletePassJournal(uid);
			return Common.ShowErrorIfExists(result);
		}
		
		
	}
}
