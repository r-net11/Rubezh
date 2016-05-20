using Common;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;

namespace RubezhService.Service
{
	public partial class SafeRubezhService
	{
		#region Employee
		public OperationResult<List<ShortEmployee>> GetEmployeeList(Guid clientUID, EmployeeFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetEmployeeList(clientUID, filter), "GetEmployeeList");
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetEmployeeDetails(clientUID, uid), "GetEmployeeDetails");
		}
		public OperationResult<bool> SaveEmployee(Guid clientUID, Employee item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveEmployee(clientUID, item, isNew), "SaveEmployee");
		}
		public OperationResult<bool> MarkDeletedEmployee(Guid clientUID, Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedEmployee(clientUID, uid, name, isEmployee), "MarkDeletedEmployee");
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(Guid clientUID, EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeOperationCall(clientUID, () => RubezhService.GetTimeTracks(clientUID, filter, startDate, endDate), "GetTimeTracks");
			return result;
		}
		public Stream GetTimeTracksStream(Guid clientUID, EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeOperationCall(clientUID, () => RubezhService.GetTimeTracksStream(clientUID, filter, startDate, endDate), "GetTimeTracksStream");
			return result;
		}
		public OperationResult<bool> SaveEmployeeDepartment(Guid clientUID, Guid uid, Guid? departmentUid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveEmployeeDepartment(clientUID, uid, departmentUid, name), "SaveEmployeeDepartment");
		}
		public OperationResult<bool> SaveEmployeePosition(Guid clientUID, Guid uid, Guid? positionUid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveEmployeePosition(clientUID, uid, positionUid, name), "SaveEmployeePosition");
		}
		public OperationResult<bool> RestoreEmployee(Guid clientUID, Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestoreEmployee(clientUID, uid, name, isEmployee), "RestoreEmployee");
		}
		#endregion

		#region Department
		public OperationResult<List<ShortDepartment>> GetDepartmentList(Guid clientUID, DepartmentFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetDepartmentList(clientUID, filter), "GetDepartmentList");
		}
		public OperationResult<Department> GetDepartmentDetails(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetDepartmentDetails(clientUID, uid), "GetDepartmentDetails");
		}
		public OperationResult<bool> SaveDepartment(Guid clientUID, Department item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveDepartment(clientUID, item, isNew), "SaveDepartment");
		}
		public OperationResult<bool> MarkDeletedDepartment(Guid clientUID, ShortDepartment department)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedDepartment(clientUID, department), "MarkDeletedDepartment");
		}
		public OperationResult<bool> SaveDepartmentChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveDepartmentChief(clientUID, uid, chiefUID, name), "SaveDepartmentChief");
		}
		public OperationResult<bool> RestoreDepartment(Guid clientUID, ShortDepartment department)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestoreDepartment(clientUID, department), "RestoreDepartment");
		}
		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetChildEmployeeUIDs(clientUID, uid), "GetChildEmployeeUIDs");
		}
		public OperationResult<List<Guid>> GetDepartmentEmployeeUIDs(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetDepartmentEmployeeUIDs(clientUID, uid), "GetEmployeeUIDs");
		}
		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetParentEmployeeUIDs(clientUID, uid), "GetParentEmployeeUIDs");
		}
		#endregion

		#region Position
		public OperationResult<List<ShortPosition>> GetPositionList(Guid clientUID, PositionFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetPositionList(clientUID, filter), "GetPositionList");
		}
		public OperationResult<List<Guid>> GetPositionEmployees(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetPositionEmployees(clientUID, uid), "GetPositionEmployees");
		}
		public OperationResult<Position> GetPositionDetails(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetPositionDetails(clientUID, uid), "GetPositionDetails");
		}
		public OperationResult<bool> SavePosition(Guid clientUID, Position item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SavePosition(clientUID, item, isNew), "SavePosition");
		}
		public OperationResult<bool> MarkDeletedPosition(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedPosition(clientUID, uid, name), "MarkDeletedPosition");
		}

		public OperationResult<bool> RestorePosition(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestorePosition(clientUID, uid, name), "RestorePosition");
		}
		#endregion

		#region Card
		public OperationResult<List<SKDCard>> GetCards(Guid clientUID, CardFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetCards(clientUID, filter), "GetCards");
		}
		public OperationResult<SKDCard> GetSingleCard(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetSingleCard(clientUID, uid), "GetSingleCard");
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid clientUID, Guid employeeUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetEmployeeCards(clientUID, employeeUID), "GetEmployeeCards");
		}
		public OperationResult<bool> AddCard(Guid clientUID, SKDCard item, string employeeName)
		{
			return SafeOperationCall(clientUID, () => RubezhService.AddCard(clientUID, item, employeeName), "AddCard");
		}
		public OperationResult<bool> EditCard(Guid clientUID, SKDCard item, string employeeName)
		{
			return SafeOperationCall(clientUID, () => RubezhService.EditCard(clientUID, item, employeeName), "EditCard");
		}
		public OperationResult<bool> DeleteCardFromEmployee(Guid clientUID, SKDCard item, string employeeName, string reason = null)
		{
			return SafeOperationCall(clientUID, () => RubezhService.DeleteCardFromEmployee(clientUID, item, employeeName, reason), "DeleteCardFromEmployee");
		}
		public OperationResult<bool> DeletedCard(Guid clientUID, SKDCard card)
		{
			return SafeOperationCall(clientUID, () => RubezhService.DeletedCard(clientUID, card), "DeletedCard");
		}
		public OperationResult<bool> SaveCardTemplate(Guid clientUID, SKDCard item)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveCardTemplate(clientUID, item), "SaveCardTemplate");
		}
		public OperationResult<List<GKUser>> GetDbDeviceUsers(Guid deviceUID, List<Guid> doorUIDs)
		{
			return SafeContext.Execute<OperationResult<List<GKUser>>>(() => RubezhService.GetDbDeviceUsers(deviceUID, doorUIDs));
		}
		#endregion

		#region AccessTemplate
		public OperationResult<List<AccessTemplate>> GetAccessTemplates(Guid clientUID, AccessTemplateFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetAccessTemplates(clientUID, filter), "GetAccessTemplates");
		}
		public OperationResult<bool> SaveAccessTemplate(Guid clientUID, AccessTemplate item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveAccessTemplate(clientUID, item, isNew), "SaveAccessTemplate");
		}
		public OperationResult<List<string>> MarkDeletedAccessTemplate(Guid clientUID, AccessTemplate item)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedAccessTemplate(clientUID, item), "MarkDeletedAccessTemplate");
		}
		public OperationResult<bool> RestoreAccessTemplate(Guid clientUID, AccessTemplate item)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestoreAccessTemplate(clientUID, item), "RestoreAccessTemplate");
		}
		#endregion

		#region Organisation
		public OperationResult<List<Organisation>> GetOrganisations(Guid clientUID, OrganisationFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetOrganisations(clientUID, filter), "GetOrganisations");
		}
		public OperationResult<bool> SaveOrganisation(Guid clientUID, OrganisationDetails item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveOrganisation(clientUID, item, isNew), "SaveOrganisation");
		}
		public OperationResult<bool> MarkDeletedOrganisation(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedOrganisation(clientUID, uid, name), "MarkDeletedOrganisation");
		}
		public OperationResult<bool> AddOrganisationDoor(Guid clientUID, Organisation organisation, Guid doorUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.AddOrganisationDoor(clientUID, organisation, doorUID), "AddOrganisationDoor");
		}
		public OperationResult<bool> RemoveOrganisationDoor(Guid clientUID, Organisation organisation, Guid doorUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RemoveOrganisationDoor(clientUID, organisation, doorUID), "RemoveOrganisationDoor");
		}
		public OperationResult<bool> SaveOrganisationUsers(Guid clientUID, Organisation organisation)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveOrganisationUsers(clientUID, organisation), "SaveOrganisationUsers");
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetOrganisationDetails(clientUID, uid), "GetOrganisationDetails");
		}
		public OperationResult<bool> SaveOrganisationChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveOrganisationChief(clientUID, uid, chiefUID, name), "SaveOrganisationChief");
		}
		public OperationResult<bool> SaveOrganisationHRChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveOrganisationHRChief(clientUID, uid, chiefUID, name), "SaveOrganisationHRChief");
		}
		public OperationResult<bool> RestoreOrganisation(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestoreOrganisation(clientUID, uid, name), "RestoreOrganisation");
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.IsAnyOrganisationItems(clientUID, uid), "IsAnyOrganisationItems");
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(Guid clientUID, AdditionalColumnTypeFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetAdditionalColumnTypes(clientUID, filter), "GetAdditionalColumnTypes");
		}
		public OperationResult<bool> SaveAdditionalColumnType(Guid clientUID, AdditionalColumnType item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveAdditionalColumnType(clientUID, item, isNew), "SaveAdditionalColumnType");
		}
		public OperationResult<bool> MarkDeletedAdditionalColumnType(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedAdditionalColumnType(clientUID, uid, name), "MarkDeletedAdditionalColumnType");
		}
		public OperationResult<bool> RestoreAdditionalColumnType(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestoreAdditionalColumnType(clientUID, uid, name), "RestoreAdditionalColumnType");
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid clientUID, Guid organisationUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetNightSettingsByOrganisation(clientUID, organisationUID), "GetNightSettingsByOrganisation");
		}
		public OperationResult<bool> SaveNightSettings(Guid clientUID, NightSettings nightSettings)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveNightSettings(clientUID, nightSettings), "SaveNightSettings");
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(Guid clientUID, PassCardTemplateFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetPassCardTemplateList(clientUID, filter), "GetPassCardTemplateList");
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetPassCardTemplateDetails(clientUID, uid), "GetPassCardTemplateDetails");
		}
		public OperationResult<bool> SavePassCardTemplate(Guid clientUID, PassCardTemplate item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SavePassCardTemplate(clientUID, item, isNew), "SavePassCardTemplate");
		}
		public OperationResult<bool> MarkDeletedPassCardTemplate(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedPassCardTemplate(clientUID, uid, name), "MarkDeletedPassCardTemplate");
		}
		public OperationResult<bool> RestorePassCardTemplate(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestorePassCardTemplate(clientUID, uid, name), "RestorePassCardTemplate");
		}
		#endregion

		public OperationResult<bool> GenerateEmployeeDays(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GenerateEmployeeDays(clientUID), "GenerateEmployeeDays");
		}

		public OperationResult<bool> GenerateJournal(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GenerateJournal(clientUID), "GenerateJournal");
		}

		public OperationResult<bool> GenerateTestData(Guid clientUID, bool isAscending)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GenerateTestData(clientUID, isAscending), "GenerateTestData");
		}

		public OperationResult<bool> SaveJournalVideoUID(Guid clientUID, Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveJournalVideoUID(clientUID, journalItemUID, videoUID, cameraUID), "SaveJournalVideoUID");
		}

		public OperationResult<bool> SaveJournalCameraUID(Guid clientUID, Guid journalItemUID, Guid CameraUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveJournalCameraUID(clientUID, journalItemUID, CameraUID), "SaveJournalCameraUID");
		}

		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetGKSchedules(clientUID), "GetGKSchedules");
		}

		public OperationResult<bool> SaveGKSchedule(Guid clientUID, GKSchedule item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveGKSchedule(clientUID, item, isNew), "SaveGKSchedule");
		}

		public OperationResult<bool> DeleteGKSchedule(Guid clientUID, GKSchedule item)
		{
			return SafeOperationCall(clientUID, () => RubezhService.DeleteGKSchedule(clientUID, item), "DeleteGKSchedule");
		}
		#endregion

		#region GKDaySchedule
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetGKDaySchedules(clientUID), "GetGKDaySchedules");
		}

		public OperationResult<bool> SaveGKDaySchedule(Guid clientUID, GKDaySchedule item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveGKDaySchedule(clientUID, item, isNew), "SaveGKDaySchedule");
		}

		public OperationResult<bool> DeleteGKDaySchedule(Guid clientUID, GKDaySchedule item)
		{
			return SafeOperationCall(clientUID, () => RubezhService.DeleteGKDaySchedule(clientUID, item), "DeleteGKDaySchedule");
		}
		#endregion

		#region Export
		public OperationResult<bool> ExportOrganisation(Guid clientUID, ExportFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.ExportOrganisation(clientUID, filter), "ExportOrganisation");
		}
		public OperationResult<bool> ImportOrganisation(Guid clientUID, ImportFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.ImportOrganisation(clientUID, filter), "ImportOrganisation");
		}
		public OperationResult<bool> ExportOrganisationList(Guid clientUID, ExportFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.ExportOrganisationList(clientUID, filter), "ExportOrganisationList");
		}
		public OperationResult<bool> ImportOrganisationList(Guid clientUID, ImportFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.ImportOrganisationList(clientUID, filter), "ImportOrganisationList");
		}
		public OperationResult<bool> ExportJournal(Guid clientUID, JournalExportFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.ExportJournal(clientUID, filter), "ExportJournal");
		}
		public OperationResult<bool> ExportConfiguration(Guid clientUID, ConfigurationExportFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.ExportConfiguration(clientUID, filter), "ExportConfiguration");
		}
		#endregion

		#region CurrentConsumption
		public OperationResult<bool> SaveCurrentConsumption(Guid clientUID, CurrentConsumption item)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveCurrentConsumption(clientUID, item), "SaveCurrentConsumption");
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(Guid clientUID, CurrentConsumptionFilter item)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetCurrentConsumption(clientUID, item), "GetCurrentConsumption");
		}
		#endregion
	}
}