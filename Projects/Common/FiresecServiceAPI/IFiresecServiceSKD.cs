using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using FiresecAPI.GK;
using FiresecAPI.SKD;

namespace FiresecAPI
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public partial interface IFiresecServiceSKD
	{
		#region Employee
		[OperationContract]
		OperationResult<List<ShortEmployee>> GetEmployeeList(EmployeeFilter filter);

		[OperationContract]
		OperationResult<Employee> GetEmployeeDetails(Guid uid);

		[OperationContract]
		OperationResult<bool> SaveEmployee(Employee item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedEmployee(Guid uid, string name);

		[OperationContract]
		OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate);

		[OperationContract]
		Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate);

		[OperationContract]
		OperationResult SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name);

		[OperationContract]
		OperationResult SaveEmployeePosition(Guid uid, Guid? positionUid, string name);

		[OperationContract]
		OperationResult RestoreEmployee(Guid uid, string name);
		#endregion

		#region Department
		[OperationContract]
		OperationResult<List<ShortDepartment>> GetDepartmentList(DepartmentFilter filter);//

		[OperationContract]
		OperationResult<Department> GetDepartmentDetails(Guid uid);//

		[OperationContract]
		OperationResult<bool> SaveDepartment(Department item, bool isNew);//

		[OperationContract]
		OperationResult MarkDeletedDepartment(ShortDepartment department);//

		[OperationContract]
		OperationResult SaveDepartmentChief(Guid uid, Guid? chiefUID, string name);//
		
		[OperationContract]
		OperationResult RestoreDepartment(ShortDepartment department);//

		[OperationContract]
		OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid);//

		[OperationContract]
		OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid);//
		#endregion

		#region Position
		[OperationContract]
		OperationResult<List<ShortPosition>> GetPositionList(PositionFilter filter);

		[OperationContract]
		OperationResult<Position> GetPositionDetails(Guid uid);

		[OperationContract]
		OperationResult<bool> SavePosition(Position item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedPosition(Guid uid, string name);

		[OperationContract]
		OperationResult RestorePosition(Guid uid, string name);
		#endregion

		#region Card
		[OperationContract]
		OperationResult<List<SKDCard>> GetCards(CardFilter filter);

		[OperationContract]
		OperationResult<SKDCard> GetSingleCard(Guid uid);

		[OperationContract]
		OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID);

		[OperationContract]
		OperationResult<bool> AddCard(SKDCard item, string employeeName);

		[OperationContract]
		OperationResult<bool> EditCard(SKDCard item, string employeeName);

		[OperationContract]
		OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null);

		[OperationContract]
		OperationResult DeletedCard(SKDCard card);

		[OperationContract]
		OperationResult SaveCardTemplate(SKDCard card);
        #endregion

		#region AccessTemplate
		[OperationContract]
		OperationResult<List<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter);

		[OperationContract]
		OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedAccessTemplate(Guid uid, string name);

		[OperationContract]
		OperationResult RestoreAccessTemplate(Guid uid, string name);
		#endregion

		#region Organisation
		[OperationContract]
		OperationResult<List<Organisation>> GetOrganisations(OrganisationFilter filter);

		[OperationContract]
		OperationResult<bool> SaveOrganisation(OrganisationDetails Organisation, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedOrganisation(Guid uid, string name);

		[OperationContract]
		OperationResult AddOrganisationDoor(Organisation organisation, Guid doorUID);

		[OperationContract]
		OperationResult RemoveOrganisationDoor(Organisation organisation, Guid doorUID);

		[OperationContract]
		OperationResult SaveOrganisationUsers(Organisation organisation);

		[OperationContract]
		OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid);

		[OperationContract]
		OperationResult SaveOrganisationChief(Guid uid, Guid? chiefUID, string name);

		[OperationContract]
		OperationResult SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name);

		[OperationContract]
		OperationResult RestoreOrganisation(Guid uid, string name);

		[OperationContract]
		OperationResult<bool> IsAnyOrganisationItems(Guid uid);

		#endregion

		#region AdditionalColumnType
		[OperationContract]
		OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter);

		[OperationContract]
		OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name);

		[OperationContract]
		OperationResult RestoreAdditionalColumnType(Guid uid, string name);
		#endregion

		#region NightSettings
		[OperationContract]
		OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID);

		[OperationContract]
		OperationResult SaveNightSettings(NightSettings nightSettings);
		#endregion

		#region PassCardTemplate
		[OperationContract]
		OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter);

		[OperationContract]
		OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid);

		[OperationContract]
		OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedPassCardTemplate(Guid uid, string name);

		[OperationContract]
		OperationResult RestorePassCardTemplate(Guid uid, string name);
		#endregion

		[OperationContract]
		OperationResult GenerateEmployeeDays();

        [OperationContract]
        OperationResult GenerateJournal();

		[OperationContract]
		OperationResult GenerateTestData(bool isAscending);

		[OperationContract]
		OperationResult SaveJournalVideoUID(Guid journaItemUID, Guid videoUID, Guid cameraUID);

		[OperationContract]
		OperationResult SaveJournalCameraUID(Guid journaItemUID, Guid CameraUID);

		#region GKSchedule
		[OperationContract]
		OperationResult<List<GKSchedule>> GetGKSchedules();

		[OperationContract]
		OperationResult SaveGKSchedule(GKSchedule item, bool isNew);

		[OperationContract]
		OperationResult DeleteGKSchedule(GKSchedule item);
		#endregion

		#region GKDaySchedule
		[OperationContract]
		OperationResult<List<GKDaySchedule>> GetGKDaySchedules();

		[OperationContract]
		OperationResult SaveGKDaySchedule(GKDaySchedule item, bool isNew);

		[OperationContract]
		OperationResult DeleteGKDaySchedule(GKDaySchedule item);
		#endregion

		#region Export
		[OperationContract]
		OperationResult ExportOrganisation(ExportFilter filter);

		[OperationContract]
		OperationResult ImportOrganisation(ImportFilter filter);

		[OperationContract]
		OperationResult ExportOrganisationList(ExportFilter filter);

		[OperationContract]
		OperationResult ImportOrganisationList(ImportFilter filter);

		[OperationContract]
		OperationResult ExportJournal(JournalExportFilter filter);

		[OperationContract]
		OperationResult ExportConfiguration(ConfigurationExportFilter filter);
		#endregion

		#region CurrentConsumption
		[OperationContract]
		OperationResult SaveCurrentConsumption(CurrentConsumption item);

		[OperationContract]
		OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter filter);
		#endregion

        [OperationContract]
        OperationResult BeginGetAsync(HRFilter filter);
    }
}