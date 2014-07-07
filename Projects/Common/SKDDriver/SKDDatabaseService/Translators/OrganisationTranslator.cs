using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class OrganisationTranslator : IsDeletedTranslator<DataAccess.Organisation, Organisation, OrganisationFilter>
	{
		public OrganisationTranslator(DataAccess.SKDDataContext context, PhotoTranslator photoTranslator)
			: base(context)
		{
			PhotoTranslator = photoTranslator;
		}

		PhotoTranslator PhotoTranslator; 

		protected OperationResult CanSave(OrganisationDetails item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name && !x.IsDeleted && x.UID != item.UID);
			if (sameName)
				return new OperationResult("Организация таким же именем уже содержится в базе данных");
			return new OperationResult();
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.AdditionalColumnTypes.Any(x => x.OrganisationUID == uid) ||
					Context.Departments.Any(x => x.OrganisationUID == uid) ||
					Context.Documents.Any(x => x.OrganisationUID == uid) ||
					Context.Employees.Any(x => x.OrganisationUID == uid) ||
					Context.EmployeeReplacements.Any(x => x.OrganisationUID == uid) ||
					Context.Holidays.Any(x => x.OrganisationUID == uid) ||
					Context.NamedIntervals.Any(x => x.OrganisationUID == uid) ||
					Context.Positions.Any(x => x.OrganisationUID == uid) ||
					Context.Phones.Any(x => x.OrganisationUID == uid) ||
					Context.Schedules.Any(x => x.OrganisationUID == uid) ||
					Context.ScheduleSchemes.Any(x => x.OrganisationUID == uid) ||
					Context.AccessTemplates.Any(x => x.OrganisationUID == uid)
				)
				return new OperationResult("Организация не может быть удалена, пока существуют элементы привязанные к ней");
			return base.CanDelete(uid);
		}

		protected override Organisation Translate(DataAccess.Organisation tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.PhotoUID = tableItem.PhotoUID;
			result.DoorUIDs = (from x in Context.OrganisationDoors.Where(x => x.OrganisationUID == result.UID) select x.DoorUID).ToList();
			result.ZoneUIDs = (from x in Context.OrganisationZones.Where(x => x.OrganisationUID == result.UID) select x.ZoneUID).ToList();
			result.CardTemplateUIDs = (from x in Context.OrganisationCardTemplates.Where(x => x.OrganisationUID == result.UID) select x.CardTemplateUID).ToList();
			result.UserUIDs = (from x in Context.OrganisationUsers.Where(x => x.OrganisationUID == result.UID) select x.UserUID).ToList();
			result.GuardZoneUIDs = (from x in Context.GuardZones.Where(x => x.ParentUID == result.UID) select x.ZoneUID).ToList();
			return result;
		}

		protected override void TranslateBack(DataAccess.Organisation tableItem, Organisation apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
		}

		public OperationResult<OrganisationDetails> GetDetails(Guid uid)
		{
			try
			{
				var result = new OperationResult<OrganisationDetails>();
				var tableItem = Table.Where(x => x.UID.Equals(uid)).FirstOrDefault();
				if (tableItem == null)
					return result;
				var organisationDetails = new OrganisationDetails
					{
						Description = tableItem.Description,
						IsDeleted = tableItem.IsDeleted,
						Name = tableItem.Name,
						Photo = PhotoTranslator.GetSingle(tableItem.PhotoUID).Result,
						RemovalDate = tableItem.RemovalDate,
						UID = tableItem.UID,
						DoorUIDs = (from x in Context.OrganisationDoors.Where(x => x.OrganisationUID == tableItem.UID) select x.DoorUID).ToList(),
						ZoneUIDs = (from x in Context.OrganisationZones.Where(x => x.OrganisationUID == tableItem.UID) select x.ZoneUID).ToList(),
						CardTemplateUIDs = (from x in Context.OrganisationCardTemplates.Where(x => x.OrganisationUID == tableItem.UID) select x.CardTemplateUID).ToList(),
						UserUIDs = (from x in Context.OrganisationUsers.Where(x => x.OrganisationUID == tableItem.UID) select x.UserUID).ToList(),
						GuardZoneUIDs = (from x in Context.GuardZones.Where(x => x.ParentUID == tableItem.UID) select x.ZoneUID).ToList()
					};
				var photoResult = PhotoTranslator.GetSingle(tableItem.PhotoUID);
				if (photoResult.HasError)
				{
					result.Error = photoResult.Error;
					return result;
				}
				organisationDetails.Photo = photoResult.Result;
				result.Result = organisationDetails;
				return result;
			}
			catch (Exception e)
			{
				return new OperationResult<OrganisationDetails>(e.Message);
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

		OperationResult SaveDoorsInternal(Guid organisationUID, List<Guid> doorUIDs)
		{
			try
			{
				var tableOrganisationZones = Context.OrganisationDoors.Where(x => x.OrganisationUID == organisationUID);
				Context.OrganisationDoors.DeleteAllOnSubmit(tableOrganisationZones);
				foreach (var zoneUID in doorUIDs)
				{
					var tableOrganisationZone = new DataAccess.OrganisationDoor();
					tableOrganisationZone.UID = Guid.NewGuid();
					tableOrganisationZone.OrganisationUID = organisationUID;
					tableOrganisationZone.DoorUID = zoneUID;
					Context.OrganisationDoors.InsertOnSubmit(tableOrganisationZone);
				}
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult SaveZones(Organisation apiItem)
		{
			return SaveZonesInternal(apiItem.UID, apiItem.ZoneUIDs);
		}

		public OperationResult SaveZones(OrganisationDetails apiItem)
		{
			return SaveZonesInternal(apiItem.UID, apiItem.ZoneUIDs);
		}

		OperationResult SaveZonesInternal(Guid organisationUID, List<Guid> ZoneUIDs)
		{
			try
			{
				var tableOrganisationZones = Context.OrganisationZones.Where(x => x.OrganisationUID == organisationUID);
				Context.OrganisationZones.DeleteAllOnSubmit(tableOrganisationZones);
				foreach (var zoneUID in ZoneUIDs)
				{
					var tableOrganisationZone = new DataAccess.OrganisationZone();
					tableOrganisationZone.UID = Guid.NewGuid();
					tableOrganisationZone.OrganisationUID = organisationUID;
					tableOrganisationZone.ZoneUID = zoneUID;
					Context.OrganisationZones.InsertOnSubmit(tableOrganisationZone);
				}
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult SaveCardTemplates(Organisation apiItem)
		{
			return SaveCardTemplatesInternal(apiItem.UID, apiItem.CardTemplateUIDs);
		}

		public OperationResult SaveCardTemplates(OrganisationDetails apiItem)
		{
			return SaveCardTemplatesInternal(apiItem.UID, apiItem.CardTemplateUIDs);
		}

		OperationResult SaveCardTemplatesInternal(Guid organisationUID, List<Guid> cardTemplateUIDs)
		{
			try
			{
				var tableOrganisationCardTemplates = Context.OrganisationCardTemplates.Where(x => x.OrganisationUID == organisationUID);
				Context.OrganisationCardTemplates.DeleteAllOnSubmit(tableOrganisationCardTemplates);
				foreach (var cardTemplateUID in cardTemplateUIDs)
				{
					var tableOrganisationCardTemplate = new DataAccess.OrganisationCardTemplate();
					tableOrganisationCardTemplate.UID = Guid.NewGuid();
					tableOrganisationCardTemplate.OrganisationUID = organisationUID;
					tableOrganisationCardTemplate.CardTemplateUID = cardTemplateUID;
					Context.OrganisationCardTemplates.InsertOnSubmit(tableOrganisationCardTemplate);
				}
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult SaveGuardZones(Organisation apiItem)
		{
			return SaveGuardZonesInternal(apiItem.UID, apiItem.GuardZoneUIDs);
		}

		public OperationResult SaveGuardZones(OrganisationDetails apiItem)
		{
			return SaveGuardZonesInternal(apiItem.UID, apiItem.GuardZoneUIDs);
		}

		OperationResult SaveGuardZonesInternal(Guid organisationUID, List<Guid> GuardZoneUIDs)
		{
			try
			{
				var tableOrganisationGuardZones = Context.GuardZones.Where(x => x.ParentUID == organisationUID);
				Context.GuardZones.DeleteAllOnSubmit(tableOrganisationGuardZones);
				foreach (var GuardZoneUID in GuardZoneUIDs)
				{
					var tableOrganisationGuardZone = new DataAccess.GuardZone();
					tableOrganisationGuardZone.UID = Guid.NewGuid();
					tableOrganisationGuardZone.ParentUID = organisationUID;
					tableOrganisationGuardZone.ZoneUID = GuardZoneUID;
					Context.GuardZones.InsertOnSubmit(tableOrganisationGuardZone);
				}
				Table.Context.SubmitChanges();
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

		OperationResult SaveUsersInternal(Guid organisationUID, List<Guid> UserUIDs)
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

		public OperationResult Save(OrganisationDetails apiItem)
		{
			var saveDoorsResult = SaveDoors(apiItem);
			if (saveDoorsResult.HasError)
				return saveDoorsResult;
			var saveZonesResult = SaveZones(apiItem);
			if (saveZonesResult.HasError)
				return saveZonesResult;
			var saveCardTemplatesResult = SaveCardTemplates(apiItem);
			if (saveCardTemplatesResult.HasError)
				return saveCardTemplatesResult;
			var saveGuardZonesResult = SaveGuardZones(apiItem);
			if (saveGuardZonesResult.HasError)
				return saveGuardZonesResult;
			var saveUsersResult = SaveUsers(apiItem);
			if (saveUsersResult.HasError)
				return saveUsersResult;
			if (apiItem.Photo != null)
			{
				var savePhotoResult = PhotoTranslator.Save(apiItem.Photo);
				if (savePhotoResult.HasError)
					return savePhotoResult;
			}
			try
			{
				if (apiItem == null)
					return new OperationResult("Попытка сохранить пустую запись");
				var verifyResult = CanSave(apiItem);
				if (verifyResult.HasError)
					return verifyResult;
				var tableItem = (from x in Table where x.UID.Equals(apiItem.UID) select x).FirstOrDefault();
				if (tableItem == null)
				{
					tableItem = new DataAccess.Organisation();
					tableItem.UID = apiItem.UID;
					tableItem.Name = apiItem.Name;
					tableItem.Description = apiItem.Description;
					if (apiItem.Photo != null)
						tableItem.PhotoUID = apiItem.Photo.UID;
					tableItem.IsDeleted = apiItem.IsDeleted;
					tableItem.RemovalDate = CheckDate(apiItem.RemovalDate);
					Table.InsertOnSubmit(tableItem);
				}
				else
				{
					tableItem.Name = apiItem.Name;
					tableItem.Description = apiItem.Description;
					if (apiItem.Photo != null)
						tableItem.PhotoUID = apiItem.Photo.UID;
					tableItem.IsDeleted = apiItem.IsDeleted;
					tableItem.RemovalDate = CheckDate(apiItem.RemovalDate);
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
			if(filter.UserUID != Guid.Empty)
				result = result.And(e => e.OrganisationUsers.Any(x => x.UserUID == filter.UserUID));
			return result;
		}		
	}
}