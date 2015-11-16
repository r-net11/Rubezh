using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhAPI;
using RubezhAPI.Journal;
using System.Diagnostics;

namespace RubezhDAL.DataClasses
{
	public class JournalTranslator
	{
		DbService DbService;
		DatabaseContext Context;
		public PassJounalSynchroniser PassJournalSynchroniser { get; private set; }
		public JounalSynchroniser JournalSynchroniser { get; private set; }

		public JournalTranslator(DbService dbService)
		{
			DbService = dbService;
			Context = DbService.Context;
			JournalSynchroniser = new JounalSynchroniser(dbService);
			PassJournalSynchroniser = new PassJounalSynchroniser(dbService);
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

		public OperationResult AddRange(List<JournalItem> apiItems)
		{
			try
			{
				if (apiItems.Count == 0)
					return new OperationResult();
                string query = "";
                foreach (var item in apiItems)
                {
                    query += "INSERT INTO dbo.\"Journals\" (\"UID\", \"EmployeeUID\", \"SystemDate\", \"DeviceDate\", \"Subsystem\", \"Name\", \"Description\", \"DescriptionText\", \"ObjectType\", \"ObjectUID\", \"Detalisation\", \"UserName\", \"VideoUID\", \"CameraUID\", \"ObjectName\", \"CardNo\") VALUES";
					query += string.Format("('{0}', {1}, '{2}', {3}, '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}'); ",
							item.UID,
							item.EmployeeUID.EmptyToNullSqlStr(),
							item.SystemDateTime.CheckDate().ToString("yyyyMMdd HH:mm:ss"),
							item.DeviceDateTime.CheckDateSqlStr(),
							(int)EventDescriptionAttributeHelper.ToSubsystem(item.JournalEventNameType),
							(int)item.JournalEventNameType,
							(int)item.JournalEventDescriptionType,
							item.DescriptionText,
							(int)item.JournalObjectType,
							item.ObjectUID,
							JournalDetalisationItem.ListToString(item.JournalDetalisationItems),
							item.UserName,
							item.VideoUID,
							item.CameraUID,
							item.ObjectName,
							item.CardNo);
				}
                Context.Database.ExecuteSqlCommand(query);
                return new OperationResult();
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalTranslator.AddRange");
				return new OperationResult(e.Message);
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
				var tableItems = GetFilteredJournalItemsInternal(filter).ToList();
				var result = new List<JournalItem>(tableItems.Select(x => Translate(x)));
				return new OperationResult<List<JournalItem>>(result);
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalTranslator.GetFilteredJournalItems");
				return OperationResult<List<JournalItem>>.FromError(e.Message);
			}
		}

		public OperationResult<List<JournalItem>> GetFilteredArchiveItems(JournalFilter filter)
		{
			try
			{
				var tableItems = GetFilteredArchiveInternal(filter).ToList();
				var result = new List<JournalItem>(tableItems.Select(x => Translate(x)));
				return new OperationResult<List<JournalItem>>(result);
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalTranslator.GetFilteredJournalItems");
				return OperationResult<List<JournalItem>>.FromError(e.Message);
			}
		}

		public OperationResult<List<JournalItem>> GetArchivePage(JournalFilter filter, int page)
		{
			try
			{
				var query = GetFilteredArchiveInternal(filter);
				query = query.Skip((page - 1) * filter.PageSize).Take(filter.PageSize);
				var journalItems = query.ToList().Select(x => Translate(x)).ToList();
				return new OperationResult<List<JournalItem>>(journalItems);
			}
			catch (Exception e)
			{
				return OperationResult<List<JournalItem>>.FromError(e.Message);
			}
		}

		public OperationResult<int> GetArchiveCount(JournalFilter filter)
		{
			try
			{
				var query = GetFilteredArchiveInternal(filter);
				var result = query.Count();
				return new OperationResult<int>(result);
			}
			catch (Exception e)
			{
				return OperationResult<int>.FromError(e.Message);
			}
		}

		bool IsInFilter(Journal item, JournalFilter filter)
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
				SystemDate = apiItem.SystemDateTime.CheckDate(),
				DeviceDate = apiItem.DeviceDateTime.CheckDate(),
				Subsystem = (int)EventDescriptionAttributeHelper.ToSubsystem(apiItem.JournalEventNameType),
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

		IQueryable<Journal> GetFilteredJournalItemsInternal(JournalFilter filter)
		{
			IQueryable<Journal> result = Context.Journals;
			if (filter.JournalEventNameTypes.Count > 0)
			{
				var names = filter.JournalEventNameTypes.Select(x => (int)x).ToList();
				result = result.Where(x => names.Contains(x.Name));
			}
			if (filter.JournalEventDescriptionTypes.Count > 0)
			{
				var descriptions = filter.JournalEventDescriptionTypes.Select(x => (int)x).ToList();
				result = result.Where(x => descriptions.Contains(x.Description));
			}
			if (filter.JournalSubsystemTypes.Count > 0 && filter.JournalEventNameTypes.Count == 0)
			{
				var subsystems = filter.JournalSubsystemTypes.Select(x => (int)x).ToList();
				result = result.Where(x => subsystems.Contains(x.Subsystem));
			}
			if (filter.JournalObjectTypes.Count > 0)
			{
				var objects = filter.JournalObjectTypes.Select(x => (int)x).ToList();
				result = result.Where(x => objects.Contains(x.ObjectType));
			}
			if (filter.ObjectUIDs.Count > 0)
			{
				result = result.Where(x => filter.ObjectUIDs.Contains(x.ObjectUID));
			}
			if (filter.ObjectUIDs.Count > 0)
			{
				result = result.Where(x => filter.ObjectUIDs.Contains(x.ObjectUID));
			}
			result = result.OrderByDescending(x => x.SystemDate).Take(filter.LastItemsCount);
			return result;
		}

		IQueryable<Journal> GetFilteredArchiveInternal(JournalFilter filter)
		{
			IQueryable<Journal> result = Context.Journals;
			if (filter.JournalEventNameTypes.Count > 0)
			{
				var names = filter.JournalEventNameTypes.Select(x => (int)x).ToList();
				result = result.Where(x => names.Contains(x.Name));
			}
			if (filter.JournalEventDescriptionTypes.Count > 0)
			{
				var descriptions = filter.JournalEventDescriptionTypes.Select(x => (int)x).ToList();
				result = result.Where(x => descriptions.Contains(x.Description));
			}
			if (filter.JournalObjectTypes.Count > 0)
			{
				var objects = filter.JournalObjectTypes.Select(x => (int)x).ToList();
				result = result.Where(x => objects.Contains(x.ObjectType));
			}
			if (filter.ObjectUIDs.Count > 0)
			{
				result = result.Where(x => filter.ObjectUIDs.Contains(x.ObjectUID));
			}
			if (filter.EmployeeUIDs.Count > 0)
			{
				result = result.Where(x => filter.EmployeeUIDs.Contains(x.ObjectUID) ||
					(x.EmployeeUID != null && filter.EmployeeUIDs.Contains(x.EmployeeUID.Value)));
			}
			if (filter.UseDeviceDateTime)
			{
				result = result.Where(x => x.DeviceDate > filter.StartDate && x.DeviceDate < filter.EndDate);
			}
			else
			{
				result = result.Where(x => x.SystemDate > filter.StartDate && x.SystemDate < filter.EndDate);
			}
			if (filter.IsSortAsc)
			{
				if (filter.UseDeviceDateTime)
					result = result.OrderBy(x => x.DeviceDate);
				else
					result = result.OrderBy(x => x.SystemDate);
			}
			else
			{
				if (filter.UseDeviceDateTime)
					result = result.OrderByDescending(x => x.DeviceDate);
				else
					result = result.OrderByDescending(x => x.SystemDate);	
			}
			return result;
		}
	}
}