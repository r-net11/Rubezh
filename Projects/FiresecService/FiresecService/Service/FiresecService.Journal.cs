using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Processor;
using System.Diagnostics;
using FiresecService.Database;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public OperationResult<List<JournalRecord>> ReadJournal(int startIndex, int count)
		{
			var operationResult = new OperationResult<List<JournalRecord>>();
			try
			{
				var internalJournal = FiresecSerializedClient.ReadEvents(startIndex, count).Result;
				if (internalJournal != null && internalJournal.Journal.IsNotNullOrEmpty())
				{
					operationResult.Result = new List<JournalRecord>(internalJournal.Journal.Select(x => JournalConverter.Convert(x)));
				}
			}
			catch (Exception e)
			{
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
			return operationResult;
		}

		public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
		{
			var operationResult = new OperationResult<List<JournalRecord>>();
			try
			{
				JournalFilterHelper journalFilterHelper = new JournalFilterHelper(FiresecManager);

				operationResult.Result =
					DataBaseContext.JournalRecords.AsEnumerable().Reverse().
					Where(journal => journalFilter.CheckDaysConstraint(journal.SystemTime)).
					Where(journal => journalFilterHelper.FilterRecord(journalFilter, journal)).
					Take(journalFilter.LastRecordsCount).ToList();
			}
			catch (Exception e)
			{
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
			return operationResult;
		}

		public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
			var operationResult = new OperationResult<List<JournalRecord>>();
			try
			{
				string dateInQuery = "DeviceTime";
				if (archiveFilter.UseSystemDate)
					dateInQuery = "SystemTime";

				var query =
				"SELECT * FROM Journal WHERE " +
				"\n " + dateInQuery + " > '" + archiveFilter.StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
				"\n AND " + dateInQuery + " < '" + archiveFilter.EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";

				if (archiveFilter.Descriptions.Count > 0)
				{
					query += "\n AND (";
					for (int i = 0; i < archiveFilter.Descriptions.Count; i++)
					{
						if (i > 0)
							query += "\n OR ";
						var description = archiveFilter.Descriptions[i];
						query += " Description = '" + description + "'";
					}
					query += ")";
				}

				if (archiveFilter.Subsystems.Count > 0)
				{
					query += "\n AND (";
					for (int i = 0; i < archiveFilter.Subsystems.Count; i++)
					{
						if (i > 0)
							query += "\n OR ";
						var subsystem = archiveFilter.Subsystems[i];
						query += " SubSystemType = '" + ((int)subsystem).ToString() + "'";
					}
					query += ")";
				}

				if (archiveFilter.DeviceNames.Count > 0)
				{
					query += "\n AND (";
					for (int i = 0; i < archiveFilter.DeviceNames.Count; i++)
					{
						var deviceName = archiveFilter.DeviceNames[i];
						if (deviceName != null)
						{
							if (i > 0)
								query += "\n OR ";
							query += " PanelName = '" + deviceName + "'";
						}
					}
					query += ")";
				}

				var result = DataBaseContext.ExecuteQuery<JournalRecord>(query);
				operationResult.Result = result.ToList();
			}
			catch (Exception e)
			{
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
			return operationResult;
		}

		public OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
		{
			var operationResult = new OperationResult<List<JournalDescriptionItem>>();
			try
			{
				string query = "SELECT DISTINCT StateType, Description FROM Journal";
				var result = DataBaseContext.ExecuteQuery<JournalDescriptionItem>(query);
				operationResult.Result = result.ToList();
			}
			catch (Exception e)
			{
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
			return operationResult;
		}

		public OperationResult<DateTime> GetArchiveStartDate()
		{
			var operationResult = new OperationResult<DateTime>();
			try
			{
				string query = "SELECT MIN(SystemTime) AS SystemTime FROM Journal";
				var result = DataBaseContext.ExecuteQuery<DateTime>(query);
				operationResult.Result = result.First();
			}
			catch (Exception e)
			{
				operationResult.Result = DateTime.Now;
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
			return operationResult;
		}

		public void AddJournalRecord(JournalRecord journalRecord)
		{
			var operationResult = new OperationResult<bool>();
			try
			{
				journalRecord.User = _userName;
				DatabaseHelper.AddJournalRecord(journalRecord);
				CallbackManager.OnNewJournalRecord(journalRecord);
				operationResult.Result = true;
			}
			catch (Exception e)
			{
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
		}

		public void ConvertJournal()
		{
			using (var dataContext = ConnectionManager.CreateFiresecDataContext())
			{
				dataContext.ExecuteCommand("DELETE FROM Journal");

				int count = 0;
				int LastEventId = 0;
				//while (true)
				{
					bool hasNewRecords = false;
					//while (true)
					{
						var document = FiresecSerializedClient.ReadEvents(LastEventId, 100000).Result;

						if (document == null || document.Journal == null || document.Journal.Count() == 0)
							return;//break;

						int newLastValue = LastEventId;
						foreach (var innerJournalItem in document.Journal)
						{
							var id = int.Parse(innerJournalItem.IDEvents);
							if (id > LastEventId)
							{
								count++;
								hasNewRecords = true;
								newLastValue = id;
								var journalRecord = JournalConverter.Convert(innerJournalItem);
								dataContext.JournalRecords.InsertOnSubmit(journalRecord);

								Trace.WriteLine(innerJournalItem.IDEvents + " - " + innerJournalItem.SysDt);
							}
						}
						//if (LastEventId == newLastValue)
						//    break;

						LastEventId = newLastValue;
					}
					//if (hasNewRecords == false)
					//    break;
				}

				dataContext.SubmitChanges();
				Trace.WriteLine("Count = " + count.ToString());
			}
		}
	}
}