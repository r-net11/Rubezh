using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Database;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public OperationResult<int> GetJournalLastId()
		{
			return new OperationResult<int>()
				{
					Result = DatabaseHelper.GetLastOldId()
				};
		}

		public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
		{
			var operationResult = new OperationResult<List<JournalRecord>>();
			try
			{
				bool hasWhere = false;
				string query = "";
				if (journalFilter.IsLastDaysCountActive)
				{
					query = "SELECT * FROM Journal WHERE " +
					"\n SystemTime > '" + DateTime.Now.AddDays(-journalFilter.LastDaysCount).ToString("yyyy-MM-dd HH:mm:ss") + "'";
					hasWhere = true;
				}
				else
				{
					query = "SELECT TOP (" + journalFilter.LastRecordsCount + ") * FROM Journal ";
				}

				if (journalFilter.StateTypes.Count > 0)
				{
					if (hasWhere == false)
					{
						query += " WHERE ";
						hasWhere = true;
					}
					else
					{
						query += "\n AND ";
					}
					query += " (";
					for (int i = 0; i < journalFilter.StateTypes.Count; i++)
					{
						if (i > 0)
							query += "\n OR ";
						var stateType = journalFilter.StateTypes[i];
						query += " StateType = " + ((int)stateType).ToString();
					}
					query += ")";
				}

				if (journalFilter.Categories.Count > 0)
				{
					if (hasWhere == false)
					{
						query += " WHERE ";
						hasWhere = true;
					}
					else
					{
						query += "\n AND ";
					}
					query += " (";
					for (int i = 0; i < journalFilter.Categories.Count; i++)
					{
						if (i > 0)
							query += "\n OR ";
						var category = journalFilter.Categories[i];
						query += " DeviceCategory = " + ((int)category).ToString();
					}
					query += ")";
				}

				query += "\n ORDER BY SystemTime DESC";

				var result = DataBaseContext.ExecuteQuery<JournalRecord>(query);
				operationResult.Result = result.ToList();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.GetFilteredJournal");
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
			return operationResult;
		}

		public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return OnGetFilteredArchive(archiveFilter);
		}

		public void BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		{
			var result = OnGetFilteredArchive(archiveFilter);
            CallbackArchiveCompleted(result.Result);
            //CallbackWrapper.GetFilteredArchiveCompleted(result.Result);
		}

		OperationResult<List<JournalRecord>> OnGetFilteredArchive(ArchiveFilter archiveFilter)
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

				query += "\n ORDER BY " + dateInQuery + " DESC";

				operationResult.Result = DataBaseContext.ExecuteQuery<JournalRecord>(query).ToList();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.GetFilteredArchive");
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
				Logger.Error(e, "Исключение при вызове FiresecService.GetDistinctDescriptions");
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
				Logger.Error(e, "Исключение при вызове FiresecService.GetArchiveStartDate");
				operationResult.Result = DateTime.Now;
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
			return operationResult;
		}

		public void AddJournalRecords(List<JournalRecord> journalRecords)
		{
			var operationResult = new OperationResult<bool>();
			try
			{
				journalRecords.ForEach(x => x.User = ClientCredentials.UserName);
				DatabaseHelper.AddJournalRecords(journalRecords);
				//ClientsCash.OnNewJournalRecord(journalRecord);
				operationResult.Result = true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.AddJournalRecords");
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
		}

		public void SetJournal(List<JournalRecord> journalRecords)
		{
			using (var dataContext = ConnectionManager.CreateFiresecDataContext())
			{
				dataContext.ExecuteCommand("DELETE FROM Journal");
				dataContext.JournalRecords.InsertAllOnSubmit(journalRecords);
				dataContext.SubmitChanges();
			}
		}
	}
}