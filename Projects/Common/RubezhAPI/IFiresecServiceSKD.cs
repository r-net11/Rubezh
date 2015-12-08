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
		OperationResult<List<ShortEmployee>> GetEmployeeList(EmployeeFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult<Employee> GetEmployeeDetails(Guid uid, Guid clientUID);

		[OperationContract]
		OperationResult<bool> SaveEmployee(Employee item, bool isNew, Guid clientUID);

		[OperationContract]
		OperationResult MarkDeletedEmployee(Guid uid, string name, bool isEmployee, Guid clientUID);

		[OperationContract]
		OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate, Guid clientUID);

		[OperationContract]
		Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate, Guid clientUID);

		[OperationContract]
		OperationResult SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name, Guid clientUID);

		[OperationContract]
		OperationResult SaveEmployeePosition(Guid uid, Guid? positionUid, string name, Guid clientUID);

		[OperationContract]
		OperationResult RestoreEmployee(Guid uid, string name, bool isEmployee, Guid clientUID);
		#endregion

		#region Department
		[OperationContract]
		OperationResult<List<ShortDepartment>> GetDepartmentList(DepartmentFilter filter, Guid clientUID);//

		[OperationContract]
		OperationResult<Department> GetDepartmentDetails(Guid uid, Guid clientUID);//

		[OperationContract]
		OperationResult<bool> SaveDepartment(Department item, bool isNew, Guid clientUID);//

		[OperationContract]
		OperationResult MarkDeletedDepartment(ShortDepartment department, Guid clientUID);//

		[OperationContract]
		OperationResult SaveDepartmentChief(Guid uid, Guid? chiefUID, string name, Guid clientUID);//

		[OperationContract]
		OperationResult RestoreDepartment(ShortDepartment department, Guid clientUID);//

		[OperationContract]
		OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid, Guid clientUID);//

		[OperationContract]
		OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid, Guid clientUID);//
		#endregion

		#region Position
		[OperationContract]
		OperationResult<List<ShortPosition>> GetPositionList(PositionFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult<List<Guid>> GetPositionEmployees(Guid uid, Guid clientUID);

		[OperationContract]
		OperationResult<Position> GetPositionDetails(Guid uid, Guid clientUID);

		[OperationContract]
		OperationResult<bool> SavePosition(Position item, bool isNew, Guid clientUID);

		[OperationContract]
		OperationResult MarkDeletedPosition(Guid uid, string name, Guid clientUID);

		[OperationContract]
		OperationResult RestorePosition(Guid uid, string name, Guid clientUID);
		#endregion

		#region Card
		[OperationContract]
		OperationResult<List<SKDCard>> GetCards(CardFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult<SKDCard> GetSingleCard(Guid uid, Guid clientUID);

		[OperationContract]
		OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID, Guid clientUID);

		[OperationContract]
		OperationResult<bool> AddCard(SKDCard item, string employeeName, Guid clientUID);

		[OperationContract]
		OperationResult<bool> EditCard(SKDCard item, string employeeName, Guid clientUID);

		[OperationContract]
		OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, Guid clientUID, string reason = null);

		[OperationContract]
		OperationResult DeletedCard(SKDCard card, Guid clientUID);

		[OperationContract]
		OperationResult SaveCardTemplate(SKDCard card, Guid clientUID);
		#endregion

		#region AccessTemplate
		[OperationContract]
		OperationResult<List<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew, Guid clientUID);

		[OperationContract]
		OperationResult MarkDeletedAccessTemplate(AccessTemplate item, Guid clientUID);

		[OperationContract]
		OperationResult RestoreAccessTemplate(AccessTemplate item, Guid clientUID);
		#endregion

		#region Organisation
		[OperationContract]
		OperationResult<List<Organisation>> GetOrganisations(OrganisationFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult<bool> SaveOrganisation(OrganisationDetails Organisation, bool isNew, Guid clientUID);

		[OperationContract]
		OperationResult MarkDeletedOrganisation(Guid uid, string name, Guid clientUID);

		[OperationContract]
		OperationResult AddOrganisationDoor(Organisation organisation, Guid doorUID, Guid clientUID);

		[OperationContract]
		OperationResult RemoveOrganisationDoor(Organisation organisation, Guid doorUID, Guid clientUID);

		[OperationContract]
		OperationResult SaveOrganisationUsers(Organisation organisation, Guid clientUID);

		[OperationContract]
		OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid, Guid clientUID);

		[OperationContract]
		OperationResult SaveOrganisationChief(Guid uid, Guid? chiefUID, string name, Guid clientUID);

		[OperationContract]
		OperationResult SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name, Guid clientUID);

		[OperationContract]
		OperationResult RestoreOrganisation(Guid uid, string name, Guid clientUID);

		[OperationContract]
		OperationResult<bool> IsAnyOrganisationItems(Guid uid, Guid clientUID);

		#endregion

		#region AdditionalColumnType
		[OperationContract]
		OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew, Guid clientUID);

		[OperationContract]
		OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name, Guid clientUID);

		[OperationContract]
		OperationResult RestoreAdditionalColumnType(Guid uid, string name, Guid clientUID);
		#endregion

		#region NightSettings
		[OperationContract]
		OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID, Guid clientUID);

		[OperationContract]
		OperationResult SaveNightSettings(NightSettings nightSettings, Guid clientUID);
		#endregion

		#region PassCardTemplate
		[OperationContract]
		OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid, Guid clientUID);

		[OperationContract]
		OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew, Guid clientUID);

		[OperationContract]
		OperationResult MarkDeletedPassCardTemplate(Guid uid, string name, Guid clientUID);

		[OperationContract]
		OperationResult RestorePassCardTemplate(Guid uid, string name, Guid clientUID);
		#endregion

		[OperationContract]
		OperationResult GenerateEmployeeDays(Guid clientUID);

		[OperationContract]
		OperationResult GenerateJournal(Guid clientUID);

		[OperationContract]
		OperationResult GenerateTestData(bool isAscending, Guid clientUID);

		[OperationContract]
		OperationResult SaveJournalVideoUID(Guid journaItemUID, Guid videoUID, Guid cameraUID, Guid clientUID);

		[OperationContract]
		OperationResult SaveJournalCameraUID(Guid journaItemUID, Guid CameraUID, Guid clientUID);

		#region GKSchedule
		[OperationContract]
		OperationResult<List<GKSchedule>> GetGKSchedules(Guid clientUID);

		[OperationContract]
		OperationResult<bool> SaveGKSchedule(GKSchedule item, bool isNew, Guid clientUID);

		[OperationContract]
		OperationResult<bool> DeleteGKSchedule(GKSchedule item, Guid clientUID);
		#endregion

		#region GKDaySchedule
		[OperationContract]
		OperationResult<List<GKDaySchedule>> GetGKDaySchedules(Guid clientUID);

		[OperationContract]
		OperationResult<bool> SaveGKDaySchedule(GKDaySchedule item, bool isNew, Guid clientUID);

		[OperationContract]
		OperationResult<bool> DeleteGKDaySchedule(GKDaySchedule item, Guid clientUID);
		#endregion

		#region Export
		[OperationContract]
		OperationResult ExportOrganisation(ExportFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult ImportOrganisation(ImportFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult ExportOrganisationList(ExportFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult ImportOrganisationList(ImportFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult ExportJournal(JournalExportFilter filter, Guid clientUID);

		[OperationContract]
		OperationResult ExportConfiguration(ConfigurationExportFilter filter, Guid clientUID);
		#endregion

		#region CurrentConsumption
		[OperationContract]
		OperationResult SaveCurrentConsumption(CurrentConsumption item, Guid clientUID);

		[OperationContract]
		OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter filter, Guid clientUID);
		#endregion
	}
}