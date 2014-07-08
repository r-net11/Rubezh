using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI.Events;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using SKDDriver;
using JournalItem = FiresecAPI.SKD.JournalItem;

namespace FiresecService
{
	public static class DBHelper
	{
		public static object locker = new object();
		public static object databaseLocker = new object();
		public static bool IsAbort { get; set; }
		public static event Action<List<JournalItem>, Guid> ArchivePortionReady;

		public static void Add(JournalItem journalItem)
		{
			lock (databaseLocker)
			{
				SKDDatabaseService.JournalItemTranslator.Save(journalItem);
			}
		}

		public static void AddMany(List<JournalItem> journalItems)
		{
			lock (databaseLocker)
			{
				SKDDatabaseService.JournalItemTranslator.Save(journalItems);
			}
		}

		public static JournalItem AddMessage(GlobalEventNameEnum globalEventNameEnum, string userName)
		{
			var result = new JournalItem();
			result.Name = globalEventNameEnum;
			result.UserName = userName;
			Add(result);
			return result;
		}

		public static List<JournalItem> BeginGetSKDFilteredArchive(SKDArchiveFilter archiveFilter, Guid archivePortionUID, bool isReport)
		{
			var journalItems = new List<JournalItem>();
			var result = new List<JournalItem>();

			try
			{
				lock (locker)
				{
					//var connectionString = global::SKDDriver.Properties.Settings.Default.SKUDConnectionString;
					var connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=SKD;Integrated Security=True;Language='English'";
					using (var dataContext = new SqlConnection(connectionString))
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

		static string BuildQuery(SKDArchiveFilter archiveFilter)
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

			if (archiveFilter.EventNames.Count > 0)
			{
				query += "\n and (";
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
				if (Enum.IsDefined(typeof(ObjectType), intValue))
					journalItem.ObjectType = (ObjectType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectUID")))
				journalItem.ObjectUID = reader.GetGuid(reader.GetOrdinal("ObjectUID"));

			if (!reader.IsDBNull(reader.GetOrdinal("State")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("State"));
				if (Enum.IsDefined(typeof(XStateClass), intValue))
					journalItem.State = (XStateClass)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("Subsystem")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("Subsystem"));
				if (Enum.IsDefined(typeof(SubsystemType), intValue))
					journalItem.SubsystemType = (SubsystemType)intValue;
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
				if (Enum.IsDefined(typeof(GlobalEventNameEnum), intValue))
					journalItem.Name = (GlobalEventNameEnum)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("Description")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("Description"));
				if (Enum.IsDefined(typeof(EventDescription), intValue))
					journalItem.Description = (EventDescription)intValue;
			}

			return journalItem;
		}

		static void UpdateNamesDescriptions(List<JournalItem> journalItems)
		{
			//var commands = new List<string>();
			//foreach (var item in journalItems)
			//{
			//	var name = item.Name;
			//	if (!EventNames.Any(x => name == x))
			//	{
			//		EventNames.Add(name);
			//		commands.Add(@"Insert Into EventNames (EventName) Values ('" + name + "')");
			//	}

			//	var description = item.Description;
			//	if (!EventDescriptions.Any(x => description == x))
			//	{
			//		EventDescriptions.Add(description);
			//		commands.Add(@"Insert Into EventDescriptions (EventDescription) Values ('" + description + "')");
			//	}
			//}
			//ExecuteNonQuery(commands);
		}
	}
}