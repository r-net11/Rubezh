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
			return SafeOperationCall<OperationResult<List<ShortEmployee>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeList(filter, FiresecServiceFactory.UID);
			}, "GetEmployeeList");
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SafeOperationCall<OperationResult<Employee>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeDetails(uid, FiresecServiceFactory.UID);
			}, "GetEmployeeDetails");
		}
		public OperationResult<bool> SaveEmployee(Employee item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployee(item, isNew, FiresecServiceFactory.UID);
			}, "SaveEmployee");
		}
		public OperationResult MarkDeletedEmployee(Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedEmployee(uid, name, isEmployee, FiresecServiceFactory.UID);
			}, "MarkDeletedEmployee");
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTracks(filter, startDate, endDate, FiresecServiceFactory.UID);
			}, "GetTimeTracks");
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeOperationCall<Stream>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTracksStream(filter, startDate, endDate, FiresecServiceFactory.UID);
			}, "GetTimeTracksStream");
			return result;
		}
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployeeDepartment(uid, departmentUid, name, FiresecServiceFactory.UID);
			}, "SaveEmployeeDepartment");
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid? positionUid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployeePosition(uid, positionUid, name, FiresecServiceFactory.UID);
			}, "SaveEmployeePosition");
		}
		public OperationResult RestoreEmployee(Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreEmployee(uid, name, isEmployee, FiresecServiceFactory.UID);
			}, "RestoreEmployee");
		}
		#endregion

		#region Department
		public OperationResult<List<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			return SafeOperationCall<OperationResult<List<ShortDepartment>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetDepartmentList(filter, FiresecServiceFactory.UID);
			}, "GetDepartmentList");
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			return SafeOperationCall<OperationResult<Department>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetDepartmentDetails(uid, FiresecServiceFactory.UID);
			}, "GetDepartmentDetails");
		}
		public OperationResult<bool> SaveDepartment(Department item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveDepartment(item, isNew, FiresecServiceFactory.UID);
			}, "SaveDepartment");
		}
		public OperationResult MarkDeletedDepartment(ShortDepartment item)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedDepartment(item, FiresecServiceFactory.UID);
			}, "MarkDeletedDepartment");
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveDepartmentChief(uid, chiefUID, name, FiresecServiceFactory.UID);
			}, "SaveDepartmentChief");
		}
		public OperationResult RestoreDepartment(ShortDepartment item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreDepartment(item, FiresecServiceFactory.UID);
			}, "RestoreDepartment");
		}
		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			return SafeOperationCall<OperationResult<List<Guid>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetChildEmployeeUIDs(uid, FiresecServiceFactory.UID);
			}, "GetChildEmployeeUIDs");
		}
		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			return SafeOperationCall<OperationResult<List<Guid>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetParentEmployeeUIDs(uid, FiresecServiceFactory.UID);
			}, "GetParentEmployeeUIDs");
		}
		#endregion

		#region Position
		public OperationResult<List<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			return SafeOperationCall<OperationResult<List<ShortPosition>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPositionList(filter, FiresecServiceFactory.UID);
			}, "GetPositionList");
		}

		public OperationResult<List<Guid>> GetPositionEmployees(Guid uid)
		{
			return SafeOperationCall<OperationResult<List<Guid>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPositionEmployees(uid, FiresecServiceFactory.UID);
			}, "GetPositionEmployees");
		}

		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			return SafeOperationCall<OperationResult<Position>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPositionDetails(uid, FiresecServiceFactory.UID);
			}, "GetPositionDetails");
		}
		public OperationResult<bool> SavePosition(Position item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SavePosition(item, isNew, FiresecServiceFactory.UID);
			}, "SavePosition");
		}
		public OperationResult MarkDeletedPosition(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedPosition(uid, name, FiresecServiceFactory.UID);
			}, "MarkDeletedPosition");
		}
		public OperationResult RestorePosition(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestorePosition(uid, name, FiresecServiceFactory.UID);
			}, "RestorePosition");
		}
		#endregion

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			return SafeOperationCall<OperationResult<DateTime>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetMinJournalDateTime(FiresecServiceFactory.UID);
			}, "GetMinJournalDateTime");
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return SafeOperationCall<OperationResult<List<JournalItem>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetFilteredJournalItems(filter, FiresecServiceFactory.UID);
			}, "GetFilteredJournalItems");
		}
		public OperationResult<bool> BeginGetJournal(JournalFilter filter)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.BeginGetJournal(filter, FiresecServiceFactory.UID);
			}, "BeginGetJournal");
		}
		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddJournalItem(journalItem, FiresecServiceFactory.UID);
			}, "AddJournalItem");
		}
		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.BeginGetArchivePage(filter, page, FiresecServiceFactory.UID);
			}, "BeginGetArchivePage");
		}
		public OperationResult<int> GetArchiveCount(JournalFilter filter)
		{
			return SafeOperationCall<OperationResult<int>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetArchiveCount(filter, FiresecServiceFactory.UID);
			}, "GetArchiveCount");
		}
		#endregion

		#region Card
		public OperationResult<List<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeOperationCall<OperationResult<List<SKDCard>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetCards(filter, FiresecServiceFactory.UID);
			}, "GetCards");
		}
		public OperationResult<SKDCard> GetSingleCard(Guid uid)
		{
			return SafeOperationCall<OperationResult<SKDCard>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetSingleCard(uid, FiresecServiceFactory.UID);
			}, "GetSingleCard");
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			return SafeOperationCall<OperationResult<List<SKDCard>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeCards(employeeUID, FiresecServiceFactory.UID);
			}, "GetEmployeeCards");
		}
		public OperationResult<bool> AddCard(SKDCard item, string employeeName)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddCard(item, employeeName, FiresecServiceFactory.UID);
			}, "AddCard");
		}
		public OperationResult<bool> EditCard(SKDCard item, string employeeName)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditCard(item, employeeName, FiresecServiceFactory.UID);
			}, "EditCard");
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteCardFromEmployee(item, employeeName, FiresecServiceFactory.UID, reason);
			}, "DeleteCardFromEmployee");
		}
		public OperationResult DeletedCard(SKDCard card)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeletedCard(card, FiresecServiceFactory.UID);
			}, "DeletedCard");
		}
		public OperationResult SaveCardTemplate(SKDCard item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveCardTemplate(item, FiresecServiceFactory.UID);
			}, "SaveCardTemplate");
		}
		#endregion

		#region AccessTemplate
		public OperationResult<List<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SafeOperationCall<OperationResult<List<AccessTemplate>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetAccessTemplates(filter, FiresecServiceFactory.UID);
			}, "GetAccessTemplates");
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveAccessTemplate(item, isNew, FiresecServiceFactory.UID);
			}, "SaveAccessTemplate");
		}
		public OperationResult MarkDeletedAccessTemplate(AccessTemplate item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedAccessTemplate(item, FiresecServiceFactory.UID);
			}, "MarkDeletedAccessTemplate");
		}
		public OperationResult RestoreAccessTemplate(AccessTemplate item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreAccessTemplate(item, FiresecServiceFactory.UID);
			}, "RestoreAccessTemplate");
		}
		#endregion

		#region Organisation
		public OperationResult<List<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SafeOperationCall<OperationResult<List<Organisation>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetOrganisations(filter, FiresecServiceFactory.UID);
			}, "GetOrganisations");
		}
		public OperationResult<bool> SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisation(item, isNew, FiresecServiceFactory.UID);
			}, "SaveOrganisation");
		}
		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedOrganisation(uid, name, FiresecServiceFactory.UID);
			}, "MarkDeletedOrganisation");
		}
		public OperationResult AddOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddOrganisationDoor(item, doorUID, FiresecServiceFactory.UID);
			}, "AddOrganisationDoor");
		}
		public OperationResult RemoveOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RemoveOrganisationDoor(item, doorUID, FiresecServiceFactory.UID);
			}, "RemoveOrganisationDoor");
		}
		public OperationResult SaveOrganisationUsers(Organisation item)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationUsers(item, FiresecServiceFactory.UID);
			}, "SaveOrganisationUsers");
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SafeOperationCall<OperationResult<OrganisationDetails>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetOrganisationDetails(uid, FiresecServiceFactory.UID);
			}, "GetOrganisationDetails");
		}
		public OperationResult SaveOrganisationChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationChief(uid, chiefUID, name, FiresecServiceFactory.UID);
			}, "SaveOrganisationChief");
		}
		public OperationResult SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationHRChief(uid, chiefUID, name, FiresecServiceFactory.UID);
			}, "SaveOrganisationHRChief");
		}
		public OperationResult RestoreOrganisation(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreOrganisation(uid, name, FiresecServiceFactory.UID);
			}, "RestoreOrganisation");
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.IsAnyOrganisationItems(uid, FiresecServiceFactory.UID);
			}, "IsAnyOrganisationItems");
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SafeOperationCall<OperationResult<List<AdditionalColumnType>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetAdditionalColumnTypes(filter, FiresecServiceFactory.UID);
			}, "GetAdditionalColumnTypes");
		}
		public OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveAdditionalColumnType(item, isNew, FiresecServiceFactory.UID);
			}, "SaveAdditionalColumnType");
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedAdditionalColumnType(uid, name, FiresecServiceFactory.UID);
			}, "MarkDeletedAdditionalColumnType");
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreAdditionalColumnType(uid, name, FiresecServiceFactory.UID);
			}, "RestoreAdditionalColumnType");
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			return SafeOperationCall<OperationResult<NightSettings>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetNightSettingsByOrganisation(organisationUID, FiresecServiceFactory.UID);
			}, "GetNightSettingsByOrganisation");
		}
		public OperationResult SaveNightSettings(NightSettings nightSettings)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveNightSettings(nightSettings, FiresecServiceFactory.UID);
			}, "SaveNightSettings");
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter)
		{
			return SafeOperationCall<OperationResult<List<ShortPassCardTemplate>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPassCardTemplateList(filter, FiresecServiceFactory.UID);
			}, "GetPassCardTemplateList");
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			return SafeOperationCall<OperationResult<PassCardTemplate>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPassCardTemplateDetails(uid, FiresecServiceFactory.UID);
			}, "GetPassCardTemplateDetails");
		}
		public OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SavePassCardTemplate(item, isNew, FiresecServiceFactory.UID);
			}, "SavePassCardTemplate");
		}
		public OperationResult MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedPassCardTemplate(uid, name, FiresecServiceFactory.UID);
			}, "MarkDeletedPassCardTemplate");
		}
		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestorePassCardTemplate(uid, name, FiresecServiceFactory.UID);
			}, "RestorePassCardTemplate");
		}
		#endregion

		public OperationResult GenerateEmployeeDays()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GenerateEmployeeDays(FiresecServiceFactory.UID);
			}, "GenerateEmployeeDays");
		}

		public OperationResult GenerateJournal()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GenerateJournal(FiresecServiceFactory.UID);
			}, "GenerateJournal");
		}

		public OperationResult GenerateTestData(bool isAscending)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GenerateTestData(isAscending, FiresecServiceFactory.UID);
			}, "GenerateTestData");
		}

		public OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveJournalVideoUID(journalItemUID, videoUID, cameraUID, FiresecServiceFactory.UID);
			}, "SaveJournalVideoUID");
		}

		public OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveJournalCameraUID(journalItemUID, CameraUID, FiresecServiceFactory.UID);
			}, "SaveJournalCameraUID");
		}

		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules()
		{
			return SafeOperationCall<OperationResult<List<GKSchedule>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetGKSchedules(FiresecServiceFactory.UID);
			}, "GetGKSchedules");
		}

		public OperationResult<bool> SaveGKSchedule(GKSchedule item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveGKSchedule(item, isNew, FiresecServiceFactory.UID);
			}, "SaveGKSchedule");
		}

		public OperationResult<bool> DeleteGKSchedule(GKSchedule item)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteGKSchedule(item, FiresecServiceFactory.UID);
			}, "DeleteGKSchedule");
		}
		#endregion

		#region GKDaySchedule
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules()
		{
			return SafeOperationCall<OperationResult<List<GKDaySchedule>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetGKDaySchedules(FiresecServiceFactory.UID);
			}, "GetGKDaySchedules");
		}

		public OperationResult<bool> SaveGKDaySchedule(GKDaySchedule item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveGKDaySchedule(item, isNew, FiresecServiceFactory.UID);
			}, "SaveGKDaySchedule");
		}

		public OperationResult<bool> DeleteGKDaySchedule(GKDaySchedule item)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteGKDaySchedule(item, FiresecServiceFactory.UID);
			}, "DeleteGKDaySchedule");
		}
		#endregion

		#region Export
		public OperationResult ExportOrganisation(ExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportOrganisation(filter, FiresecServiceFactory.UID);
			}, "ExportOrganisation");
		}
		public OperationResult ImportOrganisation(ImportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ImportOrganisation(filter, FiresecServiceFactory.UID);
			}, "ImportOrganisation");
		}
		public OperationResult ExportOrganisationList(ExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportOrganisationList(filter, FiresecServiceFactory.UID);
			}, "ExportOrganisationList");
		}
		public OperationResult ImportOrganisationList(ImportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ImportOrganisationList(filter, FiresecServiceFactory.UID);
			}, "ImportOrganisationList");
		}
		public OperationResult ExportJournal(JournalExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportJournal(filter, FiresecServiceFactory.UID);
			}, "ExportJournal");
		}
		public OperationResult ExportConfiguration(ConfigurationExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportConfiguration(filter, FiresecServiceFactory.UID);
			}, "ExportConfiguration");
		}
		#endregion

		#region CurrentConsumption
		public OperationResult SaveCurrentConsumption(CurrentConsumption item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveCurrentConsumption(item, FiresecServiceFactory.UID);
			}, "SaveCurrentConsumption");
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetCurrentConsumption(item, FiresecServiceFactory.UID);
			}, "GetCurrentConsumption");
		}
		#endregion
	}
}