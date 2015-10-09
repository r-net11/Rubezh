using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhDAL.DataClasses
{
	public class ImitatorJournalTranslator
	{
		public DbService DbService { get; private set; }
		public DatabaseContext Context { get { return DbService.Context; } }
		public ImitatorJournalTranslator(DbService dbService)
		{
			DbService = dbService;
		}

		public int GetCount()
		{
			try
			{
				var count = Context.ImitatorJournalItems.Count();
				return count;
			}
			catch
			{
				return 0;
			}
		}

		public ImitatorJournalItem GetByGKNo(int gkNo)
		{
			try
			{
				var journalItem = Context.ImitatorJournalItems.FirstOrDefault(x=>x.GkNo == gkNo);
				return journalItem;
			}
			catch
			{
				return null;
			}
		}

		public void Add(ImitatorJournalItem imitatorJournalItem)
		{
			try
			{
				imitatorJournalItem.GkNo = Context.ImitatorJournalItems.Count() + 1;
				imitatorJournalItem.DateTime = imitatorJournalItem.DateTime.CheckDate();
				Context.ImitatorJournalItems.Add(imitatorJournalItem);
				Context.SaveChanges();
			}
			catch
			{

			}
		}
	}
}