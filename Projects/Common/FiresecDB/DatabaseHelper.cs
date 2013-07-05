using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Data.SqlServerCe;

namespace FiresecDB
{
	public static class DatabaseHelper
	{
		public static string ConnectionString { get; set; }
		public static List<JournalRecord> AddJournalRecords(List<JournalRecord> journalRecords)
		{
			var sortedJournalRecords = new List<JournalRecord>();
			try
			{
				foreach (var journalRecord in journalRecords)
				{
					if (journalRecord.Description == "Потеря связи с пользователем." ||
						journalRecord.Description == "Вход пользователя в систему" ||
						journalRecord.Description == "Неизвестный запрос")
						continue;
					sortedJournalRecords.Add(journalRecord);
				}
				if (sortedJournalRecords.Count > 0)
					InsertJournalRecordToDb(sortedJournalRecords);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове DatabaseHelper.AddJournalRecords");
			}
			return sortedJournalRecords;
		}

		public static int GetLastOldId()
		{
			try
			{
				using (var dataContext = new SqlCeConnection(ConnectionString))
				{
					var query = "SELECT MAX(OldId) FROM Journal";
					var result = new SqlCeCommand(query, dataContext);
					dataContext.Open();
					var reader = result.ExecuteReader();
					int? firstResult = null;
					if (reader.Read())
						firstResult = (int)reader[0];
					dataContext.Close();
					if (firstResult != null)
						return (int)firstResult;
					return -1;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове DatabaseHelper.GetLastOldId");
			}
			return -1;
		}

		public static void InsertJournalRecordToDb(List<JournalRecord> journalRecords)
		{
			using (var dataContext = new SqlCeConnection(ConnectionString))
			{
				dataContext.ConnectionString = ConnectionString;
				dataContext.Open();
				foreach (var journalRecord in journalRecords)
				{
					var query = "SELECT * FROM Journal WHERE " +
								"\n SystemTime = '" + journalRecord.SystemTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
								"\n AND OldId = " + journalRecord.OldId;
					var result = new SqlCeCommand(query, dataContext);
					var reader = result.ExecuteReader();
					if (reader.Read())
					{
						continue;
					}

					var sqlCeCommand = new SqlCeCommand();
					sqlCeCommand.Connection = dataContext;
					sqlCeCommand.CommandText = @"Insert Into Journal" +
						"(Description,Detalization,DeviceCategory,DeviceDatabaseId,DeviceName,DeviceTime,OldId,PanelDatabaseId,PanelName,StateType,SubsystemType,SystemTime,UserName,ZoneName) Values" +
						"(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14)";
					sqlCeCommand.Parameters.AddWithValue("@p1", (object)journalRecord.Description ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p2", (object)journalRecord.Detalization ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p3", (object)journalRecord.DeviceCategory ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p4", (object)journalRecord.DeviceDatabaseId ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p5", (object)journalRecord.DeviceName ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p6", (object)journalRecord.DeviceTime ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p7", (object)journalRecord.OldId ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p8", (object)journalRecord.PanelDatabaseId ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p9", (object)journalRecord.PanelName ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p10", (object)journalRecord.StateType ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p11", (object)journalRecord.SubsystemType ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p12", (object)journalRecord.SystemTime ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p13", (object)journalRecord.User ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p14", (object)journalRecord.ZoneName ?? DBNull.Value);
					sqlCeCommand.ExecuteNonQuery();
				}
				dataContext.Close();
			}
		}

		public static OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
		{
			var operationResult = new OperationResult<List<JournalRecord>>();
			operationResult.Result = new List<JournalRecord>();
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

				using (var DataBaseContext = new SqlCeConnection(ConnectionString))
				{
					DataBaseContext.ConnectionString = ConnectionString;
					var result = new SqlCeCommand(query, DataBaseContext);
					DataBaseContext.Open();
					var reader = result.ExecuteReader();
					while (reader.Read())
					{
						var journalRecord = ReadOneJournalRecord(reader);
						operationResult.Result.Add(journalRecord);
					}
					DataBaseContext.Close();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.GetFilteredJournal");
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
			return operationResult;
		}
		public static bool IsAbort { get; set; }
		public static OperationResult<List<JournalRecord>> OnGetFilteredArchive(ArchiveFilter archiveFilter, bool isReport)
		{
			var operationResult = new OperationResult<List<JournalRecord>>();
			operationResult.Result = new List<JournalRecord>();
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
				//query = "SELECT * FROM Journal";

				using (var DataBaseContext = new SqlCeConnection(ConnectionString))
				{
					DataBaseContext.ConnectionString = ConnectionString;
					var journalRecords = new List<JournalRecord>();
					var result = new SqlCeCommand(query, DataBaseContext);
					DataBaseContext.Open();
					var reader = result.ExecuteReader();
					while (reader.Read())
					{
						if (IsAbort && !isReport)
							break;
						try
						{
							var journalRecord = ReadOneJournalRecord(reader);
							operationResult.Result.Add(journalRecord);
							if (!isReport)
							{
								journalRecords.Add(journalRecord);
								if (journalRecords.Count > 100)
								{
									if (ArchivePortionReady != null)
										ArchivePortionReady(journalRecords.ToList());

									journalRecords.Clear();
								}
							}
						}
						catch (Exception e)
						{
							Logger.Error(e, "DatabaseHelper.OnGetFilteredArchive");
						}
					}
					if (!isReport)
					{
						if (ArchivePortionReady != null)
							ArchivePortionReady(journalRecords.ToList());
					}

					DataBaseContext.Close();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.GetFilteredArchive");
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
			return operationResult;
		}

		static JournalRecord ReadOneJournalRecord(SqlCeDataReader reader)
		{
			var journalRecord = new JournalRecord();
			if (!reader.IsDBNull(reader.GetOrdinal("Description")))
				journalRecord.Description = reader.GetString(reader.GetOrdinal("Description"));
			if (!reader.IsDBNull(reader.GetOrdinal("Detalization")))
				journalRecord.Detalization = reader.GetString(reader.GetOrdinal("Detalization"));
			if (!reader.IsDBNull(reader.GetOrdinal("DeviceCategory")))
				journalRecord.DeviceCategory = reader.GetInt32(reader.GetOrdinal("DeviceCategory"));
			if (!reader.IsDBNull(reader.GetOrdinal("DeviceDatabaseId")))
				journalRecord.DeviceDatabaseId = reader.GetString(reader.GetOrdinal("DeviceDatabaseId"));
			if (!reader.IsDBNull(reader.GetOrdinal("DeviceName")))
				journalRecord.DeviceName = reader.GetString(reader.GetOrdinal("DeviceName"));
			if (!reader.IsDBNull(reader.GetOrdinal("DeviceTime")))
				journalRecord.DeviceTime = reader.GetDateTime(reader.GetOrdinal("DeviceTime"));
			if (!reader.IsDBNull(reader.GetOrdinal("OldId")))
				journalRecord.OldId = reader.GetInt32(reader.GetOrdinal("OldId"));
			if (!reader.IsDBNull(reader.GetOrdinal("PanelDatabaseId")))
				journalRecord.PanelDatabaseId = reader.GetString(reader.GetOrdinal("PanelDatabaseId"));
			if (!reader.IsDBNull(reader.GetOrdinal("PanelName")))
				journalRecord.PanelName = reader.GetString(reader.GetOrdinal("PanelName"));
			if (!reader.IsDBNull(reader.GetOrdinal("StateType")))
				journalRecord.StateType = (StateType)reader.GetInt32(reader.GetOrdinal("StateType"));
			if (!reader.IsDBNull(reader.GetOrdinal("SubsystemType")))
				journalRecord.SubsystemType = (SubsystemType)reader.GetInt32(reader.GetOrdinal("SubsystemType"));
			if (!reader.IsDBNull(reader.GetOrdinal("SystemTime")))
				journalRecord.SystemTime = reader.GetDateTime(reader.GetOrdinal("SystemTime"));
			if (!reader.IsDBNull(reader.GetOrdinal("UserName")))
				journalRecord.User = reader.GetString(reader.GetOrdinal("UserName"));
			if (!reader.IsDBNull(reader.GetOrdinal("ZoneName")))
				journalRecord.ZoneName = reader.GetString(reader.GetOrdinal("ZoneName"));
			if (!reader.IsDBNull(reader.GetOrdinal("Id")))
				journalRecord.No = reader.GetInt32(reader.GetOrdinal("Id"));
			return journalRecord;
		}

		public static event Action<List<JournalRecord>> ArchivePortionReady;

		public static OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
		{
			var operationResult = new OperationResult<List<JournalDescriptionItem>>();
			operationResult.Result = new List<JournalDescriptionItem>();
			try
			{
				string query = "SELECT DISTINCT TOP (1000) StateType, Description FROM Journal ORDER BY Description";
				using (var DataBaseContext = new SqlCeConnection(ConnectionString))
				{
					DataBaseContext.ConnectionString = ConnectionString;
					var result = new SqlCeCommand(query, DataBaseContext);
					DataBaseContext.Open();
					var reader = result.ExecuteReader();
					while (reader.Read())
					{
						var journalDescriptionItem = new JournalDescriptionItem();
						journalDescriptionItem.Description = reader.GetString(reader.GetOrdinal("Description"));
						journalDescriptionItem.StateType = (StateType)reader.GetInt32(reader.GetOrdinal("StateType"));
						operationResult.Result.Add(journalDescriptionItem);
					}
					DataBaseContext.Close();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.GetDistinctDescriptions");
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
			return operationResult;
		}

		public static OperationResult<DateTime> GetArchiveStartDate()
		{
			var operationResult = new OperationResult<DateTime>();
			try
			{
				string query = "SELECT MIN(SystemTime) AS SystemTime FROM Journal";
				using (var DataBaseContext = new SqlCeConnection(ConnectionString))
				{
					DataBaseContext.ConnectionString = ConnectionString;
					var result = new SqlCeCommand(query, DataBaseContext);
					DataBaseContext.Open();
					var reader = result.ExecuteReader();
					if (reader.Read())
					{
						operationResult.Result = reader.GetDateTime(reader.GetOrdinal("SystemTime"));
					}
					DataBaseContext.Close();
				}
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

		public static void SetJournal(List<JournalRecord> journalRecords)
		{
			using (var DataBaseContext = new SqlCeConnection(ConnectionString))
			{
				DataBaseContext.ConnectionString = ConnectionString;
				var result = new SqlCeCommand("DELETE FROM Journal", DataBaseContext);
				DataBaseContext.Open();
				result.ExecuteReader();
				InsertJournalRecordToDb(journalRecords);
				DataBaseContext.Close();
			}
		}
	}
}