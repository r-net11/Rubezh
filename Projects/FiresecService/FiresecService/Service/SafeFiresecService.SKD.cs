using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		#region Employee
		public OperationResult<List<ShortEmployee>> GetEmployeeList(EmployeeFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetEmployeeList(filter, clientUID), "GetEmployeeList", clientUID);
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetEmployeeDetails(uid, clientUID), "GetEmployeeDetails", clientUID);
		}
		public OperationResult<bool> SaveEmployee(Employee item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveEmployee(item, isNew, clientUID), "SaveEmployee", clientUID);
		}
		public OperationResult MarkDeletedEmployee(Guid uid, string name, bool isEmployee, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedEmployee(uid, name, isEmployee, clientUID), "MarkDeletedEmployee", clientUID);
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate, Guid clientUID)
		{
			var result = SafeOperationCall(() => FiresecService.GetTimeTracks(filter, startDate, endDate, clientUID), "GetTimeTracks", clientUID);
			return result;
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate, Guid clientUID)
		{
			var result = SafeOperationCall(() => FiresecService.GetTimeTracksStream(filter, startDate, endDate, clientUID), "GetTimeTracksStream", clientUID);
			return result;
		}
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveEmployeeDepartment(uid, departmentUid, name, clientUID), "SaveEmployeeDepartment", clientUID);
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid? positionUid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveEmployeePosition(uid, positionUid, name, clientUID), "SaveEmployeePosition", clientUID);
		}
		public OperationResult RestoreEmployee(Guid uid, string name, bool isEmployee, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestoreEmployee(uid, name, isEmployee, clientUID), "RestoreEmployee", clientUID);
		}
		#endregion

		#region Department
		public OperationResult<List<ShortDepartment>> GetDepartmentList(DepartmentFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetDepartmentList(filter, clientUID), "GetDepartmentList", clientUID);
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetDepartmentDetails(uid, clientUID), "GetDepartmentDetails", clientUID);
		}
		public OperationResult<bool> SaveDepartment(Department item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveDepartment(item, isNew, clientUID), "SaveDepartment", clientUID);
		}
		public OperationResult MarkDeletedDepartment(ShortDepartment department, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedDepartment(department, clientUID), "MarkDeletedDepartment", clientUID);
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid? chiefUID, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveDepartmentChief(uid, chiefUID, name, clientUID), "SaveDepartmentChief", clientUID);
		}
		public OperationResult RestoreDepartment(ShortDepartment department, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestoreDepartment(department, clientUID), "RestoreDepartment", clientUID);
		}
		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetChildEmployeeUIDs(uid, clientUID), "GetChildEmployeeUIDs", clientUID);
		}
		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetParentEmployeeUIDs(uid, clientUID), "GetParentEmployeeUIDs", clientUID);
		}
		#endregion

		#region Position
		public OperationResult<List<ShortPosition>> GetPositionList(PositionFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetPositionList(filter, clientUID), "GetPositionList", clientUID);
		}
		public OperationResult<List<Guid>> GetPositionEmployees(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetPositionEmployees(uid, clientUID), "GetPositionEmployees", clientUID);
		}
		public OperationResult<Position> GetPositionDetails(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetPositionDetails(uid, clientUID), "GetPositionDetails", clientUID);
		}
		public OperationResult<bool> SavePosition(Position item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SavePosition(item, isNew, clientUID), "SavePosition", clientUID);
		}
		public OperationResult MarkDeletedPosition(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedPosition(uid, name, clientUID), "MarkDeletedPosition", clientUID);
		}

		public OperationResult RestorePosition(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestorePosition(uid, name, clientUID), "RestorePosition", clientUID);
		}
		#endregion

		#region Card
		public OperationResult<List<SKDCard>> GetCards(CardFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetCards(filter, clientUID), "GetCards", clientUID);
		}
		public OperationResult<SKDCard> GetSingleCard(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetSingleCard(uid, clientUID), "GetSingleCard", clientUID);
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetEmployeeCards(employeeUID, clientUID), "GetEmployeeCards", clientUID);
		}
		public OperationResult<bool> AddCard(SKDCard item, string employeeName, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.AddCard(item, employeeName, clientUID), "AddCard", clientUID);
		}
		public OperationResult<bool> EditCard(SKDCard item, string employeeName, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.EditCard(item, employeeName, clientUID), "EditCard", clientUID);
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, Guid clientUID, string reason = null)
		{
			return SafeOperationCall(() => FiresecService.DeleteCardFromEmployee(item, employeeName, clientUID, reason), "DeleteCardFromEmployee", clientUID);
		}
		public OperationResult DeletedCard(SKDCard card, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.DeletedCard(card, clientUID), "DeletedCard", clientUID);
		}
		public OperationResult SaveCardTemplate(SKDCard item, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveCardTemplate(item, clientUID), "SaveCardTemplate", clientUID);
		}
		#endregion

		#region AccessTemplate
		public OperationResult<List<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetAccessTemplates(filter, clientUID), "GetAccessTemplates", clientUID);
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveAccessTemplate(item, isNew, clientUID), "SaveAccessTemplate", clientUID);
		}
		public OperationResult MarkDeletedAccessTemplate(AccessTemplate item, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedAccessTemplate(item, clientUID), "MarkDeletedAccessTemplate", clientUID);
		}
		public OperationResult RestoreAccessTemplate(AccessTemplate item, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestoreAccessTemplate(item, clientUID), "RestoreAccessTemplate", clientUID);
		}
		#endregion

		#region Organisation
		public OperationResult<List<Organisation>> GetOrganisations(OrganisationFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetOrganisations(filter, clientUID), "GetOrganisations", clientUID);
		}
		public OperationResult<bool> SaveOrganisation(OrganisationDetails item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveOrganisation(item, isNew, clientUID), "SaveOrganisation", clientUID);
		}
		public OperationResult MarkDeletedOrganisation(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedOrganisation(uid, name, clientUID), "MarkDeletedOrganisation", clientUID);
		}
		public OperationResult AddOrganisationDoor(Organisation organisation, Guid doorUID, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.AddOrganisationDoor(organisation, doorUID, clientUID), "AddOrganisationDoor", clientUID);
		}
		public OperationResult RemoveOrganisationDoor(Organisation organisation, Guid doorUID, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RemoveOrganisationDoor(organisation, doorUID, clientUID), "RemoveOrganisationDoor", clientUID);
		}
		public OperationResult SaveOrganisationUsers(Organisation organisation, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveOrganisationUsers(organisation, clientUID), "SaveOrganisationUsers", clientUID);
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetOrganisationDetails(uid, clientUID), "GetOrganisationDetails", clientUID);
		}
		public OperationResult SaveOrganisationChief(Guid uid, Guid? chiefUID, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveOrganisationChief(uid, chiefUID, name, clientUID), "SaveOrganisationChief", clientUID);
		}
		public OperationResult SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveOrganisationHRChief(uid, chiefUID, name, clientUID), "SaveOrganisationHRChief", clientUID);
		}
		public OperationResult RestoreOrganisation(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestoreOrganisation(uid, name, clientUID), "RestoreOrganisation", clientUID);
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.IsAnyOrganisationItems(uid, clientUID), "IsAnyOrganisationItems", clientUID);
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetAdditionalColumnTypes(filter, clientUID), "GetAdditionalColumnTypes", clientUID);
		}
		public OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveAdditionalColumnType(item, isNew, clientUID), "SaveAdditionalColumnType", clientUID);
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedAdditionalColumnType(uid, name, clientUID), "MarkDeletedAdditionalColumnType", clientUID);
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestoreAdditionalColumnType(uid, name, clientUID), "RestoreAdditionalColumnType", clientUID);
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetNightSettingsByOrganisation(organisationUID, clientUID), "GetNightSettingsByOrganisation", clientUID);
		}
		public OperationResult SaveNightSettings(NightSettings nightSettings, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveNightSettings(nightSettings, clientUID), "SaveNightSettings", clientUID);
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetPassCardTemplateList(filter, clientUID), "GetPassCardTemplateList", clientUID);
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetPassCardTemplateDetails(uid, clientUID), "GetPassCardTemplateDetails", clientUID);
		}
		public OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SavePassCardTemplate(item, isNew, clientUID), "SavePassCardTemplate", clientUID);
		}
		public OperationResult MarkDeletedPassCardTemplate(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedPassCardTemplate(uid, name, clientUID), "MarkDeletedPassCardTemplate", clientUID);
		}
		public OperationResult RestorePassCardTemplate(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestorePassCardTemplate(uid, name, clientUID), "RestorePassCardTemplate", clientUID);
		}
		#endregion

		public OperationResult GenerateEmployeeDays(Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GenerateEmployeeDays(clientUID), "GenerateEmployeeDays", clientUID);
		}

		public OperationResult GenerateJournal(Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GenerateJournal(clientUID), "GenerateJournal", clientUID);
		}

		public OperationResult GenerateTestData(bool isAscending, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GenerateTestData(isAscending, clientUID), "GenerateTestData", clientUID);
		}

		public OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveJournalVideoUID(journalItemUID, videoUID, cameraUID, clientUID), "SaveJournalVideoUID", clientUID);
		}

		public OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveJournalCameraUID(journalItemUID, CameraUID, clientUID), "SaveJournalCameraUID", clientUID);
		}

		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules(Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetGKSchedules(clientUID), "GetGKSchedules", clientUID);
		}

		public OperationResult<bool> SaveGKSchedule(GKSchedule item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveGKSchedule(item, isNew, clientUID), "SaveGKSchedule", clientUID);
		}

		public OperationResult<bool> DeleteGKSchedule(GKSchedule item, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.DeleteGKSchedule(item, clientUID), "DeleteGKSchedule", clientUID);
		}
		#endregion

		#region GKDaySchedule
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules(Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetGKDaySchedules(clientUID), "GetGKDaySchedules", clientUID);
		}

		public OperationResult<bool> SaveGKDaySchedule(GKDaySchedule item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveGKDaySchedule(item, isNew, clientUID), "SaveGKDaySchedule", clientUID);
		}

		public OperationResult<bool> DeleteGKDaySchedule(GKDaySchedule item, Guid clientUID)
		{
			return SafeOperationCall<OperationResult<bool>>(() => FiresecService.DeleteGKDaySchedule(item, clientUID), "DeleteGKDaySchedule", clientUID);
		}
		#endregion

		#region Export
		public OperationResult ExportOrganisation(ExportFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.ExportOrganisation(filter, clientUID), "ExportOrganisation", clientUID);
		}
		public OperationResult ImportOrganisation(ImportFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.ImportOrganisation(filter, clientUID), "ImportOrganisation", clientUID);
		}
		public OperationResult ExportOrganisationList(ExportFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.ExportOrganisationList(filter, clientUID), "ExportOrganisationList", clientUID);
		}
		public OperationResult ImportOrganisationList(ImportFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.ImportOrganisationList(filter, clientUID), "ImportOrganisationList", clientUID);
		}
		public OperationResult ExportJournal(JournalExportFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.ExportJournal(filter, clientUID), "ExportJournal", clientUID);
		}
		public OperationResult ExportConfiguration(ConfigurationExportFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.ExportConfiguration(filter, clientUID), "ExportConfiguration", clientUID);
		}
		#endregion

		#region CurrentConsumption
		public OperationResult SaveCurrentConsumption(CurrentConsumption item, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveCurrentConsumption(item, clientUID), "SaveCurrentConsumption", clientUID);
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter item, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetCurrentConsumption(item, clientUID), "GetCurrentConsumption", clientUID);
		}
		#endregion
	}
}