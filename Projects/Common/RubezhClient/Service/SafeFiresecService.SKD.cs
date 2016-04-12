using Common;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		#region Employee
		public OperationResult<List<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeList(FiresecServiceFactory.UID, filter);
			}, "GetEmployeeList");
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeDetails(FiresecServiceFactory.UID, uid);
			}, "GetEmployeeDetails");
		}
		public OperationResult<bool> SaveEmployee(Employee item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployee(FiresecServiceFactory.UID, item, isNew);
			}, "SaveEmployee");
		}
		public OperationResult<bool> MarkDeletedEmployee(Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedEmployee(FiresecServiceFactory.UID, uid, name, isEmployee);
			}, "MarkDeletedEmployee");
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTracks(FiresecServiceFactory.UID, filter, startDate, endDate);
			}, "GetTimeTracks");
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTracksStream(FiresecServiceFactory.UID, filter, startDate, endDate);
			}, "GetTimeTracksStream");
			return result;
		}
		public OperationResult<bool> SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployeeDepartment(FiresecServiceFactory.UID, uid, departmentUid, name);
			}, "SaveEmployeeDepartment");
		}
		public OperationResult<bool> SaveEmployeePosition(Guid uid, Guid? positionUid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployeePosition(FiresecServiceFactory.UID, uid, positionUid, name);
			}, "SaveEmployeePosition");
		}
		public OperationResult<bool> RestoreEmployee(Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreEmployee(FiresecServiceFactory.UID, uid, name, isEmployee);
			}, "RestoreEmployee");
		}
		#endregion

		#region Department
		public OperationResult<List<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetDepartmentList(FiresecServiceFactory.UID, filter);
			}, "GetDepartmentList");
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetDepartmentDetails(FiresecServiceFactory.UID, uid);
			}, "GetDepartmentDetails");
		}
		public OperationResult<bool> SaveDepartment(Department item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveDepartment(FiresecServiceFactory.UID, item, isNew);
			}, "SaveDepartment");
		}
		public OperationResult<bool> MarkDeletedDepartment(ShortDepartment item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedDepartment(FiresecServiceFactory.UID, item);
			}, "MarkDeletedDepartment");
		}
		public OperationResult<bool> SaveDepartmentChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveDepartmentChief(FiresecServiceFactory.UID, uid, chiefUID, name);
			}, "SaveDepartmentChief");
		}
		public OperationResult<bool> RestoreDepartment(ShortDepartment item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreDepartment(FiresecServiceFactory.UID, item);
			}, "RestoreDepartment");
		}
		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetChildEmployeeUIDs(FiresecServiceFactory.UID, uid);
			}, "GetChildEmployeeUIDs");
		}
		public OperationResult<List<Guid>> GetDepartmentEmployeeUIDs(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetDepartmentEmployeeUIDs(FiresecServiceFactory.UID, uid);
			}, "GetDepartmentEmployeeUIDs");
		}
		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetParentEmployeeUIDs(FiresecServiceFactory.UID, uid);
			}, "GetParentEmployeeUIDs");
		}
		#endregion

		#region Position
		public OperationResult<List<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPositionList(FiresecServiceFactory.UID, filter);
			}, "GetPositionList");
		}

		public OperationResult<List<Guid>> GetPositionEmployees(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPositionEmployees(FiresecServiceFactory.UID, uid);
			}, "GetPositionEmployees");
		}

		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPositionDetails(FiresecServiceFactory.UID, uid);
			}, "GetPositionDetails");
		}
		public OperationResult<bool> SavePosition(Position item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SavePosition(FiresecServiceFactory.UID, item, isNew);
			}, "SavePosition");
		}
		public OperationResult<bool> MarkDeletedPosition(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedPosition(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedPosition");
		}
		public OperationResult<bool> RestorePosition(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestorePosition(FiresecServiceFactory.UID, uid, name);
			}, "RestorePosition");
		}
		#endregion

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetMinJournalDateTime(FiresecServiceFactory.UID);
			}, "GetMinJournalDateTime");
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetFilteredJournalItems(FiresecServiceFactory.UID, filter);
			}, "GetFilteredJournalItems");
		}
		public OperationResult<bool> BeginGetJournal(JournalFilter filter, Guid clientUid)
		{
			return SafeOperationCall(() => 
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.BeginGetJournal(filter, FiresecServiceFactory.UID, clientUid);
			}, "BeginGetJournal");
		}
		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddJournalItem(FiresecServiceFactory.UID, journalItem);
			}, "AddJournalItem");
		}
		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page, string userName = null)
		{
			return SafeOperationCall(() => 
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.BeginGetArchivePage(filter, page, FiresecServiceFactory.UID, userName);
			}, "BeginGetArchivePage");
		}
		public OperationResult<int> GetArchiveCount(JournalFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetArchiveCount(FiresecServiceFactory.UID, filter);
			}, "GetArchiveCount");
		}
		#endregion

		#region Card
		public OperationResult<List<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetCards(FiresecServiceFactory.UID, filter);
			}, "GetCards");
		}
		public OperationResult<SKDCard> GetSingleCard(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetSingleCard(FiresecServiceFactory.UID, uid);
			}, "GetSingleCard");
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeCards(FiresecServiceFactory.UID, employeeUID);
			}, "GetEmployeeCards");
		}
		public OperationResult<bool> AddCard(SKDCard item, string employeeName)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddCard(FiresecServiceFactory.UID, item, employeeName);
			}, "AddCard");
		}
		public OperationResult<bool> EditCard(SKDCard item, string employeeName)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditCard(FiresecServiceFactory.UID, item, employeeName);
			}, "EditCard");
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteCardFromEmployee(FiresecServiceFactory.UID, item, employeeName, reason);
			}, "DeleteCardFromEmployee");
		}
		public OperationResult<bool> DeletedCard(SKDCard card)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeletedCard(FiresecServiceFactory.UID, card);
			}, "DeletedCard");
		}
		public OperationResult<bool> SaveCardTemplate(SKDCard item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveCardTemplate(FiresecServiceFactory.UID, item);
			}, "SaveCardTemplate");
		}
		public OperationResult<List<GKUser>> GetDbDeviceUsers(Guid deviceUID, List<Guid> doorUIDs)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetDbDeviceUsers(deviceUID, doorUIDs);
			}, "GetDbDeviceUsers");
		}
		#endregion

		#region AccessTemplate
		public OperationResult<List<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetAccessTemplates(FiresecServiceFactory.UID, filter);
			}, "GetAccessTemplates");
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveAccessTemplate(FiresecServiceFactory.UID, item, isNew);
			}, "SaveAccessTemplate");
		}
		public OperationResult<List<string>> MarkDeletedAccessTemplate(AccessTemplate item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedAccessTemplate(FiresecServiceFactory.UID, item);
			}, "MarkDeletedAccessTemplate");
		}
		public OperationResult<bool> RestoreAccessTemplate(AccessTemplate item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreAccessTemplate(FiresecServiceFactory.UID, item);
			}, "RestoreAccessTemplate");
		}
		#endregion

		#region Organisation
		public OperationResult<List<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetOrganisations(FiresecServiceFactory.UID, filter);
			}, "GetOrganisations");
		}
		public OperationResult<bool> SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisation(FiresecServiceFactory.UID, item, isNew);
			}, "SaveOrganisation");
		}
		public OperationResult<bool> MarkDeletedOrganisation(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedOrganisation(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedOrganisation");
		}
		public OperationResult<bool> AddOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddOrganisationDoor(FiresecServiceFactory.UID, item, doorUID);
			}, "AddOrganisationDoor");
		}
		public OperationResult<bool> RemoveOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RemoveOrganisationDoor(FiresecServiceFactory.UID, item, doorUID);
			}, "RemoveOrganisationDoor");
		}
		public OperationResult<bool> SaveOrganisationUsers(Organisation item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationUsers(FiresecServiceFactory.UID, item);
			}, "SaveOrganisationUsers");
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetOrganisationDetails(FiresecServiceFactory.UID, uid);
			}, "GetOrganisationDetails");
		}
		public OperationResult<bool> SaveOrganisationChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationChief(FiresecServiceFactory.UID, uid, chiefUID, name);
			}, "SaveOrganisationChief");
		}
		public OperationResult<bool> SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationHRChief(FiresecServiceFactory.UID, uid, chiefUID, name);
			}, "SaveOrganisationHRChief");
		}
		public OperationResult<bool> RestoreOrganisation(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreOrganisation(FiresecServiceFactory.UID, uid, name);
			}, "RestoreOrganisation");
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.IsAnyOrganisationItems(FiresecServiceFactory.UID, uid);
			}, "IsAnyOrganisationItems");
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetAdditionalColumnTypes(FiresecServiceFactory.UID, filter);
			}, "GetAdditionalColumnTypes");
		}
		public OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveAdditionalColumnType(FiresecServiceFactory.UID, item, isNew);
			}, "SaveAdditionalColumnType");
		}
		public OperationResult<bool> MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedAdditionalColumnType(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedAdditionalColumnType");
		}
		public OperationResult<bool> RestoreAdditionalColumnType(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreAdditionalColumnType(FiresecServiceFactory.UID, uid, name);
			}, "RestoreAdditionalColumnType");
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetNightSettingsByOrganisation(FiresecServiceFactory.UID, organisationUID);
			}, "GetNightSettingsByOrganisation");
		}
		public OperationResult<bool> SaveNightSettings(NightSettings nightSettings)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveNightSettings(FiresecServiceFactory.UID, nightSettings);
			}, "SaveNightSettings");
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPassCardTemplateList(FiresecServiceFactory.UID, filter);
			}, "GetPassCardTemplateList");
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPassCardTemplateDetails(FiresecServiceFactory.UID, uid);
			}, "GetPassCardTemplateDetails");
		}
		public OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SavePassCardTemplate(FiresecServiceFactory.UID, item, isNew);
			}, "SavePassCardTemplate");
		}
		public OperationResult<bool> MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedPassCardTemplate(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedPassCardTemplate");
		}
		public OperationResult<bool> RestorePassCardTemplate(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestorePassCardTemplate(FiresecServiceFactory.UID, uid, name);
			}, "RestorePassCardTemplate");
		}
		#endregion

		public OperationResult<bool> GenerateEmployeeDays()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GenerateEmployeeDays(FiresecServiceFactory.UID);
			}, "GenerateEmployeeDays");
		}

		public OperationResult<bool> GenerateJournal()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GenerateJournal(FiresecServiceFactory.UID);
			}, "GenerateJournal");
		}

		public OperationResult<bool> GenerateTestData(bool isAscending)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GenerateTestData(FiresecServiceFactory.UID, isAscending);
			}, "GenerateTestData");
		}

		public OperationResult<bool> SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveJournalVideoUID(FiresecServiceFactory.UID, journalItemUID, videoUID, cameraUID);
			}, "SaveJournalVideoUID");
		}

		public OperationResult<bool> SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveJournalCameraUID(FiresecServiceFactory.UID, journalItemUID, CameraUID);
			}, "SaveJournalCameraUID");
		}

		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetGKSchedules(FiresecServiceFactory.UID);
			}, "GetGKSchedules");
		}

		public OperationResult<bool> SaveGKSchedule(GKSchedule item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveGKSchedule(FiresecServiceFactory.UID, item, isNew);
			}, "SaveGKSchedule");
		}

		public OperationResult<bool> DeleteGKSchedule(GKSchedule item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteGKSchedule(FiresecServiceFactory.UID, item);
			}, "DeleteGKSchedule");
		}
		#endregion

		#region GKDaySchedule
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetGKDaySchedules(FiresecServiceFactory.UID);
			}, "GetGKDaySchedules");
		}

		public OperationResult<bool> SaveGKDaySchedule(GKDaySchedule item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveGKDaySchedule(FiresecServiceFactory.UID, item, isNew);
			}, "SaveGKDaySchedule");
		}

		public OperationResult<bool> DeleteGKDaySchedule(GKDaySchedule item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteGKDaySchedule(FiresecServiceFactory.UID, item);
			}, "DeleteGKDaySchedule");
		}
		#endregion

		#region Export
		public OperationResult<bool> ExportOrganisation(ExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportOrganisation(FiresecServiceFactory.UID, filter);
			}, "ExportOrganisation");
		}
		public OperationResult<bool> ImportOrganisation(ImportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ImportOrganisation(FiresecServiceFactory.UID, filter);
			}, "ImportOrganisation");
		}
		public OperationResult<bool> ExportOrganisationList(ExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportOrganisationList(FiresecServiceFactory.UID, filter);
			}, "ExportOrganisationList");
		}
		public OperationResult<bool> ImportOrganisationList(ImportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ImportOrganisationList(FiresecServiceFactory.UID, filter);
			}, "ImportOrganisationList");
		}
		public OperationResult<bool> ExportJournal(JournalExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportJournal(FiresecServiceFactory.UID, filter);
			}, "ExportJournal");
		}
		public OperationResult<bool> ExportConfiguration(ConfigurationExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportConfiguration(FiresecServiceFactory.UID, filter);
			}, "ExportConfiguration");
		}
		#endregion

		#region CurrentConsumption
		public OperationResult<bool> SaveCurrentConsumption(CurrentConsumption item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveCurrentConsumption(FiresecServiceFactory.UID, item);
			}, "SaveCurrentConsumption");
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetCurrentConsumption(FiresecServiceFactory.UID, item);
			}, "GetCurrentConsumption");
		}
		#endregion
	}
}