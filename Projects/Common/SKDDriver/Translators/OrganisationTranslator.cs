using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public class OrganisationTranslator : IsDeletedTranslator<DataAccess.Organisation, Organisation, OrganisationFilter>
	{
		public OrganisationTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
			Synchroniser = new OrganisationSynchroniser(Table, databaseService);
			Synchroniser.Initialize();
		}

		protected OperationResult CanSave(OrganisationDetails item)
		{
			bool hasSameName = Table.Any(x => x.Name == item.Name && !x.IsDeleted && x.UID != item.UID);
			if (hasSameName)
				return new OperationResult("Организация таким же именем уже содержится в базе данных");
			return new OperationResult();
		}

		protected override Organisation Translate(DataAccess.Organisation tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.PhotoUID = tableItem.PhotoUID;
			result.DoorUIDs = Context.OrganisationDoors.Where(x => x.OrganisationUID == result.UID).Select(x => x.DoorUID).ToList();
			result.UserUIDs = Context.OrganisationUsers.Where(x => x.OrganisationUID == result.UID).Select(x => x.UserUID).ToList();
			result.ChiefUID = tableItem.ChiefUID;
			result.HRChiefUID = tableItem.HRChiefUID;
			result.Phone = tableItem.Phone;
			return result;
		}

		protected override void TranslateBack(DataAccess.Organisation tableItem, Organisation apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.ChiefUID = apiItem.ChiefUID;
			tableItem.HRChiefUID = apiItem.HRChiefUID;
			tableItem.Phone = apiItem.Phone;
			if (tableItem.ExternalKey == null)
				tableItem.ExternalKey = "-1";
		}

		protected override OperationResult BeforeDelete(Guid uid, DateTime removalDate)
		{
			var result = new OperationResult();
			result = DatabaseService.EmployeeTranslator.MarkDeletedByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.DepartmentTranslator.MarkDeletedByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.PositionTranslator.MarkDeletedByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.AccessTemplateTranslator.MarkDeletedByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.PassCardTemplateTranslator.MarkDeletedByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.AdditionalColumnTypeTranslator.MarkDeletedByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.DayIntervalTranslator.MarkDeletedByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.HolidayTranslator.MarkDeletedByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.ScheduleTranslator.MarkDeletedByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.ScheduleSchemeTranslator.MarkDeletedByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			return result;
		}

		public OperationResult<bool> IsAnyItems(Guid organisationUID)
		{
			try
			{
				var result = Context.Employees.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.Departments.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.Positions.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.AccessTemplates.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.PassCardTemplates.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
					Context.AdditionalColumnTypes.Any(x => !x.IsDeleted && x.OrganisationUID == organisationUID) ||
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

		protected override OperationResult BeforeRestore(Guid uid, DateTime removalDate)
		{
			var result = new OperationResult();
			result = DatabaseService.EmployeeTranslator.RestoreByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.DepartmentTranslator.RestoreByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.PositionTranslator.RestoreByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.AccessTemplateTranslator.RestoreByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.PassCardTemplateTranslator.RestoreByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.AdditionalColumnTypeTranslator.RestoreByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.DayIntervalTranslator.RestoreByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.HolidayTranslator.RestoreByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.ScheduleTranslator.RestoreByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			result = DatabaseService.ScheduleSchemeTranslator.RestoreByOrganisation(uid, removalDate);
			if (result.HasError)
				return result;
			return result;
		}

		public OperationResult<OrganisationDetails> GetDetails(Guid uid)
		{
			try
			{
				var tableItem = Table.Where(x => x.UID.Equals(uid)).FirstOrDefault();
				if (tableItem == null)
					return new OperationResult<OrganisationDetails>(null);
				var organisationDetails = new OrganisationDetails
					{
						Description = tableItem.Description,
						IsDeleted = tableItem.IsDeleted,
						Name = tableItem.Name,
						RemovalDate = tableItem.RemovalDate,
						UID = tableItem.UID,
						DoorUIDs = (from x in Context.OrganisationDoors.Where(x => x.OrganisationUID == tableItem.UID) select x.DoorUID).ToList(),
						UserUIDs = (from x in Context.OrganisationUsers.Where(x => x.OrganisationUID == tableItem.UID) select x.UserUID).ToList(),
						ChiefUID = tableItem.ChiefUID,
						HRChiefUID = tableItem.HRChiefUID,
						Phone = tableItem.Phone
					};
				var photoResult = DatabaseService.PhotoTranslator.GetSingle(tableItem.PhotoUID);
				if (photoResult.HasError)
				{
					return OperationResult<OrganisationDetails>.FromError(photoResult.Errors);
				}
				organisationDetails.Photo = photoResult.Result;
				return new OperationResult<OrganisationDetails>(organisationDetails);
			}
			catch (Exception e)
			{
				return OperationResult<OrganisationDetails>.FromError(e.Message);
			}
		}

		public OperationResult SaveDoors(Organisation apiItem)
		{
			return SaveDoorsInternal(apiItem.UID, apiItem.DoorUIDs);
		}

		public OperationResult SaveDoors(OrganisationDetails apiItem)
		{
			return SaveDoorsInternal(apiItem.UID, apiItem.DoorUIDs);
		}

		private OperationResult SaveDoorsInternal(Guid organisationUID, List<Guid> doorUIDs)
		{
			try
			{
				var tableOrganisationDoors = Context.OrganisationDoors.Where(x => x.OrganisationUID == organisationUID);
				Context.OrganisationDoors.DeleteAllOnSubmit(tableOrganisationDoors);
				foreach (var doorUID in doorUIDs)
				{
					var tableOrganisationDoor = new DataAccess.OrganisationDoor();
					tableOrganisationDoor.UID = Guid.NewGuid();
					tableOrganisationDoor.DoorUID = doorUID;
					tableOrganisationDoor.OrganisationUID = organisationUID;
					tableOrganisationDoor.DoorUID = doorUID;
					Context.OrganisationDoors.InsertOnSubmit(tableOrganisationDoor);
				}
				var scheduleZones = Context.ScheduleZones.Where(x => x.Schedule.OrganisationUID == organisationUID && !doorUIDs.Contains(x.DoorUID));
				Context.ScheduleZones.DeleteAllOnSubmit(scheduleZones);
				Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult SaveUsers(Organisation apiItem)
		{
			return SaveUsersInternal(apiItem.UID, apiItem.UserUIDs);
		}

		public OperationResult SaveUsers(OrganisationDetails apiItem)
		{
			return SaveUsersInternal(apiItem.UID, apiItem.UserUIDs);
		}

		private OperationResult SaveUsersInternal(Guid organisationUID, List<Guid> UserUIDs)
		{
			try
			{
				var tableOrganisationUsers = Context.OrganisationUsers.Where(x => x.OrganisationUID == organisationUID);
				Context.OrganisationUsers.DeleteAllOnSubmit(tableOrganisationUsers);
				foreach (var UserUID in UserUIDs)
				{
					var tableOrganisationUser = new DataAccess.OrganisationUser();
					tableOrganisationUser.UID = Guid.NewGuid();
					tableOrganisationUser.OrganisationUID = organisationUID;
					tableOrganisationUser.UserUID = UserUID;
					Context.OrganisationUsers.InsertOnSubmit(tableOrganisationUser);
				}
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult SaveChief(Guid uid, Guid chiefUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				tableItem.ChiefUID = chiefUID;
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult SaveHRChief(Guid uid, Guid chiefUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				tableItem.HRChiefUID = chiefUID;
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult Save(OrganisationDetails apiItem)
		{
			var saveDoorsResult = SaveDoors(apiItem);
			if (saveDoorsResult.HasError)
				return saveDoorsResult;
			var saveUsersResult = SaveUsers(apiItem);
			if (saveUsersResult.HasError)
				return saveUsersResult;
			var photoSaveResult = DatabaseService.PhotoTranslator.SaveOrDelete(apiItem.Photo);
			if (photoSaveResult.HasError)
				return photoSaveResult;
			var systemTypesSaveResult =
				DatabaseService.TimeTrackDocumentTypeTranslator.AddSystemDocumentTypesForOrganisation(apiItem.UID);
			if (systemTypesSaveResult.HasError)
				return systemTypesSaveResult;

			try
			{
				var verifyResult = CanSave(apiItem);
				if (verifyResult.HasError)
					return verifyResult;
				var tableItem = (from x in Table where x.UID.Equals(apiItem.UID) select x).FirstOrDefault();
				if (tableItem == null)
				{
					tableItem = new DataAccess.Organisation {UID = apiItem.UID, Name = apiItem.Name, Description = apiItem.Description};
					if (apiItem.Photo != null)
						tableItem.PhotoUID = apiItem.Photo.UID;
					tableItem.IsDeleted = apiItem.IsDeleted;
					tableItem.RemovalDate = TranslatiorHelper.CheckDate(apiItem.RemovalDate);
					tableItem.ChiefUID = apiItem.ChiefUID;
					tableItem.HRChiefUID = apiItem.HRChiefUID;
					tableItem.Phone = apiItem.Phone;
					if (tableItem.ExternalKey == null)
						tableItem.ExternalKey = "-1";
					Table.InsertOnSubmit(tableItem);
				}
				else
				{
					tableItem.Name = apiItem.Name;
					tableItem.Description = apiItem.Description;
					if (apiItem.Photo != null)
						tableItem.PhotoUID = apiItem.Photo.UID;
					tableItem.IsDeleted = apiItem.IsDeleted;
					tableItem.RemovalDate = TranslatiorHelper.CheckDate(apiItem.RemovalDate);
					tableItem.ChiefUID = apiItem.ChiefUID;
					tableItem.HRChiefUID = apiItem.HRChiefUID;
					tableItem.Phone = apiItem.Phone;
					if (tableItem.ExternalKey == null)
						tableItem.ExternalKey = "-1";
				}
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected override Expression<Func<DataAccess.Organisation, bool>> IsInFilter(OrganisationFilter filter)
		{
			var result = base.IsInFilter(filter);
			if (filter.UserUID != Guid.Empty)
				result = result.And(e => e.OrganisationUsers.Any(x => x.UserUID == filter.UserUID));
			return result;
		}

		public OrganisationSynchroniser Synchroniser;
	}
}