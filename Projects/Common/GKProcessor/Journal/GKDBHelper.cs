using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using Common;
using Infrastructure.Common;
using XFiresecAPI;
using System.Diagnostics;

namespace GKProcessor
{
	public static partial class GKDBHelper
	{
		public static bool CanAdd = true;
		public static string ConnectionString = @"Data Source=" + AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf") + ";Persist Security Info=True;Max Database Size=4000";
		public static object locker = new object();

		public static void Add(JournalItem journalItem)
		{
			try
			{
				AddMany(new List<JournalItem>() { journalItem });
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
				if (journalItems.Count > 0)
				{
					lock (locker)
					{
						InsertJournalRecordToDb(journalItems);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDBHelper.AddMany");
			}
		}

		public static JournalItem AddMessage(string message, string userName)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalItemType = JournalItemType.System,
				StateClass = XStateClass.Norm,
				Name = message,
				ObjectStateClass = XStateClass.Norm,
				UserName = userName,
				SubsystemType = XSubsystemType.System
			};
			Add(journalItem);
			return journalItem;
		}

		static List<JournalItem> UpdateItemLengths(List<JournalItem> journalItems)
		{
			foreach (var item in journalItems)
			{
				if (item.Description != null && item.Description.Length > 100)
					item.Description = item.Description.Substring(0, 100);
				if (item.Name != null && item.Name.Length > 100)
					item.Name = item.Name.Substring(0, 100);
				if (item.GKIpAddress != null && item.GKIpAddress.Length > 20)
					item.GKIpAddress = item.GKIpAddress.Substring(0, 20);
				if (item.UserName != null && item.UserName.Length > 50)
					item.UserName = item.UserName.Substring(0, 50);
				if (item.ObjectName != null && item.ObjectName.Length > 100)
					item.ObjectName = item.ObjectName.Substring(0, 100);
			}			
			return journalItems;
		}

		public static void InsertJournalRecordToDb(List<JournalItem> journalItems)
		{
			journalItems = UpdateItemLengths(journalItems);
			if (CanAdd && File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
			{
				using (var dataContext = new SqlCeConnection(ConnectionString))
				{
					dataContext.ConnectionString = ConnectionString;
					dataContext.Open();
					foreach (var journalItem in journalItems)
					{
						var sqlCeCommand = new SqlCeCommand();
						sqlCeCommand.Connection = dataContext;
						sqlCeCommand.CommandText = @"Insert Into Journal" +
							"(JournalItemType,ObjectUID,Name,Description,ObjectState,GKObjectNo,GKIpAddress,GKJournalRecordNo,StateClass,UserName,SystemDateTime,DeviceDateTime,Subsystem,ObjectStateClass,ObjectName) Values" +
							"(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15)";
						sqlCeCommand.Parameters.AddWithValue("@p1", (object)journalItem.JournalItemType ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p2", (object)journalItem.ObjectUID ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p3", (object)journalItem.Name ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p4", (object)journalItem.Description ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p5", (object)journalItem.ObjectState ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p6", (object)journalItem.GKObjectNo ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p7", (object)journalItem.GKIpAddress ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p8", (object)journalItem.GKJournalRecordNo ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p9", (object)journalItem.StateClass ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p10", (object)journalItem.UserName ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p11", (object)journalItem.SystemDateTime ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p12", (object)journalItem.DeviceDateTime ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p13", (object)journalItem.SubsystemType ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p14", (object)journalItem.ObjectStateClass ?? DBNull.Value);
						sqlCeCommand.Parameters.AddWithValue("@p15", (object)journalItem.ObjectName ?? DBNull.Value);
						sqlCeCommand.ExecuteNonQuery();
					}
					dataContext.Close();
				}
			}
		}

		public static void AddColumn(string columnName, string columnType, string tableName)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			var connection = new SqlCeConnection(ConnectionString);
			if (!IsColumnExists(columnName, tableName))
			{
				var sqlCeCommand = new SqlCeCommand("alter table "+ tableName +" add column " + columnName.ToString() + " " + columnType.ToString(), connection);
				connection.Open();
				sqlCeCommand.ExecuteNonQuery();
			}
			connection.Close();
			connection.Dispose();
		}

		public static void AlterColumnType(string columnName, string columnType, string tableName)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			var connection = new SqlCeConnection(ConnectionString);
			if (IsColumnExists(columnName, tableName))
			{
				var sqlCeCommand = new SqlCeCommand("alter table " + tableName + " alter column " + columnName.ToString() + " " + columnType.ToString(), connection);
				connection.Open();
				sqlCeCommand.ExecuteNonQuery();
			}
			connection.Close();
			connection.Dispose();
		}

		public static void DropColumn(string columnName, string tableName)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			var connection = new SqlCeConnection(ConnectionString);
			if (IsColumnExists(columnName, tableName))
			{
				var sqlCeCommand = new SqlCeCommand("alter table " + tableName + " drop column " + columnName.ToString(), connection);
				connection.Open();
				sqlCeCommand.ExecuteNonQuery();
			}
			connection.Close();
			connection.Dispose();
		}

		public static void ExecuteNonQuery(string commandString)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand(commandString, connection);
			connection.Open();
			sqlCeCommand.ExecuteNonQuery();
			connection.Close();
			connection.Dispose();
		}

		static bool IsColumnExists(string columnName, string tableName)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return false;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand(
				"select column_name from INFORMATION_SCHEMA.columns where column_name = '" + columnName + "' and table_name = '" + tableName + "'",
				connection);
			connection.Open();
			var reader = sqlCeCommand.ExecuteReader();
			bool result = reader.Read();
			connection.Close();
			connection.Dispose();
			return result;
		}

		static int GetColumnLength(string columnName, string tableName)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return 0;
			int result = 0;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand(
				"select character_maximum_length from INFORMATION_SCHEMA.columns where column_name = '" + columnName + "' and table_name = '" + tableName + "'",
				connection);
			connection.Open();
			var reader = sqlCeCommand.ExecuteReader(CommandBehavior.CloseConnection);
			if (reader.Read())
			{
				result = reader.GetInt32(0);
			}
			else
			{
				result = 0;
			}
			connection.Close();
			connection.Dispose();
			return result;
		}

		public static List<JournalItem> Select(XArchiveFilter archiveFilter)
		{
			var journalItems = new List<JournalItem>();
			string dateTimeTypeString;
			if (archiveFilter.UseDeviceDateTime)
				dateTimeTypeString = "DeviceDateTime";
			else
				dateTimeTypeString = "SystemDateTime";
			try
			{
				lock (locker)
				{
					if (File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
					{
						using (var dataContext = new SqlCeConnection(ConnectionString))
						{
							var query =
							"SELECT * FROM Journal WHERE " +
							"\n " + dateTimeTypeString + " > '" + archiveFilter.StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
							"\n AND " + dateTimeTypeString + " < '" + archiveFilter.EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";

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

							if (archiveFilter.JournalDescriptionState.Count > 0)
							{
								query += "\n and (";
								int index = 0;
								foreach (var eventName in archiveFilter.JournalDescriptionState)
								{
									if (index > 0)
										query += "\n OR ";
									index++;
									query += "Name = '" + eventName.Name + "'";
								}
								query += ")";
							}

							if (archiveFilter.Descriptions.Count > 0)
							{
								query += "\n AND (";
								int index = 0;
								foreach (var description in archiveFilter.Descriptions)
								{
									if (index > 0)
										query += "\n OR ";
									index++;
									query += "Description = '" + description + "'";
								}
								query += ")";
							}

							if (archiveFilter.SubsystemTypes.Count > 0)
							{
								query += "\n AND (";
								int index = 0;
								foreach (var subsystem in archiveFilter.SubsystemTypes)
								{
									if (index > 0)
										query += "\n OR ";
									index++;
									if(subsystem == XSubsystemType.System)
										query += "Subsystem = 0";
									else
										query += "Subsystem = 1";
								}
								query += ")";
							}

							var objectUIDs = new List<Guid>();
							objectUIDs.AddRange(archiveFilter.DeviceUIDs);
							objectUIDs.AddRange(archiveFilter.ZoneUIDs);
							objectUIDs.AddRange(archiveFilter.DirectionUIDs);
							objectUIDs.AddRange(archiveFilter.DelayUIDs);
							objectUIDs.AddRange(archiveFilter.PumpStationUIDs);
							if (objectUIDs.Count > 0)
							{
								int index = 0;
								query += "\n AND (";
								foreach (var objectUID in objectUIDs)
								{
									if (index > 0)
										query += "\n OR ";
									index++;
									query += "ObjectUID = '" + objectUID + "'";
								}
								query += ")";
							}

							query += "\n ORDER BY " + dateTimeTypeString + " DESC";

							var sqlCeCommand = new SqlCeCommand(query, dataContext);
							dataContext.Open();
							var reader = sqlCeCommand.ExecuteReader();
							while (reader.Read())
							{
								var journalItem = ReadOneJournalItem(reader);
								journalItems.Add(journalItem);
							}
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
					if (File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
					{
						using (var dataContext = new SqlCeConnection(ConnectionString))
						{
							var query = "SELECT MAX(GKJournalRecordNo) FROM Journal WHERE GKIpAddress = '" + gkIPAddress + "'";
							var sqlCeCommand = new SqlCeCommand(query, dataContext);
							dataContext.Open();
							var reader = sqlCeCommand.ExecuteReader();
							int result = -1;
							if (reader.Read())
							{
								if (!reader.IsDBNull(0))
								{
									result = reader.GetInt32(0);
								}
							}
							dataContext.Close();
							return result;
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDBHelper.GetLastGKID");
			}
			return -1;
		}

		public static List<string> GetGKIPAddresses()
		{
			try
			{
				var result = new List<string>();
				lock (locker)
				{
					if (File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
					{
						using (var dataContext = new SqlCeConnection(ConnectionString))
						{
							string query = "SELECT DISTINCT GKIpAddress FROM Journal";
							var sqlCeCommand = new SqlCeCommand(query, dataContext);
							dataContext.Open();
							var reader = sqlCeCommand.ExecuteReader();
							while (reader.Read())
							{
								if (!reader.IsDBNull(0))
								{
									var gkIpAddress = reader.GetString(0);
									result.Add(gkIpAddress);
								}
							}
							return result;
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
						using (var dataContext = new SqlCeConnection(ConnectionString))
						{
							var query = "SELECT TOP (" + count.ToString() + ") * FROM Journal ORDER BY SystemDateTime DESC";
							var sqlCeCommand = new SqlCeCommand(query, dataContext);
							dataContext.Open();
							var reader = sqlCeCommand.ExecuteReader();
							while (reader.Read())
							{
								var journalItem = ReadOneJournalItem(reader);
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

		static JournalItem ReadOneJournalItem(SqlCeDataReader reader)
		{
			var journalItem = new JournalItem();
			if (!reader.IsDBNull(reader.GetOrdinal("JournalItemType")))
				journalItem.JournalItemType = (JournalItemType)reader.GetByte(reader.GetOrdinal("JournalItemType"));
			
			if (!reader.IsDBNull(reader.GetOrdinal("SystemDateTime")))
				journalItem.SystemDateTime = reader.GetDateTime(reader.GetOrdinal("SystemDateTime"));

			if (!reader.IsDBNull(reader.GetOrdinal("DeviceDateTime")))
				journalItem.DeviceDateTime = reader.GetDateTime(reader.GetOrdinal("DeviceDateTime"));

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectUID")))
				journalItem.ObjectUID = reader.GetGuid(reader.GetOrdinal("ObjectUID"));

			if (!reader.IsDBNull(reader.GetOrdinal("Name")))
				journalItem.Name = reader.GetString(reader.GetOrdinal("Name"));

			if (!reader.IsDBNull(reader.GetOrdinal("Description")))
				journalItem.Description = reader.GetString(reader.GetOrdinal("Description"));

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectState")))
				journalItem.ObjectState = reader.GetInt32(reader.GetOrdinal("ObjectState"));

			if (!reader.IsDBNull(reader.GetOrdinal("GKObjectNo")))
				journalItem.GKObjectNo = (ushort)reader.GetInt16(reader.GetOrdinal("GKObjectNo"));

			if (!reader.IsDBNull(reader.GetOrdinal("GKIpAddress")))
				journalItem.GKIpAddress = reader.GetString(reader.GetOrdinal("GKIpAddress"));

			if (!reader.IsDBNull(reader.GetOrdinal("GKJournalRecordNo")))
				journalItem.GKJournalRecordNo = reader.GetInt32(reader.GetOrdinal("GKJournalRecordNo"));

			if (!reader.IsDBNull(reader.GetOrdinal("StateClass")))
				journalItem.StateClass = (XStateClass)reader.GetByte(reader.GetOrdinal("StateClass"));

			if (!reader.IsDBNull(reader.GetOrdinal("UserName")))
				journalItem.UserName = reader.GetString(reader.GetOrdinal("UserName"));

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectStateClass")))
				journalItem.ObjectStateClass = (XStateClass)reader.GetByte(reader.GetOrdinal("ObjectStateClass"));

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectName")))
				journalItem.ObjectName = reader.GetString(reader.GetOrdinal("ObjectName"));

			if (!reader.IsDBNull(reader.GetOrdinal("Subsystem")))
				journalItem.SubsystemType = (XSubsystemType)reader.GetByte(reader.GetOrdinal("Subsystem"));
			return journalItem;
		}
	}
}