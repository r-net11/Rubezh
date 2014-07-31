using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using SKDDriver;

namespace FiresecService
{
	public static partial class DBHelper
	{
		public static object locker = new object();
		public static object databaseLocker = new object();
		public static bool IsAbort { get; set; }
		public static event Action<List<JournalItem>, Guid> ArchivePortionReady;
		static string ConnectionString = global::SKDDriver.Properties.Settings.Default.ConnectionString;

		public static void Add(JournalItem journalItem)
		{
			lock (databaseLocker)
			{
				SKDDatabaseService.JournalItemTranslator.Save(journalItem);
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
				return new OperationResult<List<JournalItem>> { Result = journalItems };
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.GetTopLast");
				return new OperationResult<List<JournalItem>>(e.Message);
			}
		}

		public static List<JournalItem> BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID, bool isReport)
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
							if (IsAbort && !isReport)
								break;
							try
							{
								var journalItem = ReadOneJournalItem(reader);
								result.Add(journalItem);
								if (!isReport)
								{
									journalItems.Add(journalItem);
									if (journalItems.Count >= archiveFilter.PageSize)
										PublishNewItemsPortion(journalItems, archivePortionUID);
								}
							}
							catch (Exception e)
							{
								Logger.Error(e, "SKD DatabaseHelper.OnGetFilteredArchive");
							}
						}
						if (!isReport)
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

		static void PublishNewItemsPortion(List<JournalItem> journalItems, Guid archivePortionUID)
		{
			if (ArchivePortionReady != null)
				ArchivePortionReady(journalItems.ToList(), archivePortionUID);
			UpdateNamesDescriptions(journalItems);
			journalItems.Clear();
		}

		static string BuildQuery(JournalFilter journalFilter)
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
					query += "Name = '" + journalEventNameType + "'";
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

		static string BuildQuery(ArchiveFilter archiveFilter)
		{
			string dateTimeTypeString;
			if (archiveFilter.UseDeviceDateTime)
				dateTimeTypeString = "DeviceDate";
			else
				dateTimeTypeString = "SystemDate";

			var query =
				"SELECT * FROM Journal WHERE " +
				"\n " + dateTimeTypeString + " > '" + archiveFilter.StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
				"\n AND " + dateTimeTypeString + " < '" + archiveFilter.EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";

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

		static JournalItem ReadOneJournalItem(SqlDataReader reader)
		{
			var journalItem = new JournalItem();

			if (!reader.IsDBNull(reader.GetOrdinal("DescriptionText")))
				journalItem.DescriptionText = reader.GetString(reader.GetOrdinal("DescriptionText"));

			if (!reader.IsDBNull(reader.GetOrdinal("NameText")))
				journalItem.NameText = reader.GetString(reader.GetOrdinal("NameText"));

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectName")))
				journalItem.ObjectName = reader.GetString(reader.GetOrdinal("ObjectName"));

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectType")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("ObjectType"));
				if (Enum.IsDefined(typeof(JournalObjectType), intValue))
					journalItem.JournalObjectType = (JournalObjectType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectUID")))
				journalItem.ObjectUID = reader.GetGuid(reader.GetOrdinal("ObjectUID"));

			if (!reader.IsDBNull(reader.GetOrdinal("State")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("State"));
				if (Enum.IsDefined(typeof(XStateClass), intValue))
					journalItem.StateClass = (XStateClass)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("Subsystem")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("Subsystem"));
				if (Enum.IsDefined(typeof(JournalSubsystemType), intValue))
					journalItem.JournalSubsystemType = (JournalSubsystemType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("UID")))
				journalItem.UID = reader.GetGuid(reader.GetOrdinal("UID"));

			if (!reader.IsDBNull(reader.GetOrdinal("UserName")))
				journalItem.UserName = reader.GetString(reader.GetOrdinal("UserName"));

			if (!reader.IsDBNull(reader.GetOrdinal("CardNo")))
				journalItem.CardNo = (int)reader.GetValue(reader.GetOrdinal("CardNo"));
			
			if (!reader.IsDBNull(reader.GetOrdinal("SystemDate")))
				journalItem.SystemDateTime = reader.GetDateTime(reader.GetOrdinal("SystemDate"));

			if (!reader.IsDBNull(reader.GetOrdinal("DeviceDate")))
				journalItem.DeviceDateTime = reader.GetDateTime(reader.GetOrdinal("DeviceDate"));

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

			return journalItem;
		}
	}
}