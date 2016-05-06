using Common;
using StrazhAPI;
using StrazhAPI.Journal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace FiresecService
{
	public static partial class DBHelper
	{
		public static object locker = new object();
		public static object databaseLocker = new object();

		public static bool IsAbort { get; set; }

		public static event Action<List<JournalItem>, Guid> ArchivePortionReady;

		public static string ConnectionString { get; set; }

		public static void Add(JournalItem journalItem)
		{
			journalItem.UID = Guid.NewGuid();

			try
			{
				lock (locker)
				{
					using (var dataContext = new SqlConnection(ConnectionString))
					{
						dataContext.ConnectionString = ConnectionString;
						dataContext.Open();

						var sqCommand = new SqlCommand();
						sqCommand.Connection = dataContext;

						string detalization = JournalDetalisationItem.ListToString(journalItem.JournalDetalisationItems);

						sqCommand.CommandText = @"Insert Into Journal" +
							"(UID,SystemDate,DeviceDate,Subsystem,Name,Description,NameText,DescriptionText,ObjectType,ObjectName,ObjectUID,Detalisation,UserName,EmployeeUID,VideoUID,CameraUID,ErrorCode,ControllerUID) Values" +
							"(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18)";
						sqCommand.Parameters.AddWithValue("@p1", (object)journalItem.UID);
						sqCommand.Parameters.AddWithValue("@p2", (object)journalItem.SystemDateTime);
						sqCommand.Parameters.AddWithValue("@p3", (object)journalItem.DeviceDateTime ?? DBNull.Value);
						sqCommand.Parameters.AddWithValue("@p4", (object)((int)journalItem.JournalSubsystemType));
						sqCommand.Parameters.AddWithValue("@p5", (object)((int)journalItem.JournalEventNameType));
						sqCommand.Parameters.AddWithValue("@p6", (object)((int)journalItem.JournalEventDescriptionType));
						sqCommand.Parameters.AddWithValue("@p7", DBNull.Value);
						sqCommand.Parameters.AddWithValue("@p8", (object)journalItem.DescriptionText ?? DBNull.Value);
						sqCommand.Parameters.AddWithValue("@p9", (object)((int)journalItem.JournalObjectType));
						sqCommand.Parameters.AddWithValue("@p10", (object)journalItem.ObjectName ?? DBNull.Value);
						sqCommand.Parameters.AddWithValue("@p11", (object)journalItem.ObjectUID);
						sqCommand.Parameters.AddWithValue("@p12", String.IsNullOrEmpty(detalization) ? DBNull.Value : (object)detalization);
						sqCommand.Parameters.AddWithValue("@p13", (object)journalItem.UserName ?? DBNull.Value);
						sqCommand.Parameters.AddWithValue("@p14", journalItem.EmployeeUID == Guid.Empty ? DBNull.Value : (object)journalItem.EmployeeUID);
						sqCommand.Parameters.AddWithValue("@p15", journalItem.VideoUID == Guid.Empty ? DBNull.Value : (object)journalItem.VideoUID);
						sqCommand.Parameters.AddWithValue("@p16", journalItem.CameraUID == Guid.Empty ? DBNull.Value : (object)journalItem.CameraUID);
						sqCommand.Parameters.AddWithValue("@p17", (object)((int)journalItem.ErrorCode));
						sqCommand.Parameters.AddWithValue("@p18", journalItem.ControllerUID == Guid.Empty ? DBNull.Value : (object)journalItem.ControllerUID);
						sqCommand.ExecuteNonQuery();

						dataContext.Close();
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.GetTopLast");
			}
		}

		public static OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			try
			{
				var journalItems = new List<JournalItem>();
				lock (locker)
				{
					using (var dataContext = new SqlConnection(ConnectionString))
					{
						var query = BuildQuery(filter);
						var sqlCommand = new SqlCommand(query, dataContext);
						dataContext.Open();
						var reader = sqlCommand.ExecuteReader();
						while (reader.Read())
						{
							var journalItem = ReadOneJournalItem(reader);
							journalItems.Add(journalItem);
						}
					}
				}
				journalItems.Reverse();
				return new OperationResult<List<JournalItem>>(journalItems);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.GetTopLast");
				return OperationResult<List<JournalItem>>.FromError(e.Message);
			}
		}

		public static List<JournalItem> BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			var journalItems = new List<JournalItem>();
			var result = new List<JournalItem>();

			try
			{
				lock (locker)
				{
					using (var dataContext = new SqlConnection(ConnectionString))
					{
						var query = BuildQuery(archiveFilter);
						var sqlCommand = new SqlCommand(query, dataContext);
						dataContext.Open();
						var reader = sqlCommand.ExecuteReader();
						while (reader.Read())
						{
							if (IsAbort)
								break;
							try
							{
								var journalItem = ReadOneJournalItem(reader);
								result.Add(journalItem);
								journalItems.Add(journalItem);
								if (journalItems.Count >= archiveFilter.PageSize)
									PublishNewItemsPortion(journalItems, archivePortionUID);
							}
							catch (Exception e)
							{
								Logger.Error(e, "SKD DatabaseHelper.OnGetFilteredArchive");
							}
						}
						PublishNewItemsPortion(journalItems, archivePortionUID);
					}
				}
			}
			catch (ThreadAbortException) { }
			catch (Exception e)
			{
				Logger.Error(e, "SKDDBHelper.Select");
			}
			return result;
		}

		private static void PublishNewItemsPortion(List<JournalItem> journalItems, Guid archivePortionUID)
		{
			if (ArchivePortionReady != null)
				ArchivePortionReady(journalItems.ToList(), archivePortionUID);
			journalItems.Clear();
		}

		private static string BuildQuery(JournalFilter journalFilter)
		{
			var query = "SELECT TOP (" + journalFilter.LastItemsCount.ToString() + ") * FROM Journal ";

			bool hasWhere = false;
			if (journalFilter.JournalEventNameTypes.Count > 0)
			{
				if (!hasWhere)
				{
					query += "\n Where (";
					hasWhere = true;
				}
				else
				{
					query += "\n AND (";
				}
				int index = 0;
				foreach (var journalEventNameType in journalFilter.JournalEventNameTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "Name = '" + (int)journalEventNameType + "'";
				}
				query += ")";
			}

			if (journalFilter.JournalEventDescriptionTypes.Count > 0)
			{
				if (!hasWhere)
				{
					query += "\n Where (";
					hasWhere = true;
				}
				else
				{
					query += "\n AND (";
				}
				int index = 0;
				foreach (var journalEventDescriptionType in journalFilter.JournalEventDescriptionTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "Description = '" + (int)journalEventDescriptionType + "'";
				}
				query += ")";
			}

			if (journalFilter.JournalSubsystemTypes.Count > 0)
			{
				if (!hasWhere)
				{
					query += "\n Where (";
					hasWhere = true;
				}
				else
				{
					query += "\n AND (";
				}
				int index = 0;
				foreach (var journalSubsystemType in journalFilter.JournalSubsystemTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "Subsystem = '" + (int)journalSubsystemType + "'";
				}
				query += ")";
			}

			if (journalFilter.JournalObjectTypes.Count > 0)
			{
				if (!hasWhere)
				{
					query += "\n Where (";
					hasWhere = true;
				}
				else
				{
					query += "\n AND (";
				}
				int index = 0;
				foreach (var journalObjectType in journalFilter.JournalObjectTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "ObjectType = '" + (int)journalObjectType + "'";
				}
				query += ")";
			}

			if (journalFilter.ObjectUIDs.Count > 0)
			{
				if (!hasWhere)
				{
					query += "\n Where (";
					hasWhere = true;
				}
				else
				{
					query += "\n AND (";
				}
				int index = 0;
				foreach (var objectUID in journalFilter.ObjectUIDs)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "ObjectUID = '" + objectUID + "'";
				}
				query += ")";
			}

			query += "\n ORDER BY SystemDate DESC";
			return query;
		}

		private static string BuildQuery(ArchiveFilter archiveFilter)
		{
			string dateTimeTypeString;
			if (archiveFilter.UseDeviceDateTime)
				dateTimeTypeString = "DeviceDate";
			else
				dateTimeTypeString = "SystemDate";

			var query =
				"SELECT * FROM Journal WHERE " +
				"\n " + dateTimeTypeString + " > '" + archiveFilter.StartDate.ToString("yyyy-MM-ddTHH:mm:ss") + "'" +
				"\n AND " + dateTimeTypeString + " < '" + archiveFilter.EndDate.ToString("yyyy-MM-ddTHH:mm:ss") + "'";

			if (archiveFilter.JournalEventNameTypes.Count > 0)
			{
				query += "\n and (";
				int index = 0;
				foreach (var journalEventNameType in archiveFilter.JournalEventNameTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "Name = '" + (int)journalEventNameType + "'";
				}
				query += ")";
			}

			if (archiveFilter.JournalSubsystemTypes.Count > 0)
			{
				query += "\n AND (";
				int index = 0;
				foreach (var journalSubsystemType in archiveFilter.JournalSubsystemTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "Subsystem = '" + (int)journalSubsystemType + "'";
				}
				query += ")";
			}

			if (archiveFilter.JournalObjectTypes.Count > 0)
			{
				query += "\n AND (";
				int index = 0;
				foreach (var journalObjectType in archiveFilter.JournalObjectTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "ObjectType = '" + (int)journalObjectType + "'";
				}
				query += ")";
			}

			if (archiveFilter.ObjectUIDs.Count > 0)
			{
				query += "\n AND (";
				int index = 0;
				foreach (var objectUID in archiveFilter.ObjectUIDs)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "ObjectUID = '" + objectUID + "'";
				}
				query += ")";
			}

			query += "\n ORDER BY " + dateTimeTypeString + " DESC";
			return query;
		}

		private static JournalItem ReadOneJournalItem(SqlDataReader reader)
		{
			var journalItem = new JournalItem();

			if (!reader.IsDBNull(reader.GetOrdinal("UID")))
				journalItem.UID = reader.GetGuid(reader.GetOrdinal("UID"));

			if (!reader.IsDBNull(reader.GetOrdinal("SystemDate")))
				journalItem.SystemDateTime = reader.GetDateTime(reader.GetOrdinal("SystemDate"));

			if (!reader.IsDBNull(reader.GetOrdinal("DeviceDate")))
				journalItem.DeviceDateTime = reader.GetDateTime(reader.GetOrdinal("DeviceDate"));

			if (!reader.IsDBNull(reader.GetOrdinal("Subsystem")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("Subsystem"));
				if (Enum.IsDefined(typeof(JournalSubsystemType), intValue))
					journalItem.JournalSubsystemType = (JournalSubsystemType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("Name")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("Name"));
				if (Enum.IsDefined(typeof(JournalEventNameType), intValue))
					journalItem.JournalEventNameType = (JournalEventNameType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("Description")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("Description"));
				if (Enum.IsDefined(typeof(JournalEventDescriptionType), intValue))
					journalItem.JournalEventDescriptionType = (JournalEventDescriptionType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("DescriptionText")))
				journalItem.DescriptionText = reader.GetString(reader.GetOrdinal("DescriptionText"));

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectType")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("ObjectType"));
				if (Enum.IsDefined(typeof(JournalObjectType), intValue))
					journalItem.JournalObjectType = (JournalObjectType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectName")))
				journalItem.ObjectName = reader.GetString(reader.GetOrdinal("ObjectName"));

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectUID")))
				journalItem.ObjectUID = reader.GetGuid(reader.GetOrdinal("ObjectUID"));

			if (!reader.IsDBNull(reader.GetOrdinal("Detalisation")))
			{
				var detalisationString = reader.GetString(reader.GetOrdinal("Detalisation"));
				journalItem.JournalDetalisationItems = JournalDetalisationItem.StringToList(detalisationString);
			}

			if (!reader.IsDBNull(reader.GetOrdinal("UserName")))
				journalItem.UserName = reader.GetString(reader.GetOrdinal("UserName"));

			if (!reader.IsDBNull(reader.GetOrdinal("EmployeeUID")))
				journalItem.EmployeeUID = reader.GetGuid(reader.GetOrdinal("EmployeeUID"));

			if (!reader.IsDBNull(reader.GetOrdinal("VideoUID")))
				journalItem.VideoUID = reader.GetGuid(reader.GetOrdinal("VideoUID"));

			if (!reader.IsDBNull(reader.GetOrdinal("CameraUID")))
				journalItem.CameraUID = reader.GetGuid(reader.GetOrdinal("CameraUID"));

			if (!reader.IsDBNull(reader.GetOrdinal("ErrorCode")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("ErrorCode"));
				if (Enum.IsDefined(typeof(JournalErrorCode), intValue))
					journalItem.ErrorCode = (JournalErrorCode)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("ControllerUID")))
				journalItem.ControllerUID = reader.GetGuid(reader.GetOrdinal("ControllerUID"));

			return journalItem;
		}
	}
}