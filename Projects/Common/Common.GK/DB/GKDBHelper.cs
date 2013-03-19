using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK.DB;
using XFiresecAPI;
using System.IO;
using Infrastructure.Common;

namespace Common.GK
{
    public static class GKDBHelper
    {
		public static object locker = new object();

		public static Journal JournalToJournalItem(JournalItem journalItem)
		{
			var journal = new Journal();
			journal.GKIpAddress = journalItem.GKIpAddress;
			journal.GKJournalRecordNo = journalItem.GKJournalRecordNo;
			journal.JournalItemType = (byte)journalItem.JournalItemType;
			journal.DateTime = journalItem.DateTime;
			journal.Name = journalItem.Name;
			journal.YesNo = journalItem.YesNo;
			journal.Description = journalItem.Description;
			journal.ObjectUID = journalItem.ObjectUID;
			journal.ObjectState = journalItem.ObjectState;
			journal.StateClass = (byte)journalItem.StateClass;
			return journal;
		}

		public static void Add(JournalItem journalItem)
        {
            try
            {
                lock (locker)
                {
                    if (File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
                    {
                        using (var dataContext = ConnectionManager.CreateGKDataContext())
                        {
                            var journal = JournalToJournalItem(journalItem);
                            dataContext.Journal.InsertOnSubmit(journal);
                            dataContext.SubmitChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "GKDBHelper.Add");
            }
        }

		public static void AddMany(List<JournalItem> journalItems)
		{
			try
			{
				if (journalItems.Count == 0)
					return;

				lock (locker)
				{
					using (var dataContext = ConnectionManager.CreateGKDataContext())
					{
						var journals = new List<Journal>();
						foreach (var journalItem in journalItems)
						{
							var journal = JournalToJournalItem(journalItem);
							journals.Add(journal);
						}
						dataContext.Journal.InsertAllOnSubmit(journals);
						dataContext.SubmitChanges();
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDBHelper.AddMany");
			}
		}

        public static void AddMessage(string message)
        {
			var journalItem = new JournalItem()
			{
				DateTime = DateTime.Now,
				JournalItemType = JournalItemType.System,
				StateClass = XStateClass.Norm,
				Name = message
			};
            Add(journalItem);
        }

		public static List<JournalItem> Select(XArchiveFilter archiveFilter)
        {
			var journalItems = new List<JournalItem>();
			try
			{
				lock (locker)
				{
					using (var dataContext = ConnectionManager.CreateGKDataContext())
					{
						var query =
						"SELECT * FROM Journal WHERE " +
						"\n DateTime > '" + archiveFilter.StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
						"\n AND DateTime < '" + archiveFilter.EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";

						if (archiveFilter.JournalItemTypes.Count > 0)
						{
							query += "\n AND (";
							int index = 0;
							foreach (var journalItemType in archiveFilter.JournalItemTypes)
							{
								if (index > 0)
									query += "\n OR ";
								index++;
								query += "JournalItemType = '" + ((int)journalItemType).ToString() + "'";
							}
							query += ")";
						}

						if (archiveFilter.StateClasses.Count > 0)
						{
							query += "\n AND (";
							int index = 0;
							foreach (var stateClass in archiveFilter.StateClasses)
							{
								if (index > 0)
									query += "\n OR ";
								index++;
								query += "StateClass = '" + ((int)stateClass).ToString() + "'";
							}
							query += ")";
						}

						if (archiveFilter.GKAddresses.Count > 0)
						{
							query += "\n AND (";
							int index = 0;
							foreach (var addresses in archiveFilter.GKAddresses)
							{
								if (index > 0)
									query += "\n OR ";
								index++;
								query += "GKIpAddress = '" + addresses + "'";
							}
							query += ")";
						}

						if (archiveFilter.EventNames.Count > 0)
						{
							query += "\n AND (";
							int index = 0;
							foreach (var eventName in archiveFilter.EventNames)
							{
								if (index > 0)
									query += "\n OR ";
								index++;
								query += "Name = '" + eventName + "'";
							}
							query += ")";
						}

						query += "\n ORDER BY DateTime DESC";

						var result = dataContext.ExecuteQuery<Journal>(query);
						foreach (var journal in result)
						{
							var journalItem = JournalItem.FromJournal(journal);
							journalItems.Add(journalItem);
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDBHelper.Select");
			}
			return journalItems;
        }

        public static int GetLastGKID(string gkIPAddress)
        {
			try
			{
				lock (locker)
				{
					using (var dataContext = ConnectionManager.CreateGKDataContext())
					{
						var query =
						"SELECT MAX(GKJournalRecordNo) FROM Journal WHERE GKIpAddress = '" + gkIPAddress + "'";

						var result = dataContext.ExecuteQuery<int?>(query);
						var firstResult = result.FirstOrDefault();
						if (firstResult != null)
							return firstResult.Value;
						return -1;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDBHelper.GetLastGKID");
				return -1;
			}
        }

        public static List<string> GetGKIPAddresses()
        {
			try
			{
				lock (locker)
				{
					if (File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
					{
						using (var dataContext = ConnectionManager.CreateGKDataContext())
						{
							string query = "SELECT DISTINCT GKIpAddress FROM Journal";
							var result = dataContext.ExecuteQuery<string>(query);
							var addresses = result.ToList();
							addresses.RemoveAll(x => string.IsNullOrEmpty(x));
							return addresses;
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDBHelper.GetGKIPAddresses");
			}
			return new List<string>();
        }

		public static List<JournalItem> GetTopLast(int count)
		{
			var journalItems = new List<JournalItem>();
			try
			{
				lock (locker)
				{
					if (File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
					{
						using (var dataContext = ConnectionManager.CreateGKDataContext())
						{
							var query = "SELECT TOP (" + count.ToString() + ") * FROM Journal ORDER BY DateTime DESC";
							var result = dataContext.ExecuteQuery<Journal>(query);
							foreach (var journal in result)
							{
								var journalItem = JournalItem.FromJournal(journal);
								journalItems.Add(journalItem);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDBHelper.GetTopLast");
			}
			journalItems.Reverse();
			return journalItems;
		}
    }
}