using Common;
using RubezhAPI;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		#region Video
		public OperationResult<bool> SaveVideoUID(Guid itemUID, Guid videoUID, Guid cameraUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = Context.Journals.FirstOrDefault(x => x.UID == itemUID);
				if (tableItem != null)
				{
					tableItem.VideoUID = videoUID;
					tableItem.CameraUID = cameraUID;
					Context.SaveChanges();
				}
				return true;
			});
		}

		public OperationResult<bool> SaveCameraUID(Guid itemUID, Guid CameraUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = Context.Journals.FirstOrDefault(x => x.UID == itemUID);
				if (tableItem != null)
				{
					tableItem.CameraUID = CameraUID;
					Context.SaveChanges();
				}
				return true;
			});
		}
		#endregion

		public OperationResult<DateTime> GetMinDate()
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				return Context.Journals.Min(x => x.SystemDate);
			});
		}

		public OperationResult<bool> AddRange(List<JournalItem> apiItems)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				if (apiItems.Count == 0)
					return true;
				var query = new StringBuilder();
				foreach (var item in apiItems)
				{
					query.Append("INSERT INTO dbo.\"Journals\" (\"UID\", \"EmployeeUID\", \"SystemDate\", \"DeviceDate\", \"Subsystem\", \"Name\", \"Description\", \"DescriptionText\", \"ObjectType\", \"ObjectUID\", \"Detalisation\", \"UserName\", \"VideoUID\", \"CameraUID\", \"ObjectName\", \"CardNo\") VALUES");
					query.Append(string.Format("('{0}', {1}, '{2}', {3}, '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}'); ",
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
							item.CardNo));
				}
				Context.Database.ExecuteSqlCommand(query.ToString());
				return true;
			});
		}

		public OperationResult<bool> Add(JournalItem apiItem)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var result = Context.Journals.Add(TranslateBack(apiItem));
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItems = BuildJournalQuery(filter).ToList();
				return new List<JournalItem>(tableItems.Select(x => Translate(x)));
			});
		}

		public OperationResult<List<JournalItem>> GetFilteredArchiveItems(JournalFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItems = BuildArchiveQuery(filter).ToList();
				return new List<JournalItem>(tableItems.Select(x => Translate(x)));
			});
		}

		public OperationResult<List<JournalItem>> GetArchivePage(JournalFilter filter, int page)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var query = BuildArchiveQuery(filter).Skip((page - 1) * filter.PageSize).Take(filter.PageSize);
				var tableItems = query.ToList();
				return new List<JournalItem>(tableItems.Select(x => Translate(x))); ;
			});
		}

		public OperationResult<int> GetArchiveCount(JournalFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var query = BuildArchiveQuery(filter);
				return query.Count();
			});
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

		IQueryable<Journal> BuildJournalQuery(JournalFilter filter)
		{
			IQueryable<Journal> result = Context.Journals;
			if (filter.ItemUID.HasValue)
				result = result.Where(x => x.UID == filter.ItemUID.Value);
			result = FilterNames(result, filter);
			if (filter.JournalSubsystemTypes.Count > 0 && filter.JournalEventNameTypes.Count == 0)
			{
				var subsystems = filter.JournalSubsystemTypes.Select(x => (int)x).ToList();
				result = result.Where(x => subsystems.Contains(x.Subsystem));
			}
			if (filter.ObjectUIDs.Count > 0)
			{
				result = result.Where(x => filter.ObjectUIDs.Contains(x.ObjectUID));
			}
			else if (filter.JournalObjectTypes.Count > 0)
			{
				var objects = filter.JournalObjectTypes.Select(x => (int)x).ToList();
				result = result.Where(x => objects.Contains(x.ObjectType));
			}
			result = result.OrderByDescending(x => x.SystemDate).Take(filter.LastItemsCount);
			return result;
		}

		IQueryable<Journal> BuildArchiveQuery(JournalFilter filter)
		{
			IQueryable<Journal> result = Context.Journals;
			result = FilterNames(result, filter);
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

		IQueryable<Journal> FilterNames(IQueryable<Journal> journal, JournalFilter filter)
		{
			IQueryable<Journal> namesResult = null;
			IQueryable<Journal> descriptionsResult = null;
			if (filter.JournalEventNameTypes.IsNotNullOrEmpty())
			{
				var names = filter.JournalEventNameTypes.Select(x => (int)x).ToList();
				namesResult = journal.Where(x => names.Contains(x.Name));
			}
			if (filter.JournalEventDescriptionTypes.IsNotNullOrEmpty())
			{
				var descriptions = filter.JournalEventDescriptionTypes.Select(x => (int)x).ToList();
				descriptionsResult = journal.Where(x => descriptions.Contains(x.Description));
			}
			if(namesResult != null && descriptionsResult != null)
				return namesResult.Union(descriptionsResult);
			if (namesResult != null)
				return namesResult;
			if (descriptionsResult != null)
				return descriptionsResult;
			return journal;
		}
	}
}