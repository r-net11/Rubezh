using Common;
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
		public OperationResult<List<ShortEmployee>> GetEmployeeList(Guid clientUID, EmployeeFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetEmployeeList(clientUID, filter), "GetEmployeeList");
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetEmployeeDetails(clientUID, uid), "GetEmployeeDetails");
		}
		public OperationResult<bool> SaveEmployee(Guid clientUID, Employee item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveEmployee(clientUID, item, isNew), "SaveEmployee");
		}
		public OperationResult<bool> MarkDeletedEmployee(Guid clientUID, Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(clientUID, () => FiresecService.MarkDeletedEmployee(clientUID, uid, name, isEmployee), "MarkDeletedEmployee");
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(Guid clientUID, EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeOperationCall(clientUID, () => FiresecService.GetTimeTracks(clientUID, filter, startDate, endDate), "GetTimeTracks");
			return result;
		}
		public Stream GetTimeTracksStream(Guid clientUID, EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeOperationCall(clientUID, () => FiresecService.GetTimeTracksStream(clientUID, filter, startDate, endDate), "GetTimeTracksStream");
			return result;
		}
		public OperationResult<bool> SaveEmployeeDepartment(Guid clientUID, Guid uid, Guid? departmentUid, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveEmployeeDepartment(clientUID, uid, departmentUid, name), "SaveEmployeeDepartment");
		}
		public OperationResult<bool> SaveEmployeePosition(Guid clientUID, Guid uid, Guid? positionUid, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveEmployeePosition(clientUID, uid, positionUid, name), "SaveEmployeePosition");
		}
		public OperationResult<bool> RestoreEmployee(Guid clientUID, Guid uid, string name, bool isEmployee)
		{
			return SafeOperationCall(clientUID, () => FiresecService.RestoreEmployee(clientUID, uid, name, isEmployee), "RestoreEmployee");
		}
		#endregion

		#region Department
		public OperationResult<List<ShortDepartment>> GetDepartmentList(Guid clientUID, DepartmentFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetDepartmentList(clientUID, filter), "GetDepartmentList");
		}
		public OperationResult<Department> GetDepartmentDetails(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetDepartmentDetails(clientUID, uid), "GetDepartmentDetails");
		}
		public OperationResult<bool> SaveDepartment(Guid clientUID, Department item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveDepartment(clientUID, item, isNew), "SaveDepartment");
		}
		public OperationResult<bool> MarkDeletedDepartment(Guid clientUID, ShortDepartment department)
		{
			return SafeOperationCall(clientUID, () => FiresecService.MarkDeletedDepartment(clientUID, department), "MarkDeletedDepartment");
		}
		public OperationResult<bool> SaveDepartmentChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveDepartmentChief(clientUID, uid, chiefUID, name), "SaveDepartmentChief");
		}
		public OperationResult<bool> RestoreDepartment(Guid clientUID, ShortDepartment department)
		{
			return SafeOperationCall(clientUID, () => FiresecService.RestoreDepartment(clientUID, department), "RestoreDepartment");
		}
		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetChildEmployeeUIDs(clientUID, uid), "GetChildEmployeeUIDs");
		}
		public OperationResult<List<Guid>> GetDepartmentEmployeeUIDs(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetDepartmentEmployeeUIDs(clientUID, uid), "GetChildEmployeeUIDs");
		}
		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetParentEmployeeUIDs(clientUID, uid), "GetParentEmployeeUIDs");
		}
		#endregion

		#region Position
		public OperationResult<List<ShortPosition>> GetPositionList(Guid clientUID, PositionFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetPositionList(clientUID, filter), "GetPositionList");
		}
		public OperationResult<List<Guid>> GetPositionEmployees(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetPositionEmployees(clientUID, uid), "GetPositionEmployees");
		}
		public OperationResult<Position> GetPositionDetails(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetPositionDetails(clientUID, uid), "GetPositionDetails");
		}
		public OperationResult<bool> SavePosition(Guid clientUID, Position item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SavePosition(clientUID, item, isNew), "SavePosition");
		}
		public OperationResult<bool> MarkDeletedPosition(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.MarkDeletedPosition(clientUID, uid, name), "MarkDeletedPosition");
		}

		public OperationResult<bool> RestorePosition(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.RestorePosition(clientUID, uid, name), "RestorePosition");
		}
		#endregion

		#region Card
		public OperationResult<List<SKDCard>> GetCards(Guid clientUID, CardFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetCards(clientUID, filter), "GetCards");
		}
		public OperationResult<SKDCard> GetSingleCard(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetSingleCard(clientUID, uid), "GetSingleCard");
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid clientUID, Guid employeeUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetEmployeeCards(clientUID, employeeUID), "GetEmployeeCards");
		}
		public OperationResult<bool> AddCard(Guid clientUID, SKDCard item, string employeeName)
		{
			return SafeOperationCall(clientUID, () => FiresecService.AddCard(clientUID, item, employeeName), "AddCard");
		}
		public OperationResult<bool> EditCard(Guid clientUID, SKDCard item, string employeeName)
		{
			return SafeOperationCall(clientUID, () => FiresecService.EditCard(clientUID, item, employeeName), "EditCard");
		}
		public OperationResult<bool> DeleteCardFromEmployee(Guid clientUID, SKDCard item, string employeeName, string reason = null)
		{
			return SafeOperationCall(clientUID, () => FiresecService.DeleteCardFromEmployee(clientUID, item, employeeName, reason), "DeleteCardFromEmployee");
		}
		public OperationResult<bool> DeletedCard(Guid clientUID, SKDCard card)
		{
			return SafeOperationCall(clientUID, () => FiresecService.DeletedCard(clientUID, card), "DeletedCard");
		}
		public OperationResult<bool> SaveCardTemplate(Guid clientUID, SKDCard item)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveCardTemplate(clientUID, item), "SaveCardTemplate");
		}
		public OperationResult<List<GKUser>> GetDbDeviceUsers(Guid deviceUID, List<Guid> doorUIDs)
		{
			return SafeContext.Execute<OperationResult<List<GKUser>>>(() => FiresecService.GetDbDeviceUsers(deviceUID, doorUIDs));
		}
		#endregion

		#region AccessTemplate
		public OperationResult<List<AccessTemplate>> GetAccessTemplates(Guid clientUID, AccessTemplateFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetAccessTemplates(clientUID, filter), "GetAccessTemplates");
		}
		public OperationResult<bool> SaveAccessTemplate(Guid clientUID, AccessTemplate item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveAccessTemplate(clientUID, item, isNew), "SaveAccessTemplate");
		}
		public OperationResult<List<string>> MarkDeletedAccessTemplate(Guid clientUID, AccessTemplate item)
		{
			return SafeOperationCall(clientUID, () => FiresecService.MarkDeletedAccessTemplate(clientUID, item), "MarkDeletedAccessTemplate");
		}
		public OperationResult<bool> RestoreAccessTemplate(Guid clientUID, AccessTemplate item)
		{
			return SafeOperationCall(clientUID, () => FiresecService.RestoreAccessTemplate(clientUID, item), "RestoreAccessTemplate");
		}
		#endregion

		#region Organisation
		public OperationResult<List<Organisation>> GetOrganisations(Guid clientUID, OrganisationFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetOrganisations(clientUID, filter), "GetOrganisations");
		}
		public OperationResult<bool> SaveOrganisation(Guid clientUID, OrganisationDetails item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveOrganisation(clientUID, item, isNew), "SaveOrganisation");
		}
		public OperationResult<bool> MarkDeletedOrganisation(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.MarkDeletedOrganisation(clientUID, uid, name), "MarkDeletedOrganisation");
		}
		public OperationResult<bool> AddOrganisationDoor(Guid clientUID, Organisation organisation, Guid doorUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.AddOrganisationDoor(clientUID, organisation, doorUID), "AddOrganisationDoor");
		}
		public OperationResult<bool> RemoveOrganisationDoor(Guid clientUID, Organisation organisation, Guid doorUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.RemoveOrganisationDoor(clientUID, organisation, doorUID), "RemoveOrganisationDoor");
		}
		public OperationResult<bool> SaveOrganisationUsers(Guid clientUID, Organisation organisation)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveOrganisationUsers(clientUID, organisation), "SaveOrganisationUsers");
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetOrganisationDetails(clientUID, uid), "GetOrganisationDetails");
		}
		public OperationResult<bool> SaveOrganisationChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveOrganisationChief(clientUID, uid, chiefUID, name), "SaveOrganisationChief");
		}
		public OperationResult<bool> SaveOrganisationHRChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveOrganisationHRChief(clientUID, uid, chiefUID, name), "SaveOrganisationHRChief");
		}
		public OperationResult<bool> RestoreOrganisation(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.RestoreOrganisation(clientUID, uid, name), "RestoreOrganisation");
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.IsAnyOrganisationItems(clientUID, uid), "IsAnyOrganisationItems");
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(Guid clientUID, AdditionalColumnTypeFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetAdditionalColumnTypes(clientUID, filter), "GetAdditionalColumnTypes");
		}
		public OperationResult<bool> SaveAdditionalColumnType(Guid clientUID, AdditionalColumnType item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveAdditionalColumnType(clientUID, item, isNew), "SaveAdditionalColumnType");
		}
		public OperationResult<bool> MarkDeletedAdditionalColumnType(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.MarkDeletedAdditionalColumnType(clientUID, uid, name), "MarkDeletedAdditionalColumnType");
		}
		public OperationResult<bool> RestoreAdditionalColumnType(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.RestoreAdditionalColumnType(clientUID, uid, name), "RestoreAdditionalColumnType");
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid clientUID, Guid organisationUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetNightSettingsByOrganisation(clientUID, organisationUID), "GetNightSettingsByOrganisation");
		}
		public OperationResult<bool> SaveNightSettings(Guid clientUID, NightSettings nightSettings)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveNightSettings(clientUID, nightSettings), "SaveNightSettings");
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(Guid clientUID, PassCardTemplateFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetPassCardTemplateList(clientUID, filter), "GetPassCardTemplateList");
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetPassCardTemplateDetails(clientUID, uid), "GetPassCardTemplateDetails");
		}
		public OperationResult<bool> SavePassCardTemplate(Guid clientUID, PassCardTemplate item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SavePassCardTemplate(clientUID, item, isNew), "SavePassCardTemplate");
		}
		public OperationResult<bool> MarkDeletedPassCardTemplate(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.MarkDeletedPassCardTemplate(clientUID, uid, name), "MarkDeletedPassCardTemplate");
		}
		public OperationResult<bool> RestorePassCardTemplate(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => FiresecService.RestorePassCardTemplate(clientUID, uid, name), "RestorePassCardTemplate");
		}
		#endregion

		public OperationResult<bool> GenerateEmployeeDays(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GenerateEmployeeDays(clientUID), "GenerateEmployeeDays");
		}

		public OperationResult<bool> GenerateJournal(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GenerateJournal(clientUID), "GenerateJournal");
		}

		public OperationResult<bool> GenerateTestData(Guid clientUID, bool isAscending)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GenerateTestData(clientUID, isAscending), "GenerateTestData");
		}

		public OperationResult<bool> SaveJournalVideoUID(Guid clientUID, Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveJournalVideoUID(clientUID, journalItemUID, videoUID, cameraUID), "SaveJournalVideoUID");
		}

		public OperationResult<bool> SaveJournalCameraUID(Guid clientUID, Guid journalItemUID, Guid CameraUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveJournalCameraUID(clientUID, journalItemUID, CameraUID), "SaveJournalCameraUID");
		}

		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetGKSchedules(clientUID), "GetGKSchedules");
		}

		public OperationResult<bool> SaveGKSchedule(Guid clientUID, GKSchedule item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveGKSchedule(clientUID, item, isNew), "SaveGKSchedule");
		}

		public OperationResult<bool> DeleteGKSchedule(Guid clientUID, GKSchedule item)
		{
			return SafeOperationCall(clientUID, () => FiresecService.DeleteGKSchedule(clientUID, item), "DeleteGKSchedule");
		}
		#endregion

		#region GKDaySchedule
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetGKDaySchedules(clientUID), "GetGKDaySchedules");
		}

		public OperationResult<bool> SaveGKDaySchedule(Guid clientUID, GKDaySchedule item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveGKDaySchedule(clientUID, item, isNew), "SaveGKDaySchedule");
		}

		public OperationResult<bool> DeleteGKDaySchedule(Guid clientUID, GKDaySchedule item)
		{
			return SafeOperationCall(clientUID, () => FiresecService.DeleteGKDaySchedule(clientUID, item), "DeleteGKDaySchedule");
		}
		#endregion

		#region Export
		public OperationResult<bool> ExportOrganisation(Guid clientUID, ExportFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.ExportOrganisation(clientUID, filter), "ExportOrganisation");
		}
		public OperationResult<bool> ImportOrganisation(Guid clientUID, ImportFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.ImportOrganisation(clientUID, filter), "ImportOrganisation");
		}
		public OperationResult<bool> ExportOrganisationList(Guid clientUID, ExportFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.ExportOrganisationList(clientUID, filter), "ExportOrganisationList");
		}
		public OperationResult<bool> ImportOrganisationList(Guid clientUID, ImportFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.ImportOrganisationList(clientUID, filter), "ImportOrganisationList");
		}
		public OperationResult<bool> ExportJournal(Guid clientUID, JournalExportFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.ExportJournal(clientUID, filter), "ExportJournal");
		}
		public OperationResult<bool> ExportConfiguration(Guid clientUID, ConfigurationExportFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.ExportConfiguration(clientUID, filter), "ExportConfiguration");
		}
		#endregion

		#region CurrentConsumption
		public OperationResult<bool> SaveCurrentConsumption(Guid clientUID, CurrentConsumption item)
		{
			return SafeOperationCall(clientUID, () => FiresecService.SaveCurrentConsumption(clientUID, item), "SaveCurrentConsumption");
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(Guid clientUID, CurrentConsumptionFilter item)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetCurrentConsumption(clientUID, item), "GetCurrentConsumption");
		}
		#endregion
	}
}