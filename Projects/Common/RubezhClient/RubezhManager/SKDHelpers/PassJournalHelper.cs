using System;

namespace RubezhClient.SKDHelpers
{
	public static class PassJournalHelper
	{
		public static bool AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			var result = ClientManager.RubezhService.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime);
			return Common.ShowErrorIfExists(result);
		}
		public static bool EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			var result = ClientManager.RubezhService.EditPassJournal(uid, zoneUID, enterTime, exitTime);
			return Common.ShowErrorIfExists(result);
		}
		public static bool DeletePassJournal(Guid uid)
		{
			var result = ClientManager.RubezhService.DeletePassJournal(uid);
			return Common.ShowErrorIfExists(result);
		}
		public static bool DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			var result = ClientManager.RubezhService.DeleteAllPassJournalItems(uid, enterTime, exitTime);
			return Common.ShowErrorIfExists(result);
		}
		public static DateTime? GetMinPassJournalDate()
		{
			var result = ClientManager.RubezhService.GetPassJournalMinDate();
			return Common.ShowErrorIfExists(result);
		}
		public static DateTime? GetMinJournalDate()
		{
			var result = ClientManager.RubezhService.GetMinJournalDateTime();
			return Common.ShowErrorIfExists(result);
		}
	}
}