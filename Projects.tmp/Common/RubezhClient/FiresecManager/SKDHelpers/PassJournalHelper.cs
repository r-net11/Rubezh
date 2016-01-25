using System;

namespace RubezhClient.SKDHelpers
{
	public static class PassJournalHelper
	{
		public static bool AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			var result = ClientManager.FiresecService.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime);
			return Common.ShowErrorIfExists(result);
		}
		public static bool EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			var result = ClientManager.FiresecService.EditPassJournal(uid, zoneUID, enterTime, exitTime);
			return Common.ShowErrorIfExists(result);
		}
		public static bool DeletePassJournal(Guid uid)
		{
			var result = ClientManager.FiresecService.DeletePassJournal(uid);
			return Common.ShowErrorIfExists(result);
		}
		public static bool DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			var result = ClientManager.FiresecService.DeleteAllPassJournalItems(uid, enterTime, exitTime);
			return Common.ShowErrorIfExists(result);
		}
		public static DateTime? GetMinPassJournalDate()
		{
			var result = ClientManager.FiresecService.GetPassJournalMinDate();
			return Common.ShowErrorIfExists(result);
		}
		public static DateTime? GetMinJournalDate()
		{
			var result = ClientManager.FiresecService.GetMinJournalDateTime();
			return Common.ShowErrorIfExists(result);
		}
	}
}