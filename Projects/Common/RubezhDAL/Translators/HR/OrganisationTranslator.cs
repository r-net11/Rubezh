using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public class OrganisationTranslator
	{
		DbService DbService;
		DatabaseContext Context;
		public OrganisationSynchroniser Synchroniser { get; private set; }
		public OrgansiationListSynchroniser ListSynchroniser { get; private set; }

		public OrganisationTranslator(DbService context)
		{
			DbService = context;
			Context = DbService.Context;
			Synchroniser = new OrganisationSynchroniser(Table, DbService);
			ListSynchroniser = new OrgansiationListSynchroniser(Table, DbService);
		}

		public OperationResult<List<API.Organisation>> Get(API.OrganisationFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItems = GetFilteredTableItems(filter).ToList();
				return tableItems.Select(x => Translate(x)).ToList();
			});
		}

		public OperationResult<API.Organisation> GetSingle(Guid uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItems = GetTableItems().FirstOrDefault(x => x.UID == uid);
				return Translate(tableItems);
			});
		}

		public OperationResult<API.OrganisationDetails> GetDetails(Guid uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItems = GetTableItems().FirstOrDefault(x => x.UID == uid);
				return TranslateDetails(tableItems);
			});
		}

		public OperationResult<bool> Save(API.OrganisationDetails item)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var canSaveResult = CanSave(item);
				if (canSaveResult.HasError)
					throw new Exception(canSaveResult.Error);
				var tableItem = GetTableItems().FirstOrDefault(x => x.UID == item.UID);
				if (tableItem == null)
				{
					tableItem = new Organisation { UID = item.UID };
					TranslateDetailsBack(item, tableItem);
					Table.Add(tableItem);
					var dayInterval = new DayInterval()
					{
						UID = Guid.NewGuid(),
						Name = "Выходной",
						DayIntervalParts = new List<DayIntervalPart>(),
						OrganisationUID = item.UID
					};
					Context.DayIntervals.Add(dayInterval);
				}
				else
				{
					ClearDependentData(tableItem);
					TranslateDetailsBack(item, tableItem);
				}
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> MarkDeleted(Guid uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var canDeleteResult = CanDelete(uid);
				if (canDeleteResult.HasError)
					throw new Exception(canDeleteResult.Error);
				var removalDate = DateTime.Now;
				DeleteItems(uid, removalDate);
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem != null)
				{
					tableItem.IsDeleted = true;
					tableItem.RemovalDate = removalDate;
					Context.SaveChanges();
				}
				return true;
			});
		}

		void DeleteItems(Guid uid, DateTime removalDate)
		{
			MarkDeletedSQLQuery("DayIntervals", removalDate, uid);
			MarkDeletedSQLQuery("Schedules", removalDate, uid);
			MarkDeletedSQLQuery("ScheduleSchemes", removalDate, uid);
			MarkDeletedSQLQuery("AccessTemplates", removalDate, uid);
			MarkDeletedSQLQuery("AdditionalColumnTypes", removalDate, uid);
			MarkDeletedSQLQuery("Employees", removalDate, uid);
			MarkDeletedSQLQuery("Holidays", removalDate, uid);
			MarkDeletedSQLQuery("PassCardTemplates", removalDate, uid);
			MarkDeletedSQLQuery("Positions", removalDate, uid);
			MarkDeletedSQLQuery("Departments", removalDate, uid);
		}

		public OperationResult<bool> Restore(Guid uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem != null)
				{
					if (Table.Any(x => !x.IsDeleted && x.UID != tableItem.UID && x.Name == tableItem.Name))
						throw new Exception("Невозможно восстановить организацию. Существует активная организация с совпадающим именем.");
					RestoreItems(tableItem.UID, tableItem.RemovalDate.GetValueOrDefault());
					tableItem.IsDeleted = false;
					tableItem.RemovalDate = null;
					Context.SaveChanges();
				}
				return true;
			});
		}

		void RestoreItems(Guid uid, DateTime removalDate)
		{
			RestoreSQLQuery("DayIntervals", removalDate, uid);
			RestoreSQLQuery("Schedules", removalDate, uid);
			RestoreSQLQuery("ScheduleSchemes", removalDate, uid);
			RestoreSQLQuery("AccessTemplates", removalDate, uid);
			RestoreSQLQuery("AdditionalColumnTypes", removalDate, uid);
			RestoreSQLQuery("Employees", removalDate, uid);
			RestoreSQLQuery("Holidays", removalDate, uid);
			RestoreSQLQuery("PassCardTemplates", removalDate, uid);
			RestoreSQLQuery("Positions", removalDate, uid);
			RestoreSQLQuery("Departments", removalDate, uid);
		}

		OperationResult<bool> CanSave(API.OrganisationDetails item)
		{
			if (item == null)
				return OperationResult<bool>.FromError("Попытка сохранить пустую запись");
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return OperationResult<bool>.FromError("Запись с таким же названием уже существует");
			else
				return new OperationResult<bool>(true);
		}

		OperationResult<bool> CanDelete(Guid uid)
		{
			return new OperationResult<bool>(true);
		}

		public API.Organisation Translate(Organisation tableItem)
		{
			return new API.Organisation
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate.GetValueOrDefault(),
				DoorUIDs = tableItem.Doors.Select(x => x.DoorUID).ToList(),
				UserUIDs = tableItem.Users.Select(x => x.UserUID).ToList(),
				ChiefUID = tableItem.ChiefUID.GetValueOrDefault(),
				HRChiefUID = tableItem.HRChiefUID.GetValueOrDefault(),
				MaxGKLevel = tableItem.MaxGKLevel,
			};
		}

		public API.OrganisationDetails TranslateDetails(Organisation tableItem)
		{
			return new API.OrganisationDetails
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate.GetValueOrDefault(),
				DoorUIDs = tableItem.Doors.Select(x => x.DoorUID).ToList(),
				UserUIDs = tableItem.Users.Select(x => x.UserUID).ToList(),
				ChiefUID = tableItem.ChiefUID.GetValueOrDefault(),
				HRChiefUID = tableItem.HRChiefUID.GetValueOrDefault(),
				Phone = tableItem.Phone,
				Photo = tableItem.Photo != null ? tableItem.Photo.Translate() : null,
				MaxGKLevel = tableItem.MaxGKLevel,

			};
		}

		public void TranslateDetailsBack(API.OrganisationDetails apiItem, Organisation tableItem)
		{
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.IsDeleted = apiItem.IsDeleted;
			tableItem.RemovalDate = apiItem.RemovalDate.CheckDate();
			tableItem.Doors = apiItem.DoorUIDs.Select(x => new OrganisationDoor { UID = Guid.NewGuid(), DoorUID = x }).ToList();
			tableItem.Users = apiItem.UserUIDs.Select(x => new OrganisationUser { UID = Guid.NewGuid(), UserUID = x }).ToList();
			tableItem.ChiefUID = apiItem.ChiefUID.EmptyToNull();
			tableItem.HRChiefUID = apiItem.HRChiefUID.EmptyToNull();
			tableItem.Phone = apiItem.Phone;
			tableItem.Photo = Photo.Create(apiItem.Photo);
			tableItem.MaxGKLevel = apiItem.MaxGKLevel;
		}

		DbSet<Organisation> Table { get { return Context.Organisations; } }

		IQueryable<Organisation> GetTableItems()
		{
			return Table
				.Include(x => x.Users)
				.Include(x => x.Doors)
				.Include(x => x.Photo);
		}

		void ClearDependentData(Organisation tableItem)
		{
			Context.OrganisationUsers.RemoveRange(tableItem.Users);
			Context.OrganisationDoors.RemoveRange(tableItem.Doors);
			if (tableItem.Photo != null)
				Context.Photos.Remove(tableItem.Photo);
		}

		IQueryable<Organisation> GetFilteredTableItems(API.OrganisationFilter filter)
		{
			var result = GetTableItems();
			if (filter.UIDs.Count() != 0)
				result = result.Where(x => filter.UIDs.Contains(x.UID));
			if (filter.ExceptUIDs.Count() != 0)
				result = result.Where(x => !filter.ExceptUIDs.Contains(x.UID));
			if (filter.LogicalDeletationType == API.LogicalDeletationType.Active)
				result = result.Where(x => !x.IsDeleted);
			if (filter.LogicalDeletationType == API.LogicalDeletationType.Deleted)
				result = result.Where(x => x.IsDeleted);
			if (filter.User != null && !filter.User.IsAdm)
				result = result.Where(x => x.Users.Any(organisationUser => organisationUser.UserUID == filter.User.UID));
			return result;
		}

		public OperationResult<bool> AddDoor(Guid uid, Guid doorUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				Context.OrganisationDoors.Add(new OrganisationDoor { UID = Guid.NewGuid(), DoorUID = doorUID, OrganisationUID = uid });
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> RemoveDoor(Guid uid, Guid doorUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var doorsToRemove = Context.OrganisationDoors.Where(x => x.OrganisationUID == uid && x.DoorUID == doorUID);
				Context.OrganisationDoors.RemoveRange(doorsToRemove);
				var zonesToRemove = Context.ScheduleZones.Include(x => x.Schedule).Where(x => x.Schedule.OrganisationUID == uid && x.DoorUID == doorUID);
				Context.ScheduleZones.RemoveRange(zonesToRemove);
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> SaveUsers(Guid organisationUID, List<Guid> UserUIDs)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableOrganisationUsers = Context.OrganisationUsers.Where(x => x.OrganisationUID == organisationUID);
				Context.OrganisationUsers.RemoveRange(tableOrganisationUsers);
				foreach (var UserUID in UserUIDs)
				{
					var tableOrganisationUser = new OrganisationUser();
					tableOrganisationUser.UID = Guid.NewGuid();
					tableOrganisationUser.OrganisationUID = organisationUID;
					tableOrganisationUser.UserUID = UserUID;
					Context.OrganisationUsers.Add(tableOrganisationUser);
				}
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> IsAnyItems(Guid organisationUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				return
					Context.Employees.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.Departments.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.Positions.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.AccessTemplates.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.PassCardTemplates.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.AdditionalColumnTypes.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.DayIntervals.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.Holidays.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.Schedules.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.ScheduleSchemes.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID);
			});
		}

		public OperationResult<bool> SaveChief(Guid uid, Guid? chiefUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem == null)
					throw new Exception("Запись не найдена");
				tableItem.ChiefUID = chiefUID != null ? chiefUID.Value.EmptyToNull() : null;
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> SaveHRChief(Guid uid, Guid? chiefUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem == null)
					throw new Exception("Запись не найдена");
				tableItem.HRChiefUID = chiefUID != null ? chiefUID.Value.EmptyToNull() : null;
				Context.SaveChanges();
				return true;
			});
		}

		void MarkDeletedSQLQuery(string tableName, DateTime removalDate, Guid organisationUID)
		{
			var command = string.Format("UPDATE dbo.\"{0}\" SET \"RemovalDate\" = '{1}', \"IsDeleted\" = '1' WHERE \"IsDeleted\" = '0' AND \"OrganisationUID\" = '{2}'", tableName, removalDate.ToString("yyyyMMdd HH:mm:ss"), organisationUID);
			Context.Database.ExecuteSqlCommand(command);
		}

		void RestoreSQLQuery(string tableName, DateTime removalDate, Guid organisationUID)
		{
			var command = string.Format("UPDATE dbo.\"{0}\" SET \"RemovalDate\" = NULL, \"IsDeleted\" = '0' WHERE \"IsDeleted\" = '1' AND \"OrganisationUID\" = '{2}' AND \"RemovalDate\" = '{1}'", tableName, removalDate.ToString("yyyyMMdd HH:mm:ss"), organisationUID);
			Context.Database.ExecuteSqlCommand(command);
		}

		void MarkDeletedByOrganisation<TTableItem>(Guid organisationUID, DateTime removalDate, DbSet<TTableItem> Table)
			where TTableItem : class, IOrganisationItem
		{
			var tableItems = Table.Where(x => x.OrganisationUID == organisationUID && !x.IsDeleted);
			foreach (var item in tableItems)
			{
				item.IsDeleted = true;
				item.RemovalDate = removalDate;
			}
		}

		void RestoreByOrganisation<TTableItem>(Guid organisationUID, DateTime removalDate, DbSet<TTableItem> Table)
			where TTableItem : class, IOrganisationItem
		{
			var tableItems = Table.Where(x => x.OrganisationUID == organisationUID && x.IsDeleted && x.RemovalDate == removalDate);
			foreach (var item in tableItems)
			{
				item.IsDeleted = false;
				item.RemovalDate = null;
			}
		}
	}
}