using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS2Api;
using Common;
using System.Data.SqlServerCe;
using FiresecAPI;
using FiresecAPI.Models;

namespace ServerFS2.Journal
{
	public static partial class DatabaseHelper
	{
		public static List<FS2JournalItem> GetFilteredJournal(JournalFilter journalFilter)
		{
			var result = new List<FS2JournalItem>();
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

				using (var sqlCeConnection = new SqlCeConnection(ConnectionString))
				{
					sqlCeConnection.ConnectionString = ConnectionString;
					var sqlCeCommand = new SqlCeCommand(query, sqlCeConnection);
					sqlCeConnection.Open();
					var reader = sqlCeCommand.ExecuteReader();
					while (reader.Read())
					{
						var journalItem = ReadOneJournalItem(reader);
						result.Add(journalItem);
					}
					sqlCeConnection.Close();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.GetFilteredJournal");
				throw new FS2Exception(e.Message);
			}
			return result;
		}
		public static bool IsAbort { get; set; }
		public static List<FS2JournalItem> OnGetFilteredArchive(ArchiveFilter archiveFilter, bool isReport)
		{
			var result = new List<FS2JournalItem>();
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

				using (var sqlCeConnection = new SqlCeConnection(ConnectionString))
				{
					sqlCeConnection.ConnectionString = ConnectionString;
					var journalItems = new List<FS2JournalItem>();
					var sqlCeCommand = new SqlCeCommand(query, sqlCeConnection);
					sqlCeConnection.Open();
					var reader = sqlCeCommand.ExecuteReader();
					while (reader.Read())
					{
						if (IsAbort && !isReport)
							break;
						try
						{
							var journalItem = ReadOneJournalItem(reader);
							result.Add(journalItem);
							if (!isReport)
							{
								journalItems.Add(journalItem);
								if (journalItems.Count > 100)
								{
									if (ArchivePortionReady != null)
										ArchivePortionReady(journalItems.ToList());

									journalItems.Clear();
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
							ArchivePortionReady(journalItems.ToList());
					}

					sqlCeConnection.Close();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.GetFilteredArchive");
				throw new FS2Exception(e.Message);
			}
			return result;
		}

		static FS2JournalItem ReadOneJournalItem(SqlCeDataReader reader)
		{
			var journalItem = new FS2JournalItem();
			if (!reader.IsDBNull(reader.GetOrdinal("Description")))
				journalItem.Description = reader.GetString(reader.GetOrdinal("Description"));
			if (!reader.IsDBNull(reader.GetOrdinal("Detalization")))
				journalItem.Detalization = reader.GetString(reader.GetOrdinal("Detalization"));
			if (!reader.IsDBNull(reader.GetOrdinal("DeviceCategory")))
				journalItem.DeviceCategory = reader.GetInt32(reader.GetOrdinal("DeviceCategory"));
			if (!reader.IsDBNull(reader.GetOrdinal("DeviceName")))
				journalItem.DeviceName = reader.GetString(reader.GetOrdinal("DeviceName"));
			if (!reader.IsDBNull(reader.GetOrdinal("DeviceTime")))
				journalItem.DeviceTime = reader.GetDateTime(reader.GetOrdinal("DeviceTime"));
			if (!reader.IsDBNull(reader.GetOrdinal("PanelName")))
				journalItem.PanelName = reader.GetString(reader.GetOrdinal("PanelName"));
			if (!reader.IsDBNull(reader.GetOrdinal("StateType")))
				journalItem.StateType = (StateType)reader.GetInt32(reader.GetOrdinal("StateType"));
			if (!reader.IsDBNull(reader.GetOrdinal("SubsystemType")))
				journalItem.SubsystemType = (SubsystemType)reader.GetInt32(reader.GetOrdinal("SubsystemType"));
			if (!reader.IsDBNull(reader.GetOrdinal("SystemTime")))
				journalItem.SystemTime = reader.GetDateTime(reader.GetOrdinal("SystemTime"));
			if (!reader.IsDBNull(reader.GetOrdinal("UserName")))
				journalItem.UserName = reader.GetString(reader.GetOrdinal("UserName"));
			if (!reader.IsDBNull(reader.GetOrdinal("ZoneName")))
				journalItem.ZoneName = reader.GetString(reader.GetOrdinal("ZoneName"));
			if (!reader.IsDBNull(reader.GetOrdinal("Id")))
				journalItem.No = reader.GetInt32(reader.GetOrdinal("Id"));
			return journalItem;
		}

		public static event Action<List<FS2JournalItem>> ArchivePortionReady;

		public static List<JournalDescriptionItem> GetDistinctDescriptions()
		{
			var result = new List<JournalDescriptionItem>();
			try
			{
				string query = "SELECT DISTINCT TOP (1000) StateType, Description FROM Journal ORDER BY Description";
				using (var sqlCeConnection = new SqlCeConnection(ConnectionString))
				{
					sqlCeConnection.ConnectionString = ConnectionString;
					var sqlCeCommand = new SqlCeCommand(query, sqlCeConnection);
					sqlCeConnection.Open();
					var reader = sqlCeCommand.ExecuteReader();
					while (reader.Read())
					{
						var journalDescriptionItem = new JournalDescriptionItem();
						journalDescriptionItem.Description = reader.GetString(reader.GetOrdinal("Description"));
						journalDescriptionItem.StateType = (StateType)reader.GetInt32(reader.GetOrdinal("StateType"));
						result.Add(journalDescriptionItem);
					}
					sqlCeConnection.Close();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.GetDistinctDescriptions");
				throw new FS2Exception(e.Message);
			}
			return result;
		}

		public static DateTime GetArchiveStartDate()
		{
			var result = DateTime.Now;
			try
			{
				string query = "SELECT MIN(SystemTime) AS SystemTime FROM Journal";
				using (var sqlCeConnection = new SqlCeConnection(ConnectionString))
				{
					sqlCeConnection.ConnectionString = ConnectionString;
					var sqlCeCommand = new SqlCeCommand(query, sqlCeConnection);
					sqlCeConnection.Open();
					var reader = sqlCeCommand.ExecuteReader();
					if (reader.Read())
					{
						result = reader.GetDateTime(reader.GetOrdinal("SystemTime"));
					}
					sqlCeConnection.Close();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.GetArchiveStartDate");
				throw new FS2Exception(e.Message);
			}
			return result;
		}
	}
}