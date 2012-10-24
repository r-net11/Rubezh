using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common.GK
{
    public static class GKDBHelper
    {
		public static void Add(JournalItem journalItem)
        {
            try
            {
                using (var dataContext = ConnectionManager.CreateGKDataContext())
                {
                    var journal = new Journal();
                    journal.DateTime = journalItem.DateTime;
                    journal.ObjectUID = journalItem.ObjectUID;
					journal.GKObjectNo = journalItem.GKObjectNo;
					journal.Description = journalItem.EventDescription;
                    dataContext.Journal.InsertOnSubmit(journal);
                    dataContext.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "GKDBHelper.Add");
            }
        }

		public static List<JournalItem> Select(XArchiveFilter archiveFilter)
        {
			var journalItems = new List<JournalItem>();
            try
            {
                using (var dataContext = ConnectionManager.CreateGKDataContext())
                {
					var query =
					"SELECT * FROM Journal WHERE " +
					"\n DateTime > '" + archiveFilter.StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
					"\n AND DateTime < '" + archiveFilter.EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";

                    var result = dataContext.ExecuteQuery<Journal>(query);
					foreach (var journal in result)
					{
						var journalItem = new JournalItem(journal);
						journalItems.Add(journalItem);
					}
					var journalRecordsCount = journalItems.Count;
                    Trace.WriteLine("Journal Count = " + journalRecordsCount.ToString());
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "GKDBHelper.Select");
            }
			return journalItems;
        }
    }
}