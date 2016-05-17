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
	public partial class SafeRubezhService
	{
		#region Employee
		public OperationResult<List<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetEmployeeList(RubezhServiceFactory.UID, filter);
			}, "GetEmployeeList");
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetEmployeeDetails(RubezhServiceFactory.UID, uid);
			}, "GetEmployeeDetails");
		}
		public OperationResult<bool> SaveEmployee(Employee item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveEmployee(RubezhServiceFactory.UID, item, isNew);
			}, "SaveEmployee");
		}
		public OperationResult<bool> MarkDeletedEmployee(Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedEmployee(RubezhServiceFactory.UID, uid, name, isEmployee);
			}, "MarkDeletedEmployee");
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetTimeTracks(RubezhServiceFactory.UID, filter, startDate, endDate);
			}, "GetTimeTracks");
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetTimeTracksStream(RubezhServiceFactory.UID, filter, startDate, endDate);
			}, "GetTimeTracksStream");
			return result;
		}
		public OperationResult<bool> SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveEmployeeDepartment(RubezhServiceFactory.UID, uid, departmentUid, name);
			}, "SaveEmployeeDepartment");
		}
		public OperationResult<bool> SaveEmployeePosition(Guid uid, Guid? positionUid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveEmployeePosition(RubezhServiceFactory.UID, uid, positionUid, name);
			}, "SaveEmployeePosition");
		}
		public OperationResult<bool> RestoreEmployee(Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestoreEmployee(RubezhServiceFactory.UID, uid, name, isEmployee);
			}, "RestoreEmployee");
		}
		#endregion

		#region Department
		public OperationResult<List<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetDepartmentList(RubezhServiceFactory.UID, filter);
			}, "GetDepartmentList");
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetDepartmentDetails(RubezhServiceFactory.UID, uid);
			}, "GetDepartmentDetails");
		}
		public OperationResult<bool> SaveDepartment(Department item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveDepartment(RubezhServiceFactory.UID, item, isNew);
			}, "SaveDepartment");
		}
		public OperationResult<bool> MarkDeletedDepartment(ShortDepartment item)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedDepartment(RubezhServiceFactory.UID, item);
			}, "MarkDeletedDepartment");
		}
		public OperationResult<bool> SaveDepartmentChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveDepartmentChief(RubezhServiceFactory.UID, uid, chiefUID, name);
			}, "SaveDepartmentChief");
		}
		public OperationResult<bool> RestoreDepartment(ShortDepartment item)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestoreDepartment(RubezhServiceFactory.UID, item);
			}, "RestoreDepartment");
		}
		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetChildEmployeeUIDs(RubezhServiceFactory.UID, uid);
			}, "GetChildEmployeeUIDs");
		}
		public OperationResult<List<Guid>> GetDepartmentEmployeeUIDs(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetDepartmentEmployeeUIDs(RubezhServiceFactory.UID, uid);
			}, "GetDepartmentEmployeeUIDs");
		}
		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetParentEmployeeUIDs(RubezhServiceFactory.UID, uid);
			}, "GetParentEmployeeUIDs");
		}
		#endregion

		#region Position
		public OperationResult<List<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetPositionList(RubezhServiceFactory.UID, filter);
			}, "GetPositionList");
		}

		public OperationResult<List<Guid>> GetPositionEmployees(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetPositionEmployees(RubezhServiceFactory.UID, uid);
			}, "GetPositionEmployees");
		}

		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetPositionDetails(RubezhServiceFactory.UID, uid);
			}, "GetPositionDetails");
		}
		public OperationResult<bool> SavePosition(Position item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SavePosition(RubezhServiceFactory.UID, item, isNew);
			}, "SavePosition");
		}
		public OperationResult<bool> MarkDeletedPosition(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedPosition(RubezhServiceFactory.UID, uid, name);
			}, "MarkDeletedPosition");
		}
		public OperationResult<bool> RestorePosition(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestorePosition(RubezhServiceFactory.UID, uid, name);
			}, "RestorePosition");
		}
		#endregion

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetMinJournalDateTime(RubezhServiceFactory.UID);
			}, "GetMinJournalDateTime");
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetFilteredJournalItems(RubezhServiceFactory.UID, filter);
			}, "GetFilteredJournalItems");
		}
		public OperationResult<bool> BeginGetJournal(JournalFilter filter, Guid clientUid)
		{
			return SafeOperationCall(() => 
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.BeginGetJournal(filter, RubezhServiceFactory.UID, clientUid);
			}, "BeginGetJournal");
		}
		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.AddJournalItem(RubezhServiceFactory.UID, journalItem);
			}, "AddJournalItem");
		}
		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page, string userName = null)
		{
			return SafeOperationCall(() => 
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.BeginGetArchivePage(filter, page, RubezhServiceFactory.UID, userName);
			}, "BeginGetArchivePage");
		}
		public OperationResult<int> GetArchiveCount(JournalFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetArchiveCount(RubezhServiceFactory.UID, filter);
			}, "GetArchiveCount");
		}
		#endregion

		#region Card
		public OperationResult<List<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetCards(RubezhServiceFactory.UID, filter);
			}, "GetCards");
		}
		public OperationResult<SKDCard> GetSingleCard(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetSingleCard(RubezhServiceFactory.UID, uid);
			}, "GetSingleCard");
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetEmployeeCards(RubezhServiceFactory.UID, employeeUID);
			}, "GetEmployeeCards");
		}
		public OperationResult<bool> AddCard(SKDCard item, string employeeName)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.AddCard(RubezhServiceFactory.UID, item, employeeName);
			}, "AddCard");
		}
		public OperationResult<bool> EditCard(SKDCard item, string employeeName)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.EditCard(RubezhServiceFactory.UID, item, employeeName);
			}, "EditCard");
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.DeleteCardFromEmployee(RubezhServiceFactory.UID, item, employeeName, reason);
			}, "DeleteCardFromEmployee");
		}
		public OperationResult<bool> DeletedCard(SKDCard card)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.DeletedCard(RubezhServiceFactory.UID, card);
			}, "DeletedCard");
		}
		public OperationResult<bool> SaveCardTemplate(SKDCard item)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveCardTemplate(RubezhServiceFactory.UID, item);
			}, "SaveCardTemplate");
		}
		public OperationResult<List<GKUser>> GetDbDeviceUsers(Guid deviceUID, List<Guid> doorUIDs)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetDbDeviceUsers(deviceUID, doorUIDs);
			}, "GetDbDeviceUsers");
		}
		#endregion

		#region AccessTemplate
		public OperationResult<List<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetAccessTemplates(RubezhServiceFactory.UID, filter);
			}, "GetAccessTemplates");
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveAccessTemplate(RubezhServiceFactory.UID, item, isNew);
			}, "SaveAccessTemplate");
		}
		public OperationResult<List<string>> MarkDeletedAccessTemplate(AccessTemplate item)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedAccessTemplate(RubezhServiceFactory.UID, item);
			}, "MarkDeletedAccessTemplate");
		}
		public OperationResult<bool> RestoreAccessTemplate(AccessTemplate item)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestoreAccessTemplate(RubezhServiceFactory.UID, item);
			}, "RestoreAccessTemplate");
		}
		#endregion

		#region Organisation
		public OperationResult<List<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetOrganisations(RubezhServiceFactory.UID, filter);
			}, "GetOrganisations");
		}
		public OperationResult<bool> SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveOrganisation(RubezhServiceFactory.UID, item, isNew);
			}, "SaveOrganisation");
		}
		public OperationResult<bool> MarkDeletedOrganisation(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedOrganisation(RubezhServiceFactory.UID, uid, name);
			}, "MarkDeletedOrganisation");
		}
		public OperationResult<bool> AddOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.AddOrganisationDoor(RubezhServiceFactory.UID, item, doorUID);
			}, "AddOrganisationDoor");
		}
		public OperationResult<bool> RemoveOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RemoveOrganisationDoor(RubezhServiceFactory.UID, item, doorUID);
			}, "RemoveOrganisationDoor");
		}
		public OperationResult<bool> SaveOrganisationUsers(Organisation item)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveOrganisationUsers(RubezhServiceFactory.UID, item);
			}, "SaveOrganisationUsers");
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetOrganisationDetails(RubezhServiceFactory.UID, uid);
			}, "GetOrganisationDetails");
		}
		public OperationResult<bool> SaveOrganisationChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveOrganisationChief(RubezhServiceFactory.UID, uid, chiefUID, name);
			}, "SaveOrganisationChief");
		}
		public OperationResult<bool> SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveOrganisationHRChief(RubezhServiceFactory.UID, uid, chiefUID, name);
			}, "SaveOrganisationHRChief");
		}
		public OperationResult<bool> RestoreOrganisation(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestoreOrganisation(RubezhServiceFactory.UID, uid, name);
			}, "RestoreOrganisation");
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.IsAnyOrganisationItems(RubezhServiceFactory.UID, uid);
			}, "IsAnyOrganisationItems");
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetAdditionalColumnTypes(RubezhServiceFactory.UID, filter);
			}, "GetAdditionalColumnTypes");
		}
		public OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveAdditionalColumnType(RubezhServiceFactory.UID, item, isNew);
			}, "SaveAdditionalColumnType");
		}
		public OperationResult<bool> MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedAdditionalColumnType(RubezhServiceFactory.UID, uid, name);
			}, "MarkDeletedAdditionalColumnType");
		}
		public OperationResult<bool> RestoreAdditionalColumnType(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestoreAdditionalColumnType(RubezhServiceFactory.UID, uid, name);
			}, "RestoreAdditionalColumnType");
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetNightSettingsByOrganisation(RubezhServiceFactory.UID, organisationUID);
			}, "GetNightSettingsByOrganisation");
		}
		public OperationResult<bool> SaveNightSettings(NightSettings nightSettings)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveNightSettings(RubezhServiceFactory.UID, nightSettings);
			}, "SaveNightSettings");
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetPassCardTemplateList(RubezhServiceFactory.UID, filter);
			}, "GetPassCardTemplateList");
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetPassCardTemplateDetails(RubezhServiceFactory.UID, uid);
			}, "GetPassCardTemplateDetails");
		}
		public OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SavePassCardTemplate(RubezhServiceFactory.UID, item, isNew);
			}, "SavePassCardTemplate");
		}
		public OperationResult<bool> MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedPassCardTemplate(RubezhServiceFactory.UID, uid, name);
			}, "MarkDeletedPassCardTemplate");
		}
		public OperationResult<bool> RestorePassCardTemplate(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestorePassCardTemplate(RubezhServiceFactory.UID, uid, name);
			}, "RestorePassCardTemplate");
		}
		#endregion

		public OperationResult<bool> GenerateEmployeeDays()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GenerateEmployeeDays(RubezhServiceFactory.UID);
			}, "GenerateEmployeeDays");
		}

		public OperationResult<bool> GenerateJournal()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GenerateJournal(RubezhServiceFactory.UID);
			}, "GenerateJournal");
		}

		public OperationResult<bool> GenerateTestData(bool isAscending)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GenerateTestData(RubezhServiceFactory.UID, isAscending);
			}, "GenerateTestData");
		}

		public OperationResult<bool> SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveJournalVideoUID(RubezhServiceFactory.UID, journalItemUID, videoUID, cameraUID);
			}, "SaveJournalVideoUID");
		}

		public OperationResult<bool> SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveJournalCameraUID(RubezhServiceFactory.UID, journalItemUID, CameraUID);
			}, "SaveJournalCameraUID");
		}

		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetGKSchedules(RubezhServiceFactory.UID);
			}, "GetGKSchedules");
		}

		public OperationResult<bool> SaveGKSchedule(GKSchedule item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveGKSchedule(RubezhServiceFactory.UID, item, isNew);
			}, "SaveGKSchedule");
		}

		public OperationResult<bool> DeleteGKSchedule(GKSchedule item)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.DeleteGKSchedule(RubezhServiceFactory.UID, item);
			}, "DeleteGKSchedule");
		}
		#endregion

		#region GKDaySchedule
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetGKDaySchedules(RubezhServiceFactory.UID);
			}, "GetGKDaySchedules");
		}

		public OperationResult<bool> SaveGKDaySchedule(GKDaySchedule item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveGKDaySchedule(RubezhServiceFactory.UID, item, isNew);
			}, "SaveGKDaySchedule");
		}

		public OperationResult<bool> DeleteGKDaySchedule(GKDaySchedule item)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.DeleteGKDaySchedule(RubezhServiceFactory.UID, item);
			}, "DeleteGKDaySchedule");
		}
		#endregion

		#region Export
		public OperationResult<bool> ExportOrganisation(ExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.ExportOrganisation(RubezhServiceFactory.UID, filter);
			}, "ExportOrganisation");
		}
		public OperationResult<bool> ImportOrganisation(ImportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.ImportOrganisation(RubezhServiceFactory.UID, filter);
			}, "ImportOrganisation");
		}
		public OperationResult<bool> ExportOrganisationList(ExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.ExportOrganisationList(RubezhServiceFactory.UID, filter);
			}, "ExportOrganisationList");
		}
		public OperationResult<bool> ImportOrganisationList(ImportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.ImportOrganisationList(RubezhServiceFactory.UID, filter);
			}, "ImportOrganisationList");
		}
		public OperationResult<bool> ExportJournal(JournalExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.ExportJournal(RubezhServiceFactory.UID, filter);
			}, "ExportJournal");
		}
		public OperationResult<bool> ExportConfiguration(ConfigurationExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.ExportConfiguration(RubezhServiceFactory.UID, filter);
			}, "ExportConfiguration");
		}
		#endregion

		#region CurrentConsumption
		public OperationResult<bool> SaveCurrentConsumption(CurrentConsumption item)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveCurrentConsumption(RubezhServiceFactory.UID, item);
			}, "SaveCurrentConsumption");
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter item)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetCurrentConsumption(RubezhServiceFactory.UID, item);
			}, "GetCurrentConsumption");
		}
		#endregion
	}
}