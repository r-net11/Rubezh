﻿using OpcClientSdk;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.License;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;

namespace RubezhClient
{
	public interface ISafeFiresecService
	{
		OperationResult<bool> AddCard(SKDCard item, string employeeName);
		OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime);
		OperationResult<bool> AddJournalItem(JournalItem journalItem);
		void AddJournalItem(string message);
		OperationResult AddOrganisationDoor(Organisation item, Guid doorUID);
		void AddTask(Action task);
		OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument);
		OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType);
		OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page);
		OperationResult<bool> BeginGetJournal(JournalFilter filter, Guid clientUid);
		void CancelGKProgress(Guid progressCallbackUID, string userName);
		OperationResult<bool> Connect(ClientCredentials clientCredentials);
		void ControlDelay(Guid uid, DelayCommandType commandType);
		void ControlDirection(Guid uid, DirectionCommandType commandType);
		void ControlFireZone(Guid uid, ZoneCommandType commandType);
		void ControlGKDevice(Guid deviceUid, GKStateBit command);
		void ControlGKDoor(Guid uid, GKDoorCommandType commandType);
		void ControlGuardZone(Guid uid, GuardZoneCommandType commandType);
		void ControlMPT(Guid uid, MPTCommandType commandType);
		void ControlPumpStation(Guid uid, PumpStationCommandType commandType);
		OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime);
		OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null);
		OperationResult DeletedCard(SKDCard card);
		OperationResult<bool> DeleteGKDaySchedule(GKDaySchedule item);
		OperationResult<bool> DeleteGKSchedule(GKSchedule item);
		OperationResult DeletePassJournal(Guid uid);
		void Disconnect(Guid clientUID);
		void Dispose();
		OperationResult<bool> EditCard(SKDCard item, string employeeName);
		OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime);
		OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument);
		OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType);
		OperationResult ExportConfiguration(ConfigurationExportFilter filter);
		void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path);
		OperationResult ExportJournal(JournalExportFilter filter);
		void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path);
		OperationResult ExportOrganisation(ExportFilter filter);
		void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path);
		OperationResult ExportOrganisationList(ExportFilter filter);
		void ExportOrganisationList(bool isWithDeleted, string path);
		IFiresecService FiresecService { get; set; }
		OperationResult GenerateEmployeeDays();
		OperationResult GenerateJournal();
		OperationResult GenerateTestData(bool isAscending);
		OperationResult<List<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter);
		OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter);
		OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid);
		OperationResult<int> GetArchiveCount(JournalFilter filter);
		OperationResult<List<SKDCard>> GetCards(CardFilter filter);
		OperationResult<DateTime> GetCardsMinDate();
		OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid);
		Stream GetConfig();
		T GetConfiguration<T>(string filename) where T : VersionedConfiguration, new();
		OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter item);
		OperationResult<List<DayInterval>> GetDayIntervals(DayIntervalFilter filter);
		OperationResult<Department> GetDepartmentDetails(Guid uid);
		OperationResult<List<ShortDepartment>> GetDepartmentList(DepartmentFilter filter);
		Dictionary<string, string> GetDirectoryHash(string directory);
		OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID);
		OperationResult<Employee> GetEmployeeDetails(Guid uid);
		OperationResult<List<ShortEmployee>> GetEmployeeList(EmployeeFilter filter);
		List<string> GetFileNamesList(string directory);
		OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter);
		OperationResult<List<GKDaySchedule>> GetGKDaySchedules();
		OperationResult<List<GKSchedule>> GetGKSchedules();
		OperationResult<List<Holiday>> GetHolidays(HolidayFilter filter);
		OperationResult<FiresecLicenseInfo> GetLicenseInfo();
		OperationResult<DateTime> GetMinJournalDateTime();
		OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID);
		OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid);
		OperationResult<List<Organisation>> GetOrganisations(OrganisationFilter filter);
		OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid);
		OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid);
		OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter);
		OperationResult<DateTime> GetPassJournalMinDate();
		OperationResult<Position> GetPositionDetails(Guid uid);
		OperationResult<List<Guid>> GetPositionEmployees(Guid uid);
		OperationResult<List<ShortPosition>> GetPositionList(PositionFilter filter);
		ProcedureProperties GetProperties(Guid layoutUID);
		OperationResult<List<Schedule>> GetSchedules(ScheduleFilter filter);
		OperationResult<List<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter);
		OperationResult<SecurityConfiguration> GetSecurityConfiguration();
		void SetSecurityConfiguration(SecurityConfiguration securityConfiguration);
		Stream GetServerAppDataFile(string dirAndFileName);
		Stream GetServerFile(string filePath);
		OperationResult<ServerState> GetServerState();
		OperationResult<SKDCard> GetSingleCard(Guid uid);
		OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime);
		OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID);
		OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate);
		Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate);
		void GKAddMessage(JournalEventNameType journalEventNameType, string description);
		OperationResult<GKDevice> GKAutoSearch(GKDevice device);
		void GKCloseSKDZone(GKSKDZone zone);
		void GKExecuteDeviceCommand(GKDevice device, GKStateBit stateBit);
		OperationResult<string> GKGetDeviceInfo(GKDevice device);
		OperationResult<int> GKGetJournalItemsCount(GKDevice device);
		OperationResult<uint> GKGetReaderCode(GKDevice device);
		OperationResult<List<GKProperty>> GKGetSingleParameter(GKBase gkBase);
		GKStates GKGetStates();
		OperationResult<bool> GKGetUsers(GKDevice device);
		OperationResult<List<GKUser>> GetDbDeviceUsers(Guid deviceUID, List<Guid> doorUIDs);
		OperationResult<List<byte>> GKGKHash(GKDevice device);
		void GKOpenSKDZone(GKSKDZone zone);
		OperationResult<GKDeviceConfiguration> GKReadConfiguration(GKDevice device);
		OperationResult<bool> GKReadConfigurationFromGKFile(GKDevice device);
		OperationResult<JournalItem> GKReadJournalItem(GKDevice device, int no);
		void GKReset(GKBase gkBase);
		void GKResetFire1(GKZone zone);
		void GKResetFire2(GKZone zone);
		OperationResult<bool> GKRewriteAllSchedules(GKDevice device);
		OperationResult<bool> GKRewriteUsers(Guid deviceUID);
		void GKSetAutomaticRegime(GKBase gkBase);
		void GKSetIgnoreRegime(GKBase gkBase);
		void GKSetManualRegime(GKBase gkBase);
		OperationResult<bool> GKSetSchedule(GKSchedule schedule);
		OperationResult<bool> GKSetSingleParameter(GKBase gkBase, List<byte> parameterBytes, List<GKProperty> deviceProperties = null);
		void GKStartMeasureMonitoring(GKDevice device);
		void GKStop(GKBase gkBase);
		void GKStopMeasureMonitoring(GKDevice device);
		OperationResult<bool> GKSyncronyseTime(GKDevice device);
		void GKTurnOff(GKBase gkBase);
		void GKTurnOffInAutomatic(GKBase gkBase);
		void GKTurnOffNow(GKBase gkBase);
		void GKTurnOnNowGlobalPimsInAutomatic();
		void GKTurnOffNowGlobalPimsInAutomatic();
		void GKTurnOffNowInAutomatic(GKBase gkBase);
		void GKTurnOn(GKBase gkBase);
		void GKTurnOnInAutomatic(GKBase gkBase);
		void GKTurnOnNow(GKBase gkBase);
		void GKTurnOnNowInAutomatic(GKBase gkBase);
		OperationResult<bool> GKUpdateFirmware(GKDevice device, List<byte> firmwareBytes);
		OperationResult<bool> GKWriteConfiguration(GKDevice device);
		OperationResult ImportOrganisation(ImportFilter filter);
		void ImportOrganisation(bool isWithDeleted, string path);
		OperationResult ImportOrganisationList(ImportFilter filter);
		void ImportOrganisationList(bool isWithDeleted, string path);
		OperationResult<bool> IsAnyOrganisationItems(Guid uid);
		bool LayoutChanged(Guid clientUID, Guid layoutUID);
		OperationResult MarkDeletedAccessTemplate(AccessTemplate item);
		OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name);
		OperationResult MarkDeletedDayInterval(Guid uid, string name);
		OperationResult MarkDeletedDepartment(ShortDepartment item);
		OperationResult MarkDeletedEmployee(Guid uid, string name, bool isEmployee);
		OperationResult MarkDeletedHoliday(Guid uid, string name);
		OperationResult MarkDeletedOrganisation(Guid uid, string name);
		OperationResult MarkDeletedPassCardTemplate(Guid uid, string name);
		OperationResult MarkDeletedPosition(Guid uid, string name);
		OperationResult MarkDeletedSchedule(Guid uid, string name);
		OperationResult MarkDeletedScheduleScheme(Guid uid, string name);
		string Ping();
		PollResult Poll(Guid clientUID, int callbackIndex);
		void ProcedureCallbackResponse(Guid procedureThreadUID, object value);
		void Ptz(Guid cameraUid, int ptzNumber);
		OperationResult RemoveOrganisationDoor(Organisation item, Guid doorUID);
		OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID);
		OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID);
		OperationResult ResetDB();
		OperationResult RestoreAccessTemplate(AccessTemplate item);
		OperationResult RestoreAdditionalColumnType(Guid uid, string name);
		OperationResult RestoreDayInterval(Guid uid, string name);
		OperationResult RestoreDepartment(ShortDepartment item);
		OperationResult RestoreEmployee(Guid uid, string name, bool isEmployee);
		OperationResult RestoreHoliday(Guid uid, string name);
		OperationResult RestoreOrganisation(Guid uid, string name);
		OperationResult RestorePassCardTemplate(Guid uid, string name);
		OperationResult RestorePosition(Guid uid, string name);
		OperationResult RestoreSchedule(Guid uid, string name);
		OperationResult RestoreScheduleScheme(Guid uid, string name);
		OperationResult<bool> RewritePmfUsers(Guid uid, List<GKUser> users);
		OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args);
		void RviAlarm(string name);
		OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew);
		OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew);
		OperationResult SaveCardTemplate(SKDCard item);
		OperationResult SaveCurrentConsumption(CurrentConsumption item);
		OperationResult<bool> SaveDayInterval(DayInterval item, bool isNew);
		OperationResult<bool> SaveDepartment(Department item, bool isNew);
		OperationResult SaveDepartmentChief(Guid uid, Guid? chiefUID, string name);
		OperationResult<bool> SaveEmployee(Employee item, bool isNew);
		OperationResult SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name);
		OperationResult SaveEmployeePosition(Guid uid, Guid? positionUid, string name);
		OperationResult<bool> SaveGKDaySchedule(GKDaySchedule item, bool isNew);
		OperationResult<bool> SaveGKSchedule(GKSchedule item, bool isNew);
		OperationResult<bool> SaveHoliday(Holiday item, bool isNew);
		OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID);
		OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID);
		OperationResult SaveNightSettings(NightSettings nightSettings);
		OperationResult<bool> SaveOrganisation(OrganisationDetails item, bool isNew);
		OperationResult SaveOrganisationChief(Guid uid, Guid? chiefUID, string name);
		OperationResult SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name);
		OperationResult SaveOrganisationUsers(Organisation item);
		OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew);
		OperationResult<bool> SavePosition(Position item, bool isNew);
		OperationResult<bool> SaveSchedule(Schedule item, bool isNew);
		OperationResult<bool> SaveScheduleScheme(ScheduleScheme item, bool isNew);
		void SetLocalConfig();
		void SetRemoteConfig(Stream stream);
		void SetVariableValue(Guid variableUid, object value);
		void StartOperationQueueThread();
		void StartPoll();
		void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout);
		void StopOperationQueueThread();
		void StopPoll();
		void StopRecord(Guid cameraUid, Guid eventUid);
		string Test(string arg);
		OperationResult<OpcDaServer[]> GetOpcDaServers(Guid clientUID);
		OperationResult<OpcServerStatus> GetOpcDaServerStatus(Guid clientUID, OpcDaServer server);
		OperationResult<OpcDaElement[]> GetOpcDaServerGroupAndTags(Guid clientUID, OpcDaServer server);

	}
}
