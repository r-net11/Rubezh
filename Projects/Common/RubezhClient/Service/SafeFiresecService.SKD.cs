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
					return firesecService.GetEmployeeList(filter);
			}, "GetEmployeeList");
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SafeOperationCall<OperationResult<Employee>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeDetails(uid);
			}, "GetEmployeeDetails");
		}
		public OperationResult<bool> SaveEmployee(Employee item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployee(item, isNew);
			}, "SaveEmployee");
		}
		public OperationResult MarkDeletedEmployee(Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedEmployee(uid, name, isEmployee);
			}, "MarkDeletedEmployee");
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTracks(filter, startDate, endDate);
			}, "GetTimeTracks");
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeOperationCall<Stream>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTracksStream(filter, startDate, endDate);
			}, "GetTimeTracksStream");
			return result;
		}
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployeeDepartment(uid, departmentUid, name);
			}, "SaveEmployeeDepartment");
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid? positionUid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployeePosition(uid, positionUid, name);
			}, "SaveEmployeePosition");
		}
		public OperationResult RestoreEmployee(Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreEmployee(uid, name, isEmployee);
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
					return firesecService.GetDepartmentList(filter);
			}, "GetDepartmentList");
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			return SafeOperationCall<OperationResult<Department>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetDepartmentDetails(uid);
			}, "GetDepartmentDetails");
		}
		public OperationResult<bool> SaveDepartment(Department item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveDepartment(item, isNew);
			}, "SaveDepartment");
		}
		public OperationResult MarkDeletedDepartment(ShortDepartment item)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedDepartment(item);
			}, "MarkDeletedDepartment");
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveDepartmentChief(uid, chiefUID, name);
			}, "SaveDepartmentChief");
		}
		public OperationResult RestoreDepartment(ShortDepartment item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreDepartment(item);
			}, "RestoreDepartment");
		}
		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			return SafeOperationCall<OperationResult<List<Guid>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetChildEmployeeUIDs(uid);
			}, "GetChildEmployeeUIDs");
		}
		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			return SafeOperationCall<OperationResult<List<Guid>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetParentEmployeeUIDs(uid);
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
					return firesecService.GetPositionList(filter);
			}, "GetPositionList");
		}

		public OperationResult<List<Guid>> GetPositionEmployees(Guid uid)
		{
			return SafeOperationCall<OperationResult<List<Guid>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPositionEmployees(uid);
			}, "GetPositionEmployees");
		}

		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			return SafeOperationCall<OperationResult<Position>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPositionDetails(uid);
			}, "GetPositionDetails");
		}
		public OperationResult<bool> SavePosition(Position item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SavePosition(item, isNew);
			}, "SavePosition");
		}
		public OperationResult MarkDeletedPosition(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedPosition(uid, name);
			}, "MarkDeletedPosition");
		}
		public OperationResult RestorePosition(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestorePosition(uid, name);
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
					return firesecService.GetMinJournalDateTime();
			}, "GetMinJournalDateTime");
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return SafeOperationCall<OperationResult<List<JournalItem>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetFilteredJournalItems(filter);
			}, "GetFilteredJournalItems");
		}
		public OperationResult<bool> BeginGetJournal(JournalFilter filter)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.BeginGetJournal(filter);
			}, "BeginGetJournal");
		}
		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddJournalItem(journalItem);
			}, "AddJournalItem");
		}
		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.BeginGetArchivePage(filter, page);
			}, "BeginGetArchivePage");
		}
		public OperationResult<int> GetArchiveCount(JournalFilter filter)
		{
			return SafeOperationCall<OperationResult<int>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetArchiveCount(filter);
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
					return firesecService.GetCards(filter);
			}, "GetCards");
		}
		public OperationResult<SKDCard> GetSingleCard(Guid uid)
		{
			return SafeOperationCall<OperationResult<SKDCard>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetSingleCard(uid);
			}, "GetSingleCard");
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			return SafeOperationCall<OperationResult<List<SKDCard>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeCards(employeeUID);
			}, "GetEmployeeCards");
		}
		public OperationResult<bool> AddCard(SKDCard item, string employeeName)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddCard(item, employeeName);
			}, "AddCard");
		}
		public OperationResult<bool> EditCard(SKDCard item, string employeeName)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditCard(item, employeeName);
			}, "EditCard");
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteCardFromEmployee(item, employeeName, reason);
			}, "DeleteCardFromEmployee");
		}
		public OperationResult DeletedCard(SKDCard card)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeletedCard(card);
			}, "DeletedCard");
		}
		public OperationResult SaveCardTemplate(SKDCard item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveCardTemplate(item);
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
					return firesecService.GetAccessTemplates(filter);
			}, "GetAccessTemplates");
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveAccessTemplate(item, isNew);
			}, "SaveAccessTemplate");
		}
		public OperationResult MarkDeletedAccessTemplate(AccessTemplate item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedAccessTemplate(item);
			}, "MarkDeletedAccessTemplate");
		}
		public OperationResult RestoreAccessTemplate(AccessTemplate item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreAccessTemplate(item);
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
					return firesecService.GetOrganisations(filter);
			}, "GetOrganisations");
		}
		public OperationResult<bool> SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisation(item, isNew);
			}, "SaveOrganisation");
		}
		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedOrganisation(uid, name);
			}, "MarkDeletedOrganisation");
		}
		public OperationResult AddOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddOrganisationDoor(item, doorUID);
			}, "AddOrganisationDoor");
		}
		public OperationResult RemoveOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RemoveOrganisationDoor(item, doorUID);
			}, "RemoveOrganisationDoor");
		}
		public OperationResult SaveOrganisationUsers(Organisation item)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationUsers(item);
			}, "SaveOrganisationUsers");
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SafeOperationCall<OperationResult<OrganisationDetails>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetOrganisationDetails(uid);
			}, "GetOrganisationDetails");
		}
		public OperationResult SaveOrganisationChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationChief(uid, chiefUID, name);
			}, "SaveOrganisationChief");
		}
		public OperationResult SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationHRChief(uid, chiefUID, name);
			}, "SaveOrganisationHRChief");
		}
		public OperationResult RestoreOrganisation(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreOrganisation(uid, name);
			}, "RestoreOrganisation");
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.IsAnyOrganisationItems(uid);
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
					return firesecService.GetAdditionalColumnTypes(filter);
			}, "GetAdditionalColumnTypes");
		}
		public OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveAdditionalColumnType(item, isNew);
			}, "SaveAdditionalColumnType");
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedAdditionalColumnType(uid, name);
			}, "MarkDeletedAdditionalColumnType");
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreAdditionalColumnType(uid, name);
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
					return firesecService.GetNightSettingsByOrganisation(organisationUID);
			}, "GetNightSettingsByOrganisation");
		}
		public OperationResult SaveNightSettings(NightSettings nightSettings)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveNightSettings(nightSettings);
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
					return firesecService.GetPassCardTemplateList(filter);
			}, "GetPassCardTemplateList");
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			return SafeOperationCall<OperationResult<PassCardTemplate>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPassCardTemplateDetails(uid);
			}, "GetPassCardTemplateDetails");
		}
		public OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SavePassCardTemplate(item, isNew);
			}, "SavePassCardTemplate");
		}
		public OperationResult MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedPassCardTemplate(uid, name);
			}, "MarkDeletedPassCardTemplate");
		}
		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestorePassCardTemplate(uid, name);
			}, "RestorePassCardTemplate");
		}
		#endregion

		public OperationResult GenerateEmployeeDays()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GenerateEmployeeDays();
			}, "GenerateEmployeeDays");
		}

		public OperationResult GenerateJournal()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GenerateJournal();
			}, "GenerateJournal");
		}

		public OperationResult GenerateTestData(bool isAscending)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GenerateTestData(isAscending);
			}, "GenerateTestData");
		}

		public OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveJournalVideoUID(journalItemUID, videoUID, cameraUID);
			}, "SaveJournalVideoUID");
		}

		public OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveJournalCameraUID(journalItemUID, CameraUID);
			}, "SaveJournalCameraUID");
		}

		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules()
		{
			return SafeOperationCall<OperationResult<List<GKSchedule>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetGKSchedules();
			}, "GetGKSchedules");
		}

		public OperationResult<bool> SaveGKSchedule(GKSchedule item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveGKSchedule(item, isNew);
			}, "SaveGKSchedule");
		}

		public OperationResult<bool> DeleteGKSchedule(GKSchedule item)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteGKSchedule(item);
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
					return firesecService.GetGKDaySchedules();
			}, "GetGKDaySchedules");
		}

		public OperationResult<bool> SaveGKDaySchedule(GKDaySchedule item, bool isNew)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveGKDaySchedule(item, isNew);
			}, "SaveGKDaySchedule");
		}

		public OperationResult<bool> DeleteGKDaySchedule(GKDaySchedule item)
		{
			return SafeOperationCall<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteGKDaySchedule(item);
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
					return firesecService.ExportOrganisation(filter);
			}, "ExportOrganisation");
		}
		public OperationResult ImportOrganisation(ImportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ImportOrganisation(filter);
			}, "ImportOrganisation");
		}
		public OperationResult ExportOrganisationList(ExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportOrganisationList(filter);
			}, "ExportOrganisationList");
		}
		public OperationResult ImportOrganisationList(ImportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ImportOrganisationList(filter);
			}, "ImportOrganisationList");
		}
		public OperationResult ExportJournal(JournalExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportJournal(filter);
			}, "ExportJournal");
		}
		public OperationResult ExportConfiguration(ConfigurationExportFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ExportConfiguration(filter);
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
					return firesecService.SaveCurrentConsumption(item);
			}, "SaveCurrentConsumption");
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter item)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetCurrentConsumption(item);
			}, "GetCurrentConsumption");
		}
		#endregion
	}
}