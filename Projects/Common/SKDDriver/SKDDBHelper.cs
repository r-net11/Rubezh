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

		}

		public static void AddMany(List<SKDJournalItem> journalItems)
		{

		}

		public static SKDJournalItem AddMessage(string name, string userName)
		{
			var journalItem = new SKDJournalItem();
			return journalItem;
		}
	}
}