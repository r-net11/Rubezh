using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace SKDDriver
{
	public static class SKDDBHelper
	{
		static SKDDatabaseService SKDService = new SKDDatabaseService();
		
		public static void Add(SKDJournalItem journalItem)
		{
			SKDService.SaveSKDJournalItems(new List<SKDJournalItem>{journalItem});	
		}

		public static void AddMany(List<SKDJournalItem> journalItems)
		{
			SKDService.SaveSKDJournalItems(journalItems);	
		}

		public static SKDJournalItem AddMessage(string name, string userName)
		{
			var journalItem = new SKDJournalItem();
			return journalItem;
		}
	}
}