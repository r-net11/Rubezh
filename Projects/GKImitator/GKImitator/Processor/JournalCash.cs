using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKImitator.Processor
{
	public static class JournalCash
	{
		static Object lockObject = new Object();
		static List<ImitatorJournalItem> ImitatorJournalItems;
		public static int Count { get; private set; }

		static JournalCash()
		{
			ImitatorJournalItems = new List<ImitatorJournalItem>();

			using (var dbService = new DbService())
			{
				Count = dbService.ImitatorJournalTranslator.GetCount();
			}

			for (int i = Math.Max(1, Count - 1000); i < Count; i++)
			{
				using (var dbService = new DbService())
				{
					var imitatorJournalItem = dbService.ImitatorJournalTranslator.GetByGKNo(i);
					if (imitatorJournalItem != null)
					{
						ImitatorJournalItems.Add(imitatorJournalItem);
					}
				}
			}
		}

		public static ImitatorJournalItem GetByGKNo(int gkNo)
		{
			ImitatorJournalItem imitatorJournalItem;
			lock (lockObject)
			{
				imitatorJournalItem = ImitatorJournalItems.FirstOrDefault(x => x.GkNo == gkNo);
			}
			if (imitatorJournalItem == null)
			{
				using (var dbService = new DbService())
				{
					imitatorJournalItem = dbService.ImitatorJournalTranslator.GetByGKNo(gkNo);
				}
			}
			return imitatorJournalItem;
		}

		public static void Add(ImitatorJournalItem imitatorJournalItem)
		{
			lock (lockObject)
			{
				ImitatorJournalItems.Add(imitatorJournalItem);
				if (ImitatorJournalItems.Count > 1000)
					ImitatorJournalItems.RemoveAt(0);
				Count = imitatorJournalItem.GkNo;
			}
		}
	}
}