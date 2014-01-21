using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;
using Infrastructure.Common;

namespace ServerFS2.Journal
{
	public static partial class ServerFS2Database
	{
		public static string ConnectionString { get; set; }

		static ServerFS2Database()
		{
			ConnectionString = @"Data Source=" + AppDataFolderHelper.GetDBFile("FSDB.sdf") + ";Password=adm;Max Database Size=4000";
		}

		public static int GetLastId(Guid deviceUID)
		{
			var lastIndex = 0;
			try
			{
				using (var sqlCeConnection = new SqlCeConnection(ConnectionString))
				{
					sqlCeConnection.ConnectionString = ConnectionString;
					sqlCeConnection.Open();
					var sqlCeCommand = new SqlCeCommand();
					sqlCeCommand.Connection = sqlCeConnection;
					sqlCeCommand.CommandText = @"SELECT LastID FROM LastIndexes WHERE DeviceUID = @p1";
					sqlCeCommand.Parameters.AddWithValue("@p1", deviceUID);
					lastIndex = (int)sqlCeCommand.ExecuteScalar();
					sqlCeConnection.Close();
				}
			}
			catch { }
			return lastIndex;
		}

		public static int GetLastSecId(Guid deviceUID)
		{
			var lastIndex = 0;
			try
			{
				using (var sqlCeConnection = new SqlCeConnection(ConnectionString))
				{
					sqlCeConnection.ConnectionString = ConnectionString;
					sqlCeConnection.Open();
					var sqlCeCommand = new SqlCeCommand();
					sqlCeCommand.Connection = sqlCeConnection;
					sqlCeCommand.CommandText = @"SELECT LastSecID FROM LastSecIndexes WHERE DeviceUID = @p1";
					sqlCeCommand.Parameters.AddWithValue("@p1", deviceUID);
					lastIndex = (int)sqlCeCommand.ExecuteScalar();
					sqlCeConnection.Close();
				}
			}
			catch { }
			return lastIndex;
		}

		public static void SetLastId(Guid deviceUID, int lastIndex)
		{
			try
			{
				using (var sqlCeConnection = new SqlCeConnection(ConnectionString))
				{
					sqlCeConnection.ConnectionString = ConnectionString;
					sqlCeConnection.Open();
					var sqlCeCommand = new SqlCeCommand();
					sqlCeCommand.Connection = sqlCeConnection;
					sqlCeCommand.CommandText = @"Update LastIndexes " +
						"set LastID = @p2 " +
						"where DeviceUID = @p1";
					sqlCeCommand.Parameters.AddWithValue("@p1", deviceUID);
					sqlCeCommand.Parameters.AddWithValue("@p2", lastIndex);
					sqlCeCommand.ExecuteNonQuery();
					sqlCeConnection.Close();
				}
			}
			catch { }
		}

		public static void SetLastSecId(Guid deviceUID, int lastIndex)
		{
			try
			{
				using (var sqlCeConnection = new SqlCeConnection(ConnectionString))
				{
					sqlCeConnection.ConnectionString = ConnectionString;
					sqlCeConnection.Open();
					var sqlCeCommand = new SqlCeCommand();
					sqlCeCommand.Connection = sqlCeConnection;
					sqlCeCommand.CommandText = @"Update LastSecIndexes " +
						"set LastSecID = @p2 " +
						"where DeviceUID = @p1";
					sqlCeCommand.Parameters.AddWithValue("@p1", deviceUID);
					sqlCeCommand.Parameters.AddWithValue("@p2", lastIndex);
					sqlCeCommand.ExecuteNonQuery();
					sqlCeConnection.Close();
				}
			}
			catch { }
		}

		public static void SetNewLastId(Guid deviceUID, int lastIndex)
		{
			try
			{
				using (var sqlCeConnection = new SqlCeConnection(ConnectionString))
				{
					sqlCeConnection.ConnectionString = ConnectionString;
					sqlCeConnection.Open();
					var sqlCeCommand = new SqlCeCommand();
					sqlCeCommand.Connection = sqlCeConnection;
					sqlCeCommand.CommandText = @"Insert Into LastIndexes" +
						"(DeviceUID,LastID) Values" +
						"(@p1,@p2)";
					sqlCeCommand.Parameters.AddWithValue("@p1", deviceUID);
					sqlCeCommand.Parameters.AddWithValue("@p2", lastIndex);
					sqlCeCommand.ExecuteNonQuery();
					sqlCeConnection.Close();
				}
			}
			catch { }
		}

		public static void AddJournalItems(List<FS2JournalItem> journalItems)
		{
			using (var dataContext = new SqlCeConnection(ConnectionString))
			{
				dataContext.ConnectionString = ConnectionString;
				dataContext.Open();
				foreach (var journalItem in journalItems)
				{
					AddItemSqlCommand(dataContext, journalItem);
				}
				dataContext.Close();
			}
		}

		private static void AddItemSqlCommand(SqlCeConnection sqlCeConnection, FS2JournalItem journalItem)
		{
			try
			{
				var sqlCeCommand = new SqlCeCommand();
				sqlCeCommand.Connection = sqlCeConnection;
				sqlCeCommand.CommandText = @"Insert Into Journal" +
					"(DeviceTime, SystemTime, Description, Detalization, DeviceName, PanelName, DeviceUID, PanelUID, ZoneName, DeviceCategory, StateType, SubsystemType, UserName) Values" +
					"(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13)";

				sqlCeCommand.Parameters.AddWithValue("@p1", (object)journalItem.DeviceTime ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p2", (object)journalItem.SystemTime ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p3", (object)journalItem.Description ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p4", (object)journalItem.Detalization ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p5", (object)journalItem.DeviceName ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p6", (object)journalItem.PanelName ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p7", (object)journalItem.DeviceUID ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p8", (object)journalItem.PanelUID ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p9", (object)journalItem.ZoneName ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p10", (object)journalItem.DeviceCategory ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p11", (object)journalItem.StateType ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p12", (object)journalItem.SubsystemType ?? DBNull.Value);
				sqlCeCommand.Parameters.AddWithValue("@p13", (object)journalItem.UserName ?? DBNull.Value);
				sqlCeCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				Logger.Error(e, "DatabaseHelper.AddItemSqlCommand");
			}
		}

		public static void AddJournalItem(FS2JournalItem journalItem)
		{
			using (var dataContext = new SqlCeConnection(ConnectionString))
			{
				dataContext.ConnectionString = ConnectionString;
				dataContext.Open();
				AddItemSqlCommand(dataContext, journalItem);
				dataContext.Close();
			}
		}

		public static List<FS2JournalItem> GetJournalItems(Guid deviceUID)
		{
			var result = new List<FS2JournalItem>();

			using (var sqlCeConnection = new SqlCeConnection(ConnectionString))
			{
				sqlCeConnection.ConnectionString = ConnectionString;
				sqlCeConnection.Open();
				var sqlCeCommand = new SqlCeCommand();
				sqlCeCommand.Connection = sqlCeConnection;
				sqlCeCommand.CommandText = @"SELECT Description, Detalization, DeviceCategory, DeviceUID, DeviceName, DeviceTime, PanelUID, PanelName, StateType, SubsystemType, SystemTime, UserName, ZoneName FROM Journal";
				var reader = sqlCeCommand.ExecuteReader();
				while (reader.Read())
				{
					try
					{
						var fsJournalItem = new FS2JournalItem();
						fsJournalItem.Description = TryGetNullableString(reader, 0);// reader.GetString(0);
						fsJournalItem.Detalization = TryGetNullableString(reader, 1);
						fsJournalItem.DeviceCategory = reader.GetInt32(2);
						fsJournalItem.DeviceUID = reader.GetGuid(3);
						if (!reader.IsDBNull(4))
							fsJournalItem.DeviceName = TryGetNullableString(reader, 4);
						fsJournalItem.DeviceTime = reader.GetDateTime(5);
						fsJournalItem.PanelUID = reader.GetGuid(6);
						fsJournalItem.PanelName = TryGetNullableString(reader, 7);
						fsJournalItem.StateType = (StateType)reader.GetInt32(8);
						fsJournalItem.SubsystemType = (SubsystemType)reader.GetInt32(9);
						fsJournalItem.SystemTime = reader.GetDateTime(10);
						fsJournalItem.UserName = TryGetNullableString(reader, 11);
						fsJournalItem.ZoneName = TryGetNullableString(reader, 12);
						result.Add(fsJournalItem);
					}
					catch { ;}
				}
				sqlCeConnection.Close();
			}
			return result;
		}

		static string TryGetNullableString(SqlCeDataReader reader, int index)
		{
			if (!reader.IsDBNull(index))
				return reader.GetString(index);
			else
				return "";
		}
	}
}