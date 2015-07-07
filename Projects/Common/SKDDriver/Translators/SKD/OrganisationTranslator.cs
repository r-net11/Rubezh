using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class OrganisationTranslator
	{
		DbService DbService; 
		DatabaseContext Context;
		public OrganisationSynchroniser Synchroniser { get; private set; }

		public OrganisationTranslator(DbService context)
		{
			DbService = context;
			Context = DbService.Context;
			Synchroniser = new OrganisationSynchroniser(Table, DbService);
		}

		public OperationResult<List<API.Organisation>> Get(API.OrganisationFilter filter)
		{
			try
			{
				var tableItems = GetFilteredTableItems(filter).ToList();
				var result = tableItems.Select(x => Translate(x)).ToList();
				return new OperationResult<List<API.Organisation>>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<List<API.Organisation>>.FromError(e.Message);
			}
		}

		public OperationResult<API.Organisation> GetSingle(Guid uid)
		{
			try
			{
				var tableItems = GetTableItems().FirstOrDefault(x => x.UID == uid);
				var result = Translate(tableItems);
				return new OperationResult<API.Organisation>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<API.Organisation>.FromError(e.Message);
			}
		}

		public OperationResult<API.OrganisationDetails> GetDetails(Guid uid)
		{
			try
			{
				var tableItems = GetTableItems().FirstOrDefault(x => x.UID == uid);
				var result = TranslateDetails(tableItems);
				return new OperationResult<API.OrganisationDetails>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<API.OrganisationDetails>.FromError(e.Message);
			}
		}

		public OperationResult<bool> Save(API.OrganisationDetails item)
		{
			try
			{
				var canSaveResult = CanSave(item);
				if (canSaveResult.HasError)
					return canSaveResult;
				var tableItem = GetTableItems().FirstOrDefault(x => x.UID == item.UID);
				if (tableItem == null)
				{
					tableItem = new Organisation { UID = item.UID };
					TranslateDetailsBack(item, tableItem);
					Table.Add(tableItem);
				}
				else
				{
					ClearDependentData(tableItem);
					TranslateDetailsBack(item, tableItem);
				}
				Context.SaveChanges();
				return new OperationResult<bool>(true);
			}
			catch (System.Exception e)
			{
				return OperationResult<bool>.FromError(e.Message);
			}
		}

		public OperationResult MarkDeleted(Guid uid)
		{
			try
			{
				var canDeleteResult = CanDelete(uid);
				if (canDeleteResult.HasError)
					return canDeleteResult;
				var removalDate = DateTime.Now;
				DeleteItems(uid, removalDate);
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem != null)
				{
					tableItem.IsDeleted = true;
					tableItem.RemovalDate = removalDate;
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		void DeleteItems(Guid uid, DateTime removalDate)
		{
			MarkDeletedByOrganisation(uid, removalDate, Context.DayIntervals);
			MarkDeletedByOrganisation(uid, removalDate, Context.Schedules);
			MarkDeletedByOrganisation(uid, removalDate, Context.ScheduleSchemes);
		}

		public OperationResult Restore(Guid uid)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem != null)
				{
					RestoreItems(tableItem.UID, tableItem.RemovalDate.GetValueOrDefault());
					tableItem.IsDeleted = false;
					tableItem.RemovalDate = null;
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		void RestoreItems(Guid uid, DateTime removalDate)
		{
			RestoreByOrganisation(uid, removalDate, Context.DayIntervals);
			RestoreByOrganisation(uid, removalDate, Context.Schedules);
			RestoreByOrganisation(uid, removalDate, Context.ScheduleSchemes);
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

		OperationResult CanDelete(Guid uid)
		{
			return new OperationResult();
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
				Photo = tableItem.Photo != null ? tableItem.Photo.Translate() : null
			};
		}

		public void TranslateDetailsBack(API.OrganisationDetails apiItem, Organisation tableItem)
		{
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.IsDeleted = apiItem.IsDeleted;
			tableItem.RemovalDate = apiItem.RemovalDate;
			tableItem.Doors = apiItem.DoorUIDs.Select(x => new OrganisationDoor { UID = Guid.NewGuid(), DoorUID = x }).ToList();
			tableItem.Users = apiItem.UserUIDs.Select(x => new OrganisationUser { UID = Guid.NewGuid(), UserUID = x }).ToList();
			tableItem.ChiefUID = apiItem.ChiefUID.EmptyToNull();
			tableItem.HRChiefUID = apiItem.HRChiefUID.EmptyToNull();
			tableItem.Phone = apiItem.Phone;
			tableItem.Photo = Photo.Create(apiItem.Photo);
		}

		DbSet<Organisation> Table { get { return Context.Organisations; } }
		
		IQueryable<Organisation> GetTableItems()
		{
			return Table.Include(x => x.Users).Include(x => x.Doors).Include(x => x.Photo);
		}
		
		void ClearDependentData(Organisation tableItem) 
		{
			Context.OrganisationUsers.RemoveRange(tableItem.Users);
			Context.OrganisationDoors.RemoveRange(tableItem.Doors);
			if(tableItem.Photo != null)
				Context.Photos.Remove(tableItem.Photo);
		}
		
		IQueryable<Organisation> GetFilteredTableItems(API.OrganisationFilter filter)
		{
			return GetTableItems().Where(x =>
				(filter.UIDs.Count() == 0 || filter.UIDs.Contains(x.UID)) &&
				(filter.ExceptUIDs.Count() == 0 || !filter.ExceptUIDs.Contains(x.UID)) &&
				(filter.LogicalDeletationType != API.LogicalDeletationType.Active || !x.IsDeleted) &&
				(filter.LogicalDeletationType != API.LogicalDeletationType.Deleted || x.IsDeleted) &&
				(filter.UserUID == Guid.Empty ||
					x.Users.Any(organisationUser => organisationUser.UserUID == filter.UserUID)) 
			);
		}

		public OperationResult AddDoor(Guid uid, Guid doorUID)
		{
			try
			{
				Context.OrganisationDoors.Add(new OrganisationDoor { UID = Guid.NewGuid(), DoorUID = doorUID, OrganisationUID = uid });
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult RemoveDoor(Guid uid, Guid doorUID)
		{
			try
			{
				var doorsToRemove = Context.OrganisationDoors.Where(x => x.OrganisationUID == uid && x.DoorUID == doorUID);
				Context.OrganisationDoors.RemoveRange(doorsToRemove);
				var zonesToRemove = Context.ScheduleZones.Include(x => x.Schedule).Where(x => x.Schedule.OrganisationUID == uid && x.DoorUID == doorUID);
				Context.ScheduleZones.RemoveRange(zonesToRemove);
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult SaveUsers(Guid organisationUID, List<Guid> UserUIDs)
		{
			try
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
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult<bool> IsAnyItems(Guid organisationUID)
		{
			try
			{
				var result = 
					//Context.Employees.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					//Context.Departments.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					//Context.Positions.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					//Context.AccessTemplates.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					//Context.PassCardTemplates.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					//Context.AdditionalColumnTypes.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.DayIntervals.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.Holidays.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.Schedules.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.ScheduleSchemes.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID);
				return new OperationResult<bool>(result);
			}
			catch (Exception e)
			{
				return OperationResult<bool>.FromError(e.Message);
			}
		}

		public OperationResult SaveChief(Guid uid, Guid? chiefUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
                if (tableItem == null)
                    return new OperationResult("Запись не найдена");
                tableItem.ChiefUID = chiefUID != null ? chiefUID.Value.EmptyToNull() : null;
                Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			
		}

		public OperationResult SaveHRChief(Guid uid, Guid? chiefUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
                if (tableItem == null)
                    return new OperationResult("Запись не найдена");
                tableItem.HRChiefUID = chiefUID != null ? chiefUID.Value.EmptyToNull() : null;
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
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
