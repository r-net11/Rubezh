using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using Common;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhLicense;
using RubezhAPI.License;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class SafeFiresecService : IFiresecService
	{
		public FiresecService FiresecService { get; set; }

		public SafeFiresecService()
		{
			FiresecService = new FiresecService();
		}

		public void BeginOperation(string operationName)
		{
		}

		public void EndOperation()
		{
		}

		OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> func, string operationName)
		{
			try
			{
				BeginOperation(operationName);
				var result = func();
				EndOperation();
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. operationName = " + operationName);
				return OperationResult<T>.FromError("Ошибка при выполнении операции на сервере" + "\n\r" + e.Message + "\n" + e.StackTrace);
			}
		}

		T SafeOperationCall<T>(Func<T> func, string operationName)
		{
			try
			{
				BeginOperation(operationName);
				var result = func();
				EndOperation();
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. operationName = " + operationName);
			}
			return default(T);
		}

		void SafeOperationCall(Action action, string operationName)
		{
			try
			{
				BeginOperation(operationName);
				action();
				EndOperation();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. operationName = " + operationName);
			}
		}

		public OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials, bool isNew)
		{
			return SafeOperationCall(() => { return FiresecService.Connect(uid, clientCredentials, isNew); }, "Connect");
		}

		public void Disconnect(Guid uid)
		{
			SafeOperationCall(() => { FiresecService.Disconnect(uid); }, "Disconnect");
		}

		public OperationResult<ServerState> GetServerState()
		{
			return SafeOperationCall(() => { return FiresecService.GetServerState(); }, "GetServerState");
		}

		public string Ping()
		{
			return SafeOperationCall(() => { return FiresecService.Ping(); }, "Ping");
		}

		public OperationResult ResetDB()
		{
			return SafeOperationCall(() => { return FiresecService.ResetDB(); }, "ResetDB");
		}

		public List<CallbackResult> Poll(Guid uid)
		{
			return SafeContext.Execute<List<CallbackResult>>(() => FiresecService.Poll(uid));
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); }, "GetSecurityConfiguration");
		}

		public List<string> GetFileNamesList(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetFileNamesList(directory); }, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetDirectoryHash(directory); }, "GetDirectoryHash");
		}

		public System.IO.Stream GetServerAppDataFile(string dirAndFileName)
		{
			return SafeOperationCall(() => { return FiresecService.GetServerAppDataFile(dirAndFileName); }, "GetServerAppDataFile");
		}

		public Stream GetConfig()
		{
			return SafeOperationCall(() => { return FiresecService.GetConfig(); }, "GetConfig");
		}

		public void SetRemoteConfig(Stream stream)
		{
			SafeOperationCall(() => { FiresecService.SetRemoteConfig(stream); }, "SetRemoteConfig");
		}

		public void SetLocalConfig()
		{
			SafeOperationCall(() => { FiresecService.SetLocalConfig(); }, "SetLocalConfig");
		}

		public string Test(string arg)
		{
			return SafeOperationCall(() => { return FiresecService.Test(arg); }, "Test");
		}

		public OperationResult<FiresecLicenseInfo> GetLicenseInfo()
		{
			return SafeOperationCall(() => { return FiresecService.GetLicenseInfo(); }, "GetLicenseInfo");
		}

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() => FiresecService.GetMinJournalDateTime());
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<JournalItem>>>(() => FiresecService.GetFilteredJournalItems(filter));
		}
		public OperationResult<bool> BeginGetJournal(JournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.BeginGetJournal(filter));
		}
		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			return SafeOperationCall(() => { return FiresecService.AddJournalItem(journalItem); }, "AddJournalItem");
		}

		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.BeginGetArchivePage(filter, page));
		}

		public OperationResult<int> GetArchiveCount(JournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<int>>(() => FiresecService.GetArchiveCount(filter));
		}
		#endregion

		#region GK

		public void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			SafeOperationCall(() => { FiresecService.CancelGKProgress(progressCallbackUID, userName); }, "CancelGKProgress");
		}

		public OperationResult<bool> GKWriteConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKWriteConfiguration(deviceUID); }, "GKWriteConfiguration");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadConfiguration(deviceUID); }, "GKReadConfiguration");
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadConfigurationFromGKFile(deviceUID); }, "GKReadConfigurationFromGKFile");
		}

		public Stream GetServerFile(string filePath)
		{
			return SafeOperationCall(() => { return FiresecService.GetServerFile(filePath); }, "GetServerFile");
		}

		public OperationResult<GKDevice> GKAutoSearch(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKAutoSearch(deviceUID); }, "GKAutoSearch");
		}

		public OperationResult<bool> GKUpdateFirmware(Guid deviceUID, List<byte> firmwareBytes)
		{
			return SafeOperationCall(() => { return FiresecService.GKUpdateFirmware(deviceUID, firmwareBytes); }, "GKUpdateFirmware");
		}

		public OperationResult<bool> GKSyncronyseTime(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKSyncronyseTime(deviceUID); }, "GKSyncronyseTime");
		}

		public OperationResult<string> GKGetDeviceInfo(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetDeviceInfo(deviceUID); }, "GKGetDeviceInfo");
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetJournalItemsCount(deviceUID); }, "GKGetJournalItemsCount");
		}

		public OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadJournalItem(deviceUID, no); }, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes, List<GKProperty> deviceProperties)
		{
			return SafeOperationCall(() => { return FiresecService.GKSetSingleParameter(objectUID, parameterBytes, deviceProperties); }, "GKSetSingleParameter");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(Guid objectUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetSingleParameter(objectUID); }, "GKGetSingleParameter");
		}

		public OperationResult<bool> GKRewriteAllSchedules(Guid gkDeviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKRewriteAllSchedules(gkDeviceUID); }, "GKRewriteAllSchedules");
		}

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule)
		{
			return SafeOperationCall(() => { return FiresecService.GKSetSchedule(schedule); }, "GKSetSchedule");
		}

		public OperationResult<bool> GKGetUsers(Guid gkDeviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetUsers(gkDeviceUID); }, "GKGetUsers");
		}

		public OperationResult<bool> GKRewriteUsers(Guid gkDeviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKRewriteUsers(gkDeviceUID); }, "GKRewriteUsers");
		}

		public OperationResult<List<MirrorUser>> GKReadMirrorUsers(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadMirrorUsers(deviceUID); }, "GKReadMirrorUsers");
		}

		public OperationResult<bool> GKWriteMirrorUsers(Guid deviceUID, List<MirrorUser> mirrorUsers)
		{
			return SafeOperationCall(() => { return FiresecService.GKWriteMirrorUsers(deviceUID, mirrorUsers); }, "GKWriteMirrorUsers");
		}

		public OperationResult<List<GKUser>> GetGKUsers(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetGKUsers(deviceUID); }, "GetGKUsers");
		}

		public OperationResult<bool> RewritePmfUsers(Guid uid, List<GKUser> users)
		{
			return SafeOperationCall(() => { return FiresecService.RewritePmfUsers(uid, users); }, "RewritePmfUsers");
		}

		public OperationResult<List<byte>> GKGKHash(Guid gkDeviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGKHash(gkDeviceUID); }, "GKGKHash");
		}

		public GKStates GKGetStates()
		{
			return SafeOperationCall(() => { return FiresecService.GKGetStates(); }, "GKGetStates");
		}
		public void GKExecuteDeviceCommand(Guid deviceUID, GKStateBit stateBit)
		{
			SafeOperationCall(() => { FiresecService.GKExecuteDeviceCommand(deviceUID, stateBit); }, "GKExecuteDeviceCommand");
		}

		public void GKReset(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKReset(uid, objectType); }, "GKReset");
		}

		public void GKResetFire1(Guid zoneUID)
		{
			SafeOperationCall(() => { FiresecService.GKResetFire1(zoneUID); }, "GKResetFire1");
		}

		public void GKResetFire2(Guid zoneUID)
		{
			SafeOperationCall(() => { FiresecService.GKResetFire2(zoneUID); }, "GKResetFire2");
		}

		public void GKSetAutomaticRegime(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKSetAutomaticRegime(uid, objectType); }, "GKSetAutomaticRegime");
		}

		public void GKSetManualRegime(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKSetManualRegime(uid, objectType); }, "GKSetManualRegime");
		}

		public void GKSetIgnoreRegime(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKSetIgnoreRegime(uid, objectType); }, "GKSetIgnoreRegime");
		}

		public void GKTurnOn(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOn(uid, objectType); }, "GKTurnOn");
		}

		public void GKTurnOnNow(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnNow(uid, objectType); }, "GKTurnOnNow");
		}

		public void GKTurnOnInAutomatic(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnInAutomatic(uid, objectType); }, "GKTurnOnInAutomatic");
		}

		public void GKTurnOnNowInAutomatic(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnNowInAutomatic(uid, objectType); }, "GKTurnOnNowInAutomatic");
		}

		public void GKTurnOff(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOff(uid, objectType); }, "GKTurnOff");
		}

		public void GKTurnOffNow(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffNow(uid, objectType); }, "GKTurnOffNow");
		}

		public void GKTurnOffInAutomatic(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffInAutomatic(uid, objectType); }, "GKTurnOffInAutomatic");
		}

		public void GKTurnOffNowInAutomatic(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffNowInAutomatic(uid, objectType); }, "GKTurnOffNowInAutomatic");
		}

		public void GKTurnOnNowGlobalPimsInAutomatic()
		{
			SafeOperationCall(() => FiresecService.GKTurnOnNowGlobalPimsInAutomatic(), "GKTurnOnNowGlobalPimsInAutomatic");
		}

		public void GKTurnOffNowGlobalPimsInAutomatic()
		{
			SafeOperationCall(() => FiresecService.GKTurnOffNowGlobalPimsInAutomatic(), "GKTurnOffNowGlobalPimsInAutomatic");
		}

		public void GKStop(Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKStop(uid, objectType); }, "GKStop");
		}

		public void GKStartMeasureMonitoring(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.GKStartMeasureMonitoring(deviceUID); }, "GKStartMeasureMonitoring");
		}

		public void GKStopMeasureMonitoring(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.GKStopMeasureMonitoring(deviceUID); }, "GKStopMeasureMonitoring");
		}

		public OperationResult<uint> GKGetReaderCode(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetReaderCode(deviceUID); }, "GKGetReaderCode");
		}

		public void GKOpenSKDZone(Guid zoneUID)
		{
			SafeOperationCall(() => { FiresecService.GKOpenSKDZone(zoneUID); }, "GKOpenSKDZone");
		}

		public void GKCloseSKDZone(Guid zoneUID)
		{
			SafeOperationCall(() => { FiresecService.GKCloseSKDZone(zoneUID); }, "GKCloseSKDZone");
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid)
		{
			return SafeOperationCall(() => FiresecService.GetAlsMeasure(alsUid), "GetAlsMeasure");
		}

		#endregion

		#region Automation

		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			return SafeOperationCall(() => { return FiresecService.RunProcedure(clientUID, procedureUID, args); }, "RunProcedure");
		}

		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value)
		{
			SafeOperationCall(() => FiresecService.ProcedureCallbackResponse(procedureThreadUID, value), "ProcedureCallbackResponse");
		}

		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetProperties(layoutUID); }, "GetProperties");
		}
				
		public void SetVariableValue(Guid variableUid, object value)
		{
			SafeOperationCall(() => FiresecService.SetVariableValue(variableUid, value), "SetVariableValue");
		}

		public Variable GetVariable(Guid variableUid)
		{
			return SafeOperationCall(() => { return FiresecService.GetVariable(variableUid); }, "GetVariable");
		}

		public void AddJournalItemA(string message)
		{
			SafeOperationCall(() => FiresecService.AddJournalItemA(message), "AddJournalItem");
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command)
		{
			SafeOperationCall(() => FiresecService.ControlGKDevice(deviceUid, command), "ControlGKDevice");
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			SafeOperationCall(() => FiresecService.StartRecord(cameraUid, journalItemUid, eventUid, timeout), "StartRecord");
		}

		public void StopRecord(Guid cameraUid, Guid eventUid)
		{
			SafeOperationCall(() => FiresecService.StopRecord(cameraUid, eventUid), "StopRecord");
		}

		public void Ptz(Guid cameraUid, int ptzNumber)
		{
			SafeOperationCall(() => FiresecService.Ptz(cameraUid, ptzNumber), "Ptz");
		}

		public void RviAlarm(string name)
		{
			SafeOperationCall(() => FiresecService.RviAlarm(name), "RviAlarm");
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType)
		{
			SafeOperationCall(() => FiresecService.ControlFireZone(uid, commandType), "ControlFireZone");
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType)
		{
			SafeOperationCall(() => FiresecService.ControlGuardZone(uid, commandType), "ControlGuardZone");
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType)
		{
			SafeOperationCall(() => FiresecService.ControlDirection(uid, commandType), "ControlDirection");
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType)
		{
			SafeOperationCall(() => FiresecService.ControlGKDoor(uid, commandType), "ControlGKDoor");
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType)
		{
			SafeOperationCall(() => FiresecService.ControlDelay(uid, commandType), "ControlDelay");
		}

		public void ControlPumpStation(Guid uid, PumpStationCommandType commandType)
		{
			SafeOperationCall(() => FiresecService.ControlPumpStation(uid, commandType), "ControlPumpStation");
		}

		public void ControlMPT(Guid uid, MPTCommandType commandType)
		{
			SafeOperationCall(() => FiresecService.ControlMPT(uid, commandType), "ControlMPT");
		}

		public void ExportJournalA(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			SafeOperationCall(() => FiresecService.ExportJournalA(isExportJournal, isExportPassJournal, minDate, maxDate, path), "ExportJournal");
		}

		public void ExportOrganisationA(bool isWithDeleted, Guid organisationUid, string path)
		{
			SafeOperationCall(() => FiresecService.ExportOrganisationA(isWithDeleted, organisationUid, path), "ExportOrganisation");
		}

		public void ExportOrganisationListA(bool isWithDeleted, string path)
		{
			SafeOperationCall(() => FiresecService.ExportOrganisationListA(isWithDeleted, path), "ExportOrganisationList");
		}

		public void ExportConfigurationA(bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			SafeOperationCall(() => FiresecService.ExportConfigurationA(isExportDevices, isExportDoors, isExportZones, path), "ExportConfiguration");
		}

		public void ImportOrganisationA(bool isWithDeleted, string path)
		{
			SafeOperationCall(() => FiresecService.ImportOrganisationA(isWithDeleted, path), "ImportOrganisation");
		}

		public void ImportOrganisationListA(bool isWithDeleted, string path)
		{
			SafeOperationCall(() => FiresecService.ImportOrganisationListA(isWithDeleted, path), "ImportOrganisationList");
		}
		#endregion
	}
}