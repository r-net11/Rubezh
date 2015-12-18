using RubezhAPI.GK;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace RubezhAPI
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public partial interface IFiresecServiceSKD
	{
		#region Employee
		[OperationContract]
		OperationResult<List<ShortEmployee>> GetEmployeeList(Guid clientUID, EmployeeFilter filter);

		[OperationContract]
		OperationResult<Employee> GetEmployeeDetails(Guid clientUID, Guid uid);

		[OperationContract]
		OperationResult<bool> SaveEmployee(Guid clientUID, Employee item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedEmployee(Guid clientUID, Guid uid, string name, bool isEmployee);

		[OperationContract]
		OperationResult<TimeTrackResult> GetTimeTracks(Guid clientUID, EmployeeFilter filter, DateTime startDate, DateTime endDate);

		[OperationContract]
		Stream GetTimeTracksStream(Guid clientUID, EmployeeFilter filter, DateTime startDate, DateTime endDate);

		[OperationContract]
		OperationResult SaveEmployeeDepartment(Guid clientUID, Guid uid, Guid? departmentUid, string name);

		[OperationContract]
		OperationResult SaveEmployeePosition(Guid clientUID, Guid uid, Guid? positionUid, string name);

		[OperationContract]
		OperationResult RestoreEmployee(Guid clientUID, Guid uid, string name, bool isEmployee);
		#endregion

		#region Department
		[OperationContract]
		OperationResult<List<ShortDepartment>> GetDepartmentList(Guid clientUID, DepartmentFilter filter);//

		[OperationContract]
		OperationResult<Department> GetDepartmentDetails(Guid clientUID, Guid uid);//

		[OperationContract]
		OperationResult<bool> SaveDepartment(Guid clientUID, Department item, bool isNew);//

		[OperationContract]
		OperationResult MarkDeletedDepartment(Guid clientUID, ShortDepartment department);//

		[OperationContract]
		OperationResult SaveDepartmentChief(Guid clientUID, Guid uid, Guid? chiefUID, string name);//

		[OperationContract]
		OperationResult RestoreDepartment(Guid clientUID, ShortDepartment department);//

		[OperationContract]
		OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid clientUID, Guid uid);//

		[OperationContract]
		OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid clientUID, Guid uid);//
		#endregion

		#region Position
		[OperationContract]
		OperationResult<List<ShortPosition>> GetPositionList(Guid clientUID, PositionFilter filter);

		[OperationContract]
		OperationResult<List<Guid>> GetPositionEmployees(Guid clientUID, Guid uid);

		[OperationContract]
		OperationResult<Position> GetPositionDetails(Guid clientUID, Guid uid);

		[OperationContract]
		OperationResult<bool> SavePosition(Guid clientUID, Position item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedPosition(Guid clientUID, Guid uid, string name);

		[OperationContract]
		OperationResult RestorePosition(Guid clientUID, Guid uid, string name);
		#endregion

		#region Card
		[OperationContract]
		OperationResult<List<SKDCard>> GetCards(Guid clientUID, CardFilter filter);

		[OperationContract]
		OperationResult<SKDCard> GetSingleCard(Guid clientUID, Guid uid);

		[OperationContract]
		OperationResult<List<SKDCard>> GetEmployeeCards(Guid clientUID, Guid employeeUID);

		[OperationContract]
		OperationResult<bool> AddCard(Guid clientUID, SKDCard item, string employeeName);

		[OperationContract]
		OperationResult<bool> EditCard(Guid clientUID, SKDCard item, string employeeName);

		[OperationContract]
		OperationResult<bool> DeleteCardFromEmployee(Guid clientUID, SKDCard item, string employeeName, string reason = null);

		[OperationContract]
		OperationResult DeletedCard(Guid clientUID, SKDCard card);

		[OperationContract]
		OperationResult SaveCardTemplate(Guid clientUID, SKDCard card);

		[OperationContract]
		OperationResult<List<GKUser>> GetDbDeviceUsers(Guid deviceUID, List<Guid> deviceUIDs);
		#endregion

		#region AccessTemplate
		[OperationContract]
		OperationResult<List<AccessTemplate>> GetAccessTemplates(Guid clientUID, AccessTemplateFilter filter);

		[OperationContract]
		OperationResult<bool> SaveAccessTemplate(Guid clientUID, AccessTemplate item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedAccessTemplate(Guid clientUID, AccessTemplate item);

		[OperationContract]
		OperationResult RestoreAccessTemplate(Guid clientUID, AccessTemplate item);
		#endregion

		#region Organisation
		[OperationContract]
		OperationResult<List<Organisation>> GetOrganisations(Guid clientUID, OrganisationFilter filter);

		[OperationContract]
		OperationResult<bool> SaveOrganisation(Guid clientUID, OrganisationDetails Organisation, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedOrganisation(Guid clientUID, Guid uid, string name);

		[OperationContract]
		OperationResult AddOrganisationDoor(Guid clientUID, Organisation organisation, Guid doorUID);

		[OperationContract]
		OperationResult RemoveOrganisationDoor(Guid clientUID, Organisation organisation, Guid doorUID);

		[OperationContract]
		OperationResult SaveOrganisationUsers(Guid clientUID, Organisation organisation);

		[OperationContract]
		OperationResult<OrganisationDetails> GetOrganisationDetails(Guid clientUID, Guid uid);

		[OperationContract]
		OperationResult SaveOrganisationChief(Guid clientUID, Guid uid, Guid? chiefUID, string name);

		[OperationContract]
		OperationResult SaveOrganisationHRChief(Guid clientUID, Guid uid, Guid? chiefUID, string name);

		[OperationContract]
		OperationResult RestoreOrganisation(Guid clientUID, Guid uid, string name);

		[OperationContract]
		OperationResult<bool> IsAnyOrganisationItems(Guid clientUID, Guid uid);

		#endregion

		#region AdditionalColumnType
		[OperationContract]
		OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(Guid clientUID, AdditionalColumnTypeFilter filter);

		[OperationContract]
		OperationResult<bool> SaveAdditionalColumnType(Guid clientUID, AdditionalColumnType item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedAdditionalColumnType(Guid clientUID, Guid uid, string name);

		[OperationContract]
		OperationResult RestoreAdditionalColumnType(Guid clientUID, Guid uid, string name);
		#endregion

		#region NightSettings
		[OperationContract]
		OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid clientUID, Guid organisationUID);

		[OperationContract]
		OperationResult SaveNightSettings(Guid clientUID, NightSettings nightSettings);
		#endregion

		#region PassCardTemplate
		[OperationContract]
		OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(Guid clientUID, PassCardTemplateFilter filter);

		[OperationContract]
		OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid clientUID, Guid uid);

		[OperationContract]
		OperationResult<bool> SavePassCardTemplate(Guid clientUID, PassCardTemplate item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedPassCardTemplate(Guid clientUID, Guid uid, string name);

		[OperationContract]
		OperationResult RestorePassCardTemplate(Guid clientUID, Guid uid, string name);
		#endregion

		[OperationContract]
		OperationResult GenerateEmployeeDays(Guid clientUID);

		[OperationContract]
		OperationResult GenerateJournal(Guid clientUID);

		[OperationContract]
		OperationResult GenerateTestData(Guid clientUID, bool isAscending);

		[OperationContract]
		OperationResult SaveJournalVideoUID(Guid clientUID, Guid journaItemUID, Guid videoUID, Guid cameraUID);

		[OperationContract]
		OperationResult SaveJournalCameraUID(Guid clientUID, Guid journaItemUID, Guid CameraUID);

		#region GKSchedule
		[OperationContract]
		OperationResult<List<GKSchedule>> GetGKSchedules(Guid clientUID);

		[OperationContract]
		OperationResult<bool> SaveGKSchedule(Guid clientUID, GKSchedule item, bool isNew);

		[OperationContract]
		OperationResult<bool> DeleteGKSchedule(Guid clientUID, GKSchedule item);
		#endregion

		#region GKDaySchedule
		[OperationContract]
		OperationResult<List<GKDaySchedule>> GetGKDaySchedules(Guid clientUID);

		[OperationContract]
		OperationResult<bool> SaveGKDaySchedule(Guid clientUID, GKDaySchedule item, bool isNew);

		[OperationContract]
		OperationResult<bool> DeleteGKDaySchedule(Guid clientUID, GKDaySchedule item);
		#endregion

		#region Export
		[OperationContract]
		OperationResult ExportOrganisation(Guid clientUID, ExportFilter filter);

		[OperationContract]
		OperationResult ImportOrganisation(Guid clientUID, ImportFilter filter);

		[OperationContract]
		OperationResult ExportOrganisationList(Guid clientUID, ExportFilter filter);

		[OperationContract]
		OperationResult ImportOrganisationList(Guid clientUID, ImportFilter filter);

		[OperationContract]
		OperationResult ExportJournal(Guid clientUID, JournalExportFilter filter);

		[OperationContract]
		OperationResult ExportConfiguration(Guid clientUID, ConfigurationExportFilter filter);
		#endregion

		#region CurrentConsumption
		[OperationContract]
		OperationResult SaveCurrentConsumption(Guid clientUID, CurrentConsumption item);

		[OperationContract]
		OperationResult<List<CurrentConsumption>> GetCurrentConsumption(Guid clientUID, CurrentConsumptionFilter filter);
		#endregion
	}
}