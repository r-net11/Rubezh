using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace SKDDriver
{
	public static class SKDDBHelper
	{
		public static void Add(SKDJournalItem journalItem)
		{
			SKDDatabaseService.JournalItemTranslator.Save(new List<SKDJournalItem> { journalItem });	
		}

		public static void AddMany(List<SKDJournalItem> journalItems)
		{
			SKDDatabaseService.JournalItemTranslator.Save(journalItems);	
		}

		public static SKDJournalItem AddMessage(string name, string userName)
		{
			var result = new SKDJournalItem();
			result.Name = name;
			result.UserName = userName;
			return result;
		}
	}
}