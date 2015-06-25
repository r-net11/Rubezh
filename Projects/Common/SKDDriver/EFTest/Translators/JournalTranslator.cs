using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Journal;

namespace SKDDriver.DataClasses
{
	public class JournalTranslator
	{
		DbService DbService; 
		DatabaseContext Context;
		public PassJounalSynchroniser Synchroniser { get; private set; }

		public JournalTranslator(DbService context)
		{
			DbService = context;
			Context = DbService.Context;
		}
		
		public event Action<List<JournalItem>, Guid> ArchivePortionReady;
		public static bool IsAbort { get; set; }

		
		#region Video
		public OperationResult SaveVideoUID(Guid itemUID, Guid videoUID, Guid cameraUID)
		{
			try
			{
				var tableItem = Context.Journals.FirstOrDefault(x => x.UID == itemUID);
				if (tableItem != null)
				{
					tableItem.VideoUID = videoUID;
					tableItem.CameraUID = cameraUID;
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult SaveCameraUID(Guid itemUID, Guid CameraUID)
		{
			try
			{
				var tableItem = Context.Journals.FirstOrDefault(x => x.UID == itemUID);
				if (tableItem != null)
				{
					tableItem.CameraUID = CameraUID;
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}
		#endregion
		
		public OperationResult<DateTime> GetMinDate()
		{
			try
			{
				var result = Context.Journals.Min(x => x.SystemDate);
				return new OperationResult<DateTime>(result);
			}
			catch (Exception e)
			{
				return OperationResult<DateTime>.FromError(e.Message);
			}
		}

		public OperationResult Add(JournalItem apiItem)
		{
			try
			{
				var result = Context.Journals.Add(TranslateBack(apiItem));
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalTranslator.Add");
				return new OperationResult(e.Message);
			}
		}

		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			try
			{
				var tableItems = Context.Journals.SqlQuery(BuildJournalFilterQuery(filter)).ToList();
				var result = new List<JournalItem>(tableItems.Select(x => Translate(x)));
				return new OperationResult<List<JournalItem>>(result);
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalTranslator.GetFilteredJournalItems");
				return OperationResult<List<JournalItem>>.FromError(e.Message);
			}
		}

		public OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			try
			{
				IsAbort = false;
				var pageSize = archiveFilter.PageSize;
				int page = 0;
				bool isEnd = false;
				while (!isEnd && !IsAbort)
				{
					var journalItems = Context.Journals.SqlQuery(BuildArchiveFilterQuery(archiveFilter)).Skip(page * pageSize).Take(pageSize).ToList().Select(x => Translate(x)).ToList();
					page++;
					isEnd = journalItems.Count < pageSize;
					PublishNewItemsPortion(journalItems, archivePortionUID);
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalTranslator.GetFilteredJournalItems");
				return new OperationResult(e.Message);
			}
		}

		bool IsInFilter(Journal item, ArchiveFilter filter)
		{
			bool result = true;
			if (filter.UseDeviceDateTime)
			{
				result = result && item.DeviceDate > filter.StartDate && item.DeviceDate < filter.EndDate;
			}
			else
			{
				result = result && item.SystemDate > filter.StartDate && item.SystemDate < filter.EndDate;
			}
			return result;
		}

		void PublishNewItemsPortion(List<JournalItem> journalItems, Guid archivePortionUID)
		{
			if (ArchivePortionReady != null)
				ArchivePortionReady(journalItems.ToList(), archivePortionUID);
		}

		JournalItem Translate(Journal apiItem)
		{
			return new JournalItem
			{
				UID = apiItem.UID,
				SystemDateTime = apiItem.SystemDate,
				DeviceDateTime = apiItem.DeviceDate,
				JournalSubsystemType = (JournalSubsystemType)apiItem.Subsystem,
				JournalEventNameType = (JournalEventNameType)apiItem.Name,
				JournalEventDescriptionType = (JournalEventDescriptionType)apiItem.Description,
				DescriptionText = apiItem.DescriptionText,
				JournalObjectType = (JournalObjectType)apiItem.ObjectType,
				ObjectUID = apiItem.ObjectUID,
				ObjectName = apiItem.ObjectName,
				UserName = apiItem.UserName,
				CardNo = apiItem.CardNo,
				EmployeeUID = apiItem.EmployeeUID.GetValueOrDefault(),
				VideoUID = apiItem.VideoUID.GetValueOrDefault(),
				CameraUID = apiItem.CameraUID.GetValueOrDefault(),
				JournalDetalisationItems = JournalDetalisationItem.StringToList(apiItem.Detalisation),
			};
		}

		Journal TranslateBack(JournalItem apiItem)
		{
			return new Journal
			{
				UID = apiItem.UID,
				SystemDate = apiItem.SystemDateTime,
				DeviceDate = apiItem.DeviceDateTime,
				Subsystem = (int)apiItem.JournalSubsystemType,
				Name = (int)apiItem.JournalEventNameType,
				Description = (int)apiItem.JournalEventDescriptionType,
				DescriptionText = apiItem.DescriptionText,
				ObjectType = (int)apiItem.JournalObjectType,
				ObjectUID = apiItem.ObjectUID,
				ObjectName = apiItem.ObjectName,
				UserName = apiItem.UserName,
				CardNo = apiItem.CardNo,
				EmployeeUID = apiItem.EmployeeUID.EmptyToNull(),
				VideoUID = apiItem.VideoUID,
				CameraUID = apiItem.CameraUID,
				Detalisation = JournalDetalisationItem.ListToString(apiItem.JournalDetalisationItems),
			};
		}

		string BuildJournalFilterQuery(JournalFilter journalFilter)
		{
			var lastItemsCount = journalFilter.LastItemsCount.ToString();
			string query;
			if(Context.ContextType == DbContextType.MSSQL)
				query = string.Format("SELECT TOP ({0}) * FROM {1}", lastItemsCount, IntoBrackets("Journal"));
			else
				query = string.Format("SELECT * FROM {0}", IntoBrackets("Journal"));
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
					query += string.Format("{0} = '{1}'", IntoBrackets("Name"), (int)journalEventNameType);
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
					query += string.Format("{0} = '{1}'", IntoBrackets("Description"), (int)journalEventDescriptionType);
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
					query += string.Format("{0} = '{1}'", IntoBrackets("Subsystem"), (int)journalSubsystemType);
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
					query += string.Format("{0} = '{1}'", IntoBrackets("ObjectType"), (int)journalObjectType);
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
					query += string.Format("{0} = '{1}'", IntoBrackets("ObjectUID"), objectUID);
				}
				query += ")";
			}

			query += string.Format("\n ORDER BY {0} DESC", IntoBrackets("SystemDate"));
			if (Context.ContextType == DbContextType.PostgreSQL)
				query += string.Format("\n LIMIT {0}", lastItemsCount);
			return query;
		}

		string BuildArchiveFilterQuery(ArchiveFilter archiveFilter)
		{
			string dateTimeTypeString;
			if (archiveFilter.UseDeviceDateTime)
				dateTimeTypeString = IntoBrackets("DeviceDate");
			else
				dateTimeTypeString = IntoBrackets("SystemDate");

			var query = string.Format( "SELECT * FROM {0} WHERE {1}  > '{2}'  AND {3}  < '{4}'", 
				IntoBrackets("Journal"), 
				dateTimeTypeString, 
				archiveFilter.StartDate.ToString("yyyy-MM-dd HH:mm:ss"), 
				dateTimeTypeString, 
				archiveFilter.EndDate.ToString("yyyy-MM-dd HH:mm:ss"));

			if (archiveFilter.JournalEventNameTypes.Count > 0)
			{
				query += "\n and (";
				int index = 0;
				foreach (var journalEventNameType in archiveFilter.JournalEventNameTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += string.Format("{0} = '{1}'", IntoBrackets("Name"), (int)journalEventNameType);
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
					query += string.Format("{0} = '{1}'", IntoBrackets("Subsystem"), (int)journalSubsystemType);
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
					query += string.Format("{0} = '{1}'", IntoBrackets("ObjectType"), (int)journalObjectType);
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
					query += string.Format("{0} = '{1}'", IntoBrackets("ObjectUID"), objectUID);
				}
				query += ")";
			}

			query += "\n ORDER BY " + dateTimeTypeString + " DESC";

			return query;
		}

		string IntoBrackets(string item)
		{
			string openingBracket = "";
			string closingBracket = "";
			switch (Context.ContextType)
			{
				case DbContextType.MSSQL:
					openingBracket = "[";
					closingBracket = "]";
					break;
				case DbContextType.PostgreSQL:
					openingBracket = "\"";
					closingBracket = "\"";
					break;
			}
			return openingBracket + item + closingBracket;
		}

	}
}
