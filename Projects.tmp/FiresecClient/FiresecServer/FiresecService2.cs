using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FiresecServer
{
	public class FiresecService2 : IFiresecService
	{
		public OperationResult<bool> Connect(RubezhAPI.Models.ClientCredentials clientCredentials)
		{
			Console.WriteLine("{0}: Connect", DateTime.Now.ToLongTimeString());
			return new OperationResult<bool>();
		}

		public void Disconnect(Guid clientUID)
		{
			Console.WriteLine("{0}: Disconnect", DateTime.Now.ToLongTimeString());
		}

		public OperationResult<ServerState> GetServerState(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public PollResult Poll(Guid clientUID, int callbackIndex)
		{
			Console.WriteLine("{0}: -> Poll", DateTime.Now.ToLongTimeString());
			Thread.Sleep(30000);
			Console.WriteLine("{0}: Poll ->", DateTime.Now.ToLongTimeString());
			return new PollResult();
		}

		public void LayoutChanged(Guid clientUID, Guid layoutUID)
		{
			throw new NotImplementedException();
		}

		public string Test(Guid clientUID, string arg)
		{
			Console.WriteLine("{0}: Test", DateTime.Now.ToLongTimeString());
			return arg;
		}

		public OperationResult<RubezhAPI.Models.SecurityConfiguration> GetSecurityConfiguration(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public void SetSecurityConfiguration(Guid clientUID, RubezhAPI.Models.SecurityConfiguration securityConfiguration)
		{
			throw new NotImplementedException();
		}

		public string Ping(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> ResetDB(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.License.FiresecLicenseInfo> GetLicenseInfo(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<DateTime> GetMinJournalDateTime(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.Journal.JournalItem>> GetFilteredJournalItems(Guid clientUID, RubezhAPI.Journal.JournalFilter journalFilter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> BeginGetJournal(RubezhAPI.Journal.JournalFilter journalFilter, Guid clentUid, Guid journalClientUid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> AddJournalItem(Guid clientUID, RubezhAPI.Journal.JournalItem journalItem)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> BeginGetArchivePage(RubezhAPI.Journal.JournalFilter filter, int page, Guid clentUid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<int> GetArchiveCount(Guid clientUID, RubezhAPI.Journal.JournalFilter filter)
		{
			throw new NotImplementedException();
		}

		public List<string> GetFileNamesList(Guid clientUID, string directory)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, string> GetDirectoryHash(Guid clientUID, string directory)
		{
			throw new NotImplementedException();
		}

		public System.IO.Stream GetServerAppDataFile(Guid clientUID, string dirAndFileName)
		{
			throw new NotImplementedException();
		}

		public System.IO.Stream GetServerFile(Guid clientUID, string filePath)
		{
			throw new NotImplementedException();
		}

		public System.IO.Stream GetConfig(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public void SetRemoteConfig(System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}

		public void SetLocalConfig(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.Automation.OpcDaServer[]> GetOpcDaServers(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.Automation.OpcDaElement[]> GetOpcDaServerGroupAndTags(Guid clientUID, RubezhAPI.Automation.OpcDaServer server)
		{
			throw new NotImplementedException();
		}

		public OperationResult<OpcServerStatus> GetOpcDaServerStatus(Guid clientUID, RubezhAPI.Automation.OpcDaServer server)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.Automation.OpcDaTagValue[]> ReadOpcDaServerTags(Guid clientUID, RubezhAPI.Automation.OpcDaServer server)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> WriteOpcDaTag(Guid clientUID, Guid serverId, object value)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.ShortEmployee>> GetEmployeeList(Guid clientUID, RubezhAPI.SKD.EmployeeFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.SKD.Employee> GetEmployeeDetails(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveEmployee(Guid clientUID, RubezhAPI.SKD.Employee item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> MarkDeletedEmployee(Guid clientUID, Guid uid, string name, bool isEmployee)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.SKD.TimeTrackResult> GetTimeTracks(Guid clientUID, RubezhAPI.SKD.EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			throw new NotImplementedException();
		}

		public System.IO.Stream GetTimeTracksStream(Guid clientUID, RubezhAPI.SKD.EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveEmployeeDepartment(Guid clientUID, Guid uid, Guid? departmentUid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveEmployeePosition(Guid clientUID, Guid uid, Guid? positionUid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestoreEmployee(Guid clientUID, Guid uid, string name, bool isEmployee)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.ShortDepartment>> GetDepartmentList(Guid clientUID, RubezhAPI.SKD.DepartmentFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.SKD.Department> GetDepartmentDetails(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveDepartment(Guid clientUID, RubezhAPI.SKD.Department item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> MarkDeletedDepartment(Guid clientUID, RubezhAPI.SKD.ShortDepartment department)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveDepartmentChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestoreDepartment(Guid clientUID, RubezhAPI.SKD.ShortDepartment department)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.ShortPosition>> GetPositionList(Guid clientUID, RubezhAPI.SKD.PositionFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<Guid>> GetPositionEmployees(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.SKD.Position> GetPositionDetails(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SavePosition(Guid clientUID, RubezhAPI.SKD.Position item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> MarkDeletedPosition(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestorePosition(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.SKDCard>> GetCards(Guid clientUID, RubezhAPI.SKD.CardFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.SKD.SKDCard> GetSingleCard(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.SKDCard>> GetEmployeeCards(Guid clientUID, Guid employeeUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> AddCard(Guid clientUID, RubezhAPI.SKD.SKDCard item, string employeeName)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> EditCard(Guid clientUID, RubezhAPI.SKD.SKDCard item, string employeeName)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> DeleteCardFromEmployee(Guid clientUID, RubezhAPI.SKD.SKDCard item, string employeeName, string reason = null)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> DeletedCard(Guid clientUID, RubezhAPI.SKD.SKDCard card)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveCardTemplate(Guid clientUID, RubezhAPI.SKD.SKDCard card)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.GK.GKUser>> GetDbDeviceUsers(Guid deviceUID, List<Guid> deviceUIDs)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.AccessTemplate>> GetAccessTemplates(Guid clientUID, RubezhAPI.SKD.AccessTemplateFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveAccessTemplate(Guid clientUID, RubezhAPI.SKD.AccessTemplate item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<string>> MarkDeletedAccessTemplate(Guid clientUID, RubezhAPI.SKD.AccessTemplate item)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestoreAccessTemplate(Guid clientUID, RubezhAPI.SKD.AccessTemplate item)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.Organisation>> GetOrganisations(Guid clientUID, RubezhAPI.SKD.OrganisationFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveOrganisation(Guid clientUID, RubezhAPI.SKD.OrganisationDetails Organisation, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> MarkDeletedOrganisation(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> AddOrganisationDoor(Guid clientUID, RubezhAPI.SKD.Organisation organisation, Guid doorUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RemoveOrganisationDoor(Guid clientUID, RubezhAPI.SKD.Organisation organisation, Guid doorUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveOrganisationUsers(Guid clientUID, RubezhAPI.SKD.Organisation organisation)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.SKD.OrganisationDetails> GetOrganisationDetails(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveOrganisationChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveOrganisationHRChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestoreOrganisation(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> IsAnyOrganisationItems(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.AdditionalColumnType>> GetAdditionalColumnTypes(Guid clientUID, RubezhAPI.SKD.AdditionalColumnTypeFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveAdditionalColumnType(Guid clientUID, RubezhAPI.SKD.AdditionalColumnType item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> MarkDeletedAdditionalColumnType(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestoreAdditionalColumnType(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.SKD.NightSettings> GetNightSettingsByOrganisation(Guid clientUID, Guid organisationUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveNightSettings(Guid clientUID, RubezhAPI.SKD.NightSettings nightSettings)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.ShortPassCardTemplate>> GetPassCardTemplateList(Guid clientUID, RubezhAPI.SKD.PassCardTemplateFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.SKD.PassCardTemplate> GetPassCardTemplateDetails(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SavePassCardTemplate(Guid clientUID, RubezhAPI.SKD.PassCardTemplate item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> MarkDeletedPassCardTemplate(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestorePassCardTemplate(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GenerateEmployeeDays(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GenerateJournal(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GenerateTestData(Guid clientUID, bool isAscending)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveJournalVideoUID(Guid clientUID, Guid journaItemUID, Guid videoUID, Guid cameraUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveJournalCameraUID(Guid clientUID, Guid journaItemUID, Guid CameraUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.GK.GKSchedule>> GetGKSchedules(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveGKSchedule(Guid clientUID, RubezhAPI.GK.GKSchedule item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> DeleteGKSchedule(Guid clientUID, RubezhAPI.GK.GKSchedule item)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.GK.GKDaySchedule>> GetGKDaySchedules(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveGKDaySchedule(Guid clientUID, RubezhAPI.GK.GKDaySchedule item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> DeleteGKDaySchedule(Guid clientUID, RubezhAPI.GK.GKDaySchedule item)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> ExportOrganisation(Guid clientUID, RubezhAPI.SKD.ExportFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> ImportOrganisation(Guid clientUID, RubezhAPI.SKD.ImportFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> ExportOrganisationList(Guid clientUID, RubezhAPI.SKD.ExportFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> ImportOrganisationList(Guid clientUID, RubezhAPI.SKD.ImportFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> ExportJournal(Guid clientUID, RubezhAPI.SKD.JournalExportFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> ExportConfiguration(Guid clientUID, RubezhAPI.SKD.ConfigurationExportFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveCurrentConsumption(Guid clientUID, RubezhAPI.GK.CurrentConsumption item)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.GK.CurrentConsumption>> GetCurrentConsumption(Guid clientUID, RubezhAPI.GK.CurrentConsumptionFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.DayInterval>> GetDayIntervals(Guid clientUID, RubezhAPI.SKD.DayIntervalFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveDayInterval(Guid clientUID, RubezhAPI.SKD.DayInterval item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> MarkDeletedDayInterval(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestoreDayInterval(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.Holiday>> GetHolidays(Guid clientUID, RubezhAPI.SKD.HolidayFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveHoliday(Guid clientUID, RubezhAPI.SKD.Holiday item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> MarkDeletedHoliday(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestoreHoliday(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.ScheduleScheme>> GetScheduleSchemes(Guid clientUID, RubezhAPI.SKD.ScheduleSchemeFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveScheduleScheme(Guid clientUID, RubezhAPI.SKD.ScheduleScheme item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> MarkDeletedScheduleScheme(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestoreScheduleScheme(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.Schedule>> GetSchedules(Guid clientUID, RubezhAPI.SKD.ScheduleFilter filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SaveSchedule(Guid clientUID, RubezhAPI.SKD.Schedule item, bool isNew)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> MarkDeletedSchedule(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RestoreSchedule(Guid clientUID, Guid uid, string name)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.TimeTrackDocument>> GetTimeTrackDocument(Guid clientUID, Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> AddTimeTrackDocument(Guid clientUID, RubezhAPI.SKD.TimeTrackDocument timeTrackDocument)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> EditTimeTrackDocument(Guid clientUID, RubezhAPI.SKD.TimeTrackDocument timeTrackDocument)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RemoveTimeTrackDocument(Guid clientUID, Guid timeTrackDocumentUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.SKD.TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid clientUID, Guid organisationUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> AddTimeTrackDocumentType(Guid clientUID, RubezhAPI.SKD.TimeTrackDocumentType timeTrackDocumentType)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> EditTimeTrackDocumentType(Guid clientUID, RubezhAPI.SKD.TimeTrackDocumentType timeTrackDocumentType)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RemoveTimeTrackDocumentType(Guid clientUID, Guid timeTrackDocumentTypeUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> AddCustomPassJournal(Guid clientUID, Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> EditPassJournal(Guid clientUID, Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> DeletePassJournal(Guid clientUID, Guid uid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> DeleteAllPassJournalItems(Guid clientUID, Guid uid, DateTime enterTime, DateTime exitTime)
		{
			throw new NotImplementedException();
		}

		public OperationResult<DateTime> GetPassJournalMinDate(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<DateTime> GetCardsMinDate(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public void CancelGKProgress(Guid clientUID, Guid progressCallbackUID, string userName)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GKWriteConfiguration(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.GK.GKDeviceConfiguration> GKReadConfiguration(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.GK.GKDevice> GKAutoSearch(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GKUpdateFirmware(Guid clientUID, Guid deviceUID, List<byte> firmwareBytes)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GKSyncronyseTime(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> GKGetDeviceInfo(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.Journal.JournalItem> GKReadJournalItem(Guid clientUID, Guid deviceUID, int no)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GKSetSingleParameter(Guid clientUID, Guid objectUID, List<byte> parameterBytes, List<RubezhAPI.GK.GKProperty> deviceProperties)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.GK.GKProperty>> GKGetSingleParameter(Guid clientUID, Guid objectUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GKRewriteAllSchedules(Guid clientUID, Guid gkDeviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GKSetSchedule(Guid clientUID, RubezhAPI.GK.GKSchedule schedule)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GKGetUsers(Guid clientUID, Guid gkDeviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> GKRewriteUsers(Guid clientUID, Guid gkDeviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<RubezhAPI.GK.GKUser>> GetGKUsers(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RewritePmfUsers(Guid clientUID, Guid uid, List<RubezhAPI.GK.GKUser> users)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<byte>> GKGKHash(Guid clientUID, Guid gkDeviceUID)
		{
			throw new NotImplementedException();
		}

		public RubezhAPI.GK.GKStates GKGetStates(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public void GKExecuteDeviceCommand(Guid clientUID, Guid deviceUID, RubezhAPI.GK.GKStateBit stateBit)
		{
			throw new NotImplementedException();
		}

		public void GKReset(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKResetFire1(Guid clientUID, Guid zoneUID)
		{
			throw new NotImplementedException();
		}

		public void GKResetFire2(Guid clientUID, Guid zoneUID)
		{
			throw new NotImplementedException();
		}

		public void GKSetAutomaticRegime(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKSetManualRegime(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKSetIgnoreRegime(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKTurnOn(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKTurnOnNow(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKTurnOnInAutomatic(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKTurnOnNowInAutomatic(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKTurnOff(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKTurnOffNow(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKTurnOffInAutomatic(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKTurnOffNowInAutomatic(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKStop(Guid clientUID, Guid uid, RubezhAPI.GK.GKBaseObjectType objectType)
		{
			throw new NotImplementedException();
		}

		public void GKStartMeasureMonitoring(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public void GKTurnOnNowGlobalPimsInAutomatic(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public void GKTurnOffNowGlobalPimsInAutomatic(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		public void GKStopMeasureMonitoring(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<uint> GKGetReaderCode(Guid clientUID, Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public void GKOpenSKDZone(Guid clientUID, Guid zoneUID)
		{
			throw new NotImplementedException();
		}

		public void GKCloseSKDZone(Guid clientUID, Guid zoneUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.GK.CurrentConsumption> GetAlsMeasure(Guid clientUID, Guid alsUid)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<RubezhAPI.Automation.Argument> args)
		{
			throw new NotImplementedException();
		}

		public void ProcedureCallbackResponse(Guid clientUID, Guid callbackUID, object value)
		{
			throw new NotImplementedException();
		}

		public RubezhAPI.AutomationCallback.ProcedureProperties GetProperties(Guid clientUID, Guid layoutUID)
		{
			throw new NotImplementedException();
		}

		public void SetVariableValue(Guid clientUID, Guid variableUid, object value)
		{
			throw new NotImplementedException();
		}

		public void AddJournalItemA(Guid clientUID, string message, Guid? objectUID = null)
		{
			throw new NotImplementedException();
		}

		public void ControlGKDevice(Guid clientUID, Guid deviceUid, RubezhAPI.GK.GKStateBit command)
		{
			throw new NotImplementedException();
		}

		public void StartRecord(Guid clientUID, Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			throw new NotImplementedException();
		}

		public void StopRecord(Guid clientUID, Guid cameraUid, Guid eventUid)
		{
			throw new NotImplementedException();
		}

		public void Ptz(Guid clientUID, Guid cameraUid, int ptzNumber)
		{
			throw new NotImplementedException();
		}

		public void RviAlarm(Guid clientUID, string name)
		{
			throw new NotImplementedException();
		}

		public void ControlFireZone(Guid clientUID, Guid uid, RubezhAPI.Automation.ZoneCommandType commandType)
		{
			throw new NotImplementedException();
		}

		public void ControlGuardZone(Guid clientUID, Guid uid, RubezhAPI.Automation.GuardZoneCommandType commandType)
		{
			throw new NotImplementedException();
		}

		public void ControlDirection(Guid clientUID, Guid uid, RubezhAPI.Automation.DirectionCommandType commandType)
		{
			throw new NotImplementedException();
		}

		public void ControlGKDoor(Guid clientUID, Guid uid, RubezhAPI.Automation.GKDoorCommandType commandType)
		{
			throw new NotImplementedException();
		}

		public void ControlDelay(Guid clientUID, Guid uid, RubezhAPI.Automation.DelayCommandType commandType)
		{
			throw new NotImplementedException();
		}

		public void ControlPumpStation(Guid clientUID, Guid uid, RubezhAPI.Automation.PumpStationCommandType commandType)
		{
			throw new NotImplementedException();
		}

		public void ControlMPT(Guid clientUID, Guid uid, RubezhAPI.Automation.MPTCommandType commandType)
		{
			throw new NotImplementedException();
		}

		public void ExportJournalA(Guid clientUID, bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			throw new NotImplementedException();
		}

		public void ExportOrganisationA(Guid clientUID, bool isWithDeleted, Guid organisationUid, string path)
		{
			throw new NotImplementedException();
		}

		public void ExportOrganisationListA(Guid clientUID, bool isWithDeleted, string path)
		{
			throw new NotImplementedException();
		}

		public void ExportConfigurationA(Guid clientUID, bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			throw new NotImplementedException();
		}

		public void ImportOrganisationA(Guid clientUID, bool isWithDeleted, string path)
		{
			throw new NotImplementedException();
		}

		public void ImportOrganisationListA(Guid clientUID, bool isWithDeleted, string path)
		{
			throw new NotImplementedException();
		}
	}
}
