using System;
using System.Collections.Generic;
using System.IO;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		#region Employee
		public OperationResult<IEnumerable<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortEmployee>>>(() => FiresecService.GetEmployeeList(filter));
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Employee>>(() => FiresecService.GetEmployeeDetails(uid));
		}
		public OperationResult SaveEmployee(Employee item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployee(item, isNew));
		}
		public OperationResult MarkDeletedEmployee(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedEmployee(uid, name));
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeContext.Execute<OperationResult<TimeTrackResult>>(() => FiresecService.GetTimeTracks(filter, startDate, endDate));
			return result;
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeContext.Execute<Stream>(() => FiresecService.GetTimeTracksStream(filter, startDate, endDate));
			return result;
		}
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid departmentUid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployeeDepartment(uid, departmentUid, name));
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid positionUid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployeePosition(uid, positionUid, name));
		}
		public OperationResult RestoreEmployee(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreEmployee(uid, name));
		}
		#endregion

		#region Department
		public OperationResult<IEnumerable<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortDepartment>>>(() => FiresecService.GetDepartmentList(filter));
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Department>>(() => FiresecService.GetDepartmentDetails(uid));
		}
		public OperationResult SaveDepartment(Department item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartment(item, isNew));
		}
		public OperationResult MarkDeletedDepartment(ShortDepartment department)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDepartment(department));
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartmentChief(uid, chiefUID, name));
		}
		public OperationResult RestoreDepartment(ShortDepartment department)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreDepartment(department));
		}
		public OperationResult<IEnumerable<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Guid>>>(() => FiresecService.GetChildEmployeeUIDs(uid));
		}
		public OperationResult<IEnumerable<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Guid>>>(() => FiresecService.GetParentEmployeeUIDs(uid));
		}
		#endregion

		#region Position
		public OperationResult<IEnumerable<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortPosition>>>(() => FiresecService.GetPositionList(filter));
		}
		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Position>>(() => FiresecService.GetPositionDetails(uid));
		}
		public OperationResult SavePosition(Position item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePosition(item, isNew));
		}
		public OperationResult MarkDeletedPosition(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedPosition(uid, name));
		}

		public OperationResult RestorePosition(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestorePosition(uid, name));
		}
		#endregion

		#region Card
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDCard>>>(() => FiresecService.GetCards(filter));
		}
		public OperationResult<IEnumerable<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDCard>>>(() => FiresecService.GetEmployeeCards(employeeUID));
		}
		public OperationResult<bool> AddCard(SKDCard item, string employeeName)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.AddCard(item, employeeName));
		}
		public OperationResult<bool> EditCard(SKDCard item, string employeeName)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.EditCard(item, employeeName));
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.DeleteCardFromEmployee(item, employeeName, reason));
		}
		public OperationResult DeletedCard(SKDCard card)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeletedCard(card));
		}
		public OperationResult SaveCardTemplate(SKDCard item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveCardTemplate(item));
		}
		
		#endregion

		#region AccessTemplate
		public OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AccessTemplate>>>(() => FiresecService.GetAccessTemplates(filter));
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SaveAccessTemplate(item, isNew));
		}
		public OperationResult MarkDeletedAccessTemplate(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAccessTemplate(uid, name));
		}
		public OperationResult RestoreAccessTemplate(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreAccessTemplate(uid, name));
		}
		#endregion

		#region Organisation
		public OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Organisation>>>(() => FiresecService.GetOrganisations(filter));
		}
		public OperationResult SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisation(item, isNew));
		}
		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedOrganisation(uid, name));
		}
		public OperationResult AddOrganisationDoor(Organisation organisation, Guid doorUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.AddOrganisationDoor(organisation, doorUID));
		}
		public OperationResult RemoveOrganisationDoor(Organisation organisation, Guid doorUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RemoveOrganisationDoor(organisation, doorUID));
		}
		public OperationResult SaveOrganisationUsers(Organisation organisation)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationUsers(organisation));
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<OrganisationDetails>>(() => FiresecService.GetOrganisationDetails(uid));
		}
		public OperationResult SaveOrganisationChief(Guid uid, Guid chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationChief(uid, chiefUID, name));
		}
		public OperationResult SaveOrganisationHRChief(Guid uid, Guid chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationHRChief(uid, chiefUID, name));
		}
		public OperationResult RestoreOrganisation(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreOrganisation(uid, name));
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.IsAnyOrganisationItems(uid));
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<IEnumerable<ShortAdditionalColumnType>> GetAdditionalColumnTypeList(AdditionalColumnTypeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortAdditionalColumnType>>>(() => FiresecService.GetAdditionalColumnTypeList(filter));
		}
		public OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AdditionalColumnType>>>(() => FiresecService.GetAdditionalColumnTypes(filter));
		}
		public OperationResult<AdditionalColumnType> GetAdditionalColumnTypeDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<AdditionalColumnType>>(() => FiresecService.GetAdditionalColumnTypeDetails(uid));
		}
		public OperationResult SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAdditionalColumnType(item, isNew));
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAdditionalColumnType(uid, name));
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreAdditionalColumnType(uid, name));
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			return SafeContext.Execute<OperationResult<NightSettings>>(() => FiresecService.GetNightSettingsByOrganisation(organisationUID));
		}
		public OperationResult SaveNightSettings(NightSettings nightSettings)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveNightSettings(nightSettings));
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<IEnumerable<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortPassCardTemplate>>>(() => FiresecService.GetPassCardTemplateList(filter));
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<PassCardTemplate>>(() => FiresecService.GetPassCardTemplateDetails(uid));
		}
		public OperationResult SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePassCardTemplate(item, isNew));
		}
		public OperationResult MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedPassCardTemplate(uid, name));
		}
		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestorePassCardTemplate(uid, name));
		}
		#endregion

		public OperationResult ResetSKDDatabase()
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ResetSKDDatabase());
		}

		public OperationResult GenerateEmployeeDays()
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.GenerateEmployeeDays());
		}

		public OperationResult GenerateTestData(bool isAscending)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.GenerateTestData(isAscending));
		}

		public OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveJournalVideoUID(journalItemUID, videoUID, cameraUID));
		}

		public OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveJournalCameraUID(journalItemUID, CameraUID));
		}

		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules()
		{
			return SafeContext.Execute<OperationResult<List<GKSchedule>>>(() => FiresecService.GetGKSchedules());
		}

		public OperationResult SaveGKSchedule(GKSchedule item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveGKSchedule(item, isNew));
		}

		public OperationResult DeleteGKSchedule(GKSchedule item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeleteGKSchedule(item));
		}
		#endregion

		#region GKDaySchedule
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules()
		{
			return SafeContext.Execute<OperationResult<List<GKDaySchedule>>>(() => FiresecService.GetGKDaySchedules());
		}

		public OperationResult SaveGKDaySchedule(GKDaySchedule item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveGKDaySchedule(item, isNew));
		}

		public OperationResult DeleteGKDaySchedule(GKDaySchedule item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeleteGKDaySchedule(item));
		}
		#endregion

		#region Export
		public OperationResult ExportOrganisation(ExportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ExportOrganisation(filter));
		}
		public OperationResult ImportOrganisation(ImportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ImportOrganisation(filter));
		}
		public OperationResult ExportOrganisationList(ExportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ExportOrganisationList(filter));
		}
		public OperationResult ImportOrganisationList(ImportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ImportOrganisationList(filter));
		}
		public OperationResult ExportJournal(JournalExportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ExportJournal(filter));
		}
		public OperationResult ExportConfiguration(ConfigurationExportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ExportConfiguration(filter));
		}
		#endregion

		#region CurrentConsumption
		public OperationResult SaveCurrentConsumption(CurrentConsumption item)
		{
			return SafeContext.Execute(() => FiresecService.SaveCurrentConsumption(item));
		}
		public OperationResult<IEnumerable<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter item)
		{
			return SafeContext.Execute(() => FiresecService.GetCurrentConsumption(item));
		}
		#endregion
	}
}