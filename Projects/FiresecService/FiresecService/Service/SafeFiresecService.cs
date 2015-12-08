using Common;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.License;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

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

		bool CheckClient(Guid clientUID)
		{
			return ClientsManager.ClientInfos.Any(x => x.UID == clientUID);
		}

		OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> func, string operationName, Guid clientUID)
		{
			if (!CheckClient(clientUID)) throw new InvalidOperationException("Попытка вызова метода неавторизванным клиентом. OperationName = " + operationName);
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

		T SafeOperationCall<T>(Func<T> func, string operationName, Guid clientUID)
		{
			if (!CheckClient(clientUID)) throw new InvalidOperationException("Попытка вызова метода неавторизванным клиентом. OperationName = " + operationName);
			try
			{
				BeginOperation(operationName);
				var result = func();
				EndOperation();
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. OperationName = " + operationName);
			}
			return default(T);
		}

		void SafeOperationCall(Action action, string operationName, Guid clientUID)
		{
			if (!CheckClient(clientUID)) throw new InvalidOperationException("Попытка вызова метода неавторизванным клиентом. OperationName = " + operationName);
			try
			{
				BeginOperation(operationName);
				action();
				EndOperation();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. OperationName = " + operationName);
			}
		}

		public OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials)
		{
			try
			{
				return FiresecService.Connect(uid, clientCredentials);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.Connect");
				return OperationResult<bool>.FromError("Ошибка при выполнении операции на сервере" + "\n\r" + e.Message + "\n" + e.StackTrace);
			}
		}

		public void Disconnect(Guid uid)
		{
			try
			{
				FiresecService.Disconnect(uid);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.Disconnect");
			}
		}

		public OperationResult<ServerState> GetServerState(Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetServerState(clientUID); }, "GetServerState", clientUID);
		}

		public string Ping(Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.Ping(clientUID); }, "Ping", clientUID);
		}

		public OperationResult ResetDB(Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.ResetDB(clientUID); }, "ResetDB", clientUID);
		}

		public List<CallbackResult> Poll(Guid uid, int callbackIndex)
		{
			return SafeContext.Execute<List<CallbackResult>>(() => FiresecService.Poll(uid, callbackIndex));
		}

		public SecurityConfiguration GetSecurityConfiguration(Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(clientUID); }, "GetSecurityConfiguration", clientUID);
		}

		public List<string> GetFileNamesList(string directory, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetFileNamesList(directory, clientUID); }, "GetFileNamesList", clientUID);
		}

		public Dictionary<string, string> GetDirectoryHash(string directory, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetDirectoryHash(directory, clientUID); }, "GetDirectoryHash", clientUID);
		}

		public System.IO.Stream GetServerAppDataFile(string dirAndFileName, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetServerAppDataFile(dirAndFileName, clientUID); }, "GetServerAppDataFile", clientUID);
		}

		public Stream GetConfig(Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetConfig(clientUID); }, "GetConfig", clientUID);
		}

		public void SetRemoteConfig(Stream stream)
		{
			try
			{
				FiresecService.SetRemoteConfig(stream);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SetRemoteConfig");
			}
		}

		public void SetLocalConfig(Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.SetLocalConfig(clientUID); }, "SetLocalConfig", clientUID);
		}

		public string Test(string arg, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.Test(arg, clientUID); }, "Test", clientUID);
		}

		public OperationResult<FiresecLicenseInfo> GetLicenseInfo(Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetLicenseInfo(clientUID); }, "GetLicenseInfo", clientUID);
		}

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime(Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetMinJournalDateTime(clientUID), "GetMinJournalDateTime", clientUID);
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetFilteredJournalItems(filter, clientUID), "GetFilteredJournalItems", clientUID);
		}
		public OperationResult<bool> BeginGetJournal(JournalFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.BeginGetJournal(filter, clientUID), "BeginGetJournal", clientUID);
		}
		public OperationResult<bool> AddJournalItem(JournalItem journalItem, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.AddJournalItem(journalItem, clientUID); }, "AddJournalItem", clientUID);
		}

		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.BeginGetArchivePage(filter, page, clientUID), "BeginGetArchivePage", clientUID);
		}

		public OperationResult<int> GetArchiveCount(JournalFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetArchiveCount(filter, clientUID), "GetArchiveCount", clientUID);
		}
		#endregion

		#region GK

		public void CancelGKProgress(Guid progressCallbackUID, string userName, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.CancelGKProgress(progressCallbackUID, userName, clientUID); }, "CancelGKProgress", clientUID);
		}

		public OperationResult<bool> GKWriteConfiguration(Guid deviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKWriteConfiguration(deviceUID, clientUID); }, "GKWriteConfiguration", clientUID);
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid deviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadConfiguration(deviceUID, clientUID); }, "GKReadConfiguration", clientUID);
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(Guid deviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadConfigurationFromGKFile(deviceUID, clientUID); }, "GKReadConfigurationFromGKFile", clientUID);
		}

		public Stream GetServerFile(string filePath, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetServerFile(filePath, clientUID); }, "GetServerFile", clientUID);
		}

		public OperationResult<GKDevice> GKAutoSearch(Guid deviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKAutoSearch(deviceUID, clientUID); }, "GKAutoSearch", clientUID);
		}

		public OperationResult<bool> GKUpdateFirmware(Guid deviceUID, List<byte> firmwareBytes, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKUpdateFirmware(deviceUID, firmwareBytes, clientUID); }, "GKUpdateFirmware", clientUID);
		}

		public OperationResult<bool> GKSyncronyseTime(Guid deviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKSyncronyseTime(deviceUID, clientUID); }, "GKSyncronyseTime", clientUID);
		}

		public OperationResult<string> GKGetDeviceInfo(Guid deviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetDeviceInfo(deviceUID, clientUID); }, "GKGetDeviceInfo", clientUID);
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid deviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetJournalItemsCount(deviceUID, clientUID); }, "GKGetJournalItemsCount", clientUID);
		}

		public OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadJournalItem(deviceUID, no, clientUID); }, "GKReadJournalItem", clientUID);
		}

		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes, Guid clientUID, List<GKProperty> deviceProperties)
		{
			return SafeOperationCall(() => { return FiresecService.GKSetSingleParameter(objectUID, parameterBytes, clientUID, deviceProperties); }, "GKSetSingleParameter", clientUID);
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(Guid objectUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetSingleParameter(objectUID, clientUID); }, "GKGetSingleParameter", clientUID);
		}

		public OperationResult<bool> GKRewriteAllSchedules(Guid gkDeviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKRewriteAllSchedules(gkDeviceUID, clientUID); }, "GKRewriteAllSchedules", clientUID);
		}

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKSetSchedule(schedule, clientUID); }, "GKSetSchedule", clientUID);
		}

		public OperationResult<bool> GKGetUsers(Guid gkDeviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetUsers(gkDeviceUID, clientUID); }, "GKGetUsers", clientUID);
		}

		public OperationResult<bool> GKRewriteUsers(Guid gkDeviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKRewriteUsers(gkDeviceUID, clientUID); }, "GKRewriteUsers", clientUID);
		}

		public OperationResult<List<MirrorUser>> GKReadMirrorUsers(Guid deviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadMirrorUsers(deviceUID, clientUID); }, "GKReadMirrorUsers", clientUID);
		}

		public OperationResult<bool> GKWriteMirrorUsers(Guid deviceUID, List<MirrorUser> mirrorUsers, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKWriteMirrorUsers(deviceUID, mirrorUsers, clientUID); }, "GKWriteMirrorUsers", clientUID);
		}

		public OperationResult<List<GKUser>> GetGKUsers(Guid deviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetGKUsers(deviceUID, clientUID); }, "GetGKUsers", clientUID);
		}

		public OperationResult<bool> RewritePmfUsers(Guid uid, List<GKUser> users, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.RewritePmfUsers(uid, users, clientUID); }, "RewritePmfUsers", clientUID);
		}

		public OperationResult<List<byte>> GKGKHash(Guid gkDeviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGKHash(gkDeviceUID, clientUID); }, "GKGKHash", clientUID);
		}

		public GKStates GKGetStates(Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetStates(clientUID); }, "GKGetStates", clientUID);
		}
		public void GKExecuteDeviceCommand(Guid deviceUID, GKStateBit stateBit, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKExecuteDeviceCommand(deviceUID, stateBit, clientUID); }, "GKExecuteDeviceCommand", clientUID);
		}

		public void GKReset(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKReset(uid, objectType, clientUID); }, "GKReset", clientUID);
		}

		public void GKResetFire1(Guid zoneUID, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKResetFire1(zoneUID, clientUID); }, "GKResetFire1", clientUID);
		}

		public void GKResetFire2(Guid zoneUID, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKResetFire2(zoneUID, clientUID); }, "GKResetFire2", clientUID);
		}

		public void GKSetAutomaticRegime(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKSetAutomaticRegime(uid, objectType, clientUID); }, "GKSetAutomaticRegime", clientUID);
		}

		public void GKSetManualRegime(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKSetManualRegime(uid, objectType, clientUID); }, "GKSetManualRegime", clientUID);
		}

		public void GKSetIgnoreRegime(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKSetIgnoreRegime(uid, objectType, clientUID); }, "GKSetIgnoreRegime", clientUID);
		}

		public void GKTurnOn(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOn(uid, objectType, clientUID); }, "GKTurnOn", clientUID);
		}

		public void GKTurnOnNow(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnNow(uid, objectType, clientUID); }, "GKTurnOnNow", clientUID);
		}

		public void GKTurnOnInAutomatic(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnInAutomatic(uid, objectType, clientUID); }, "GKTurnOnInAutomatic", clientUID);
		}

		public void GKTurnOnNowInAutomatic(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnNowInAutomatic(uid, objectType, clientUID); }, "GKTurnOnNowInAutomatic", clientUID);
		}

		public void GKTurnOff(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOff(uid, objectType, clientUID); }, "GKTurnOff", clientUID);
		}

		public void GKTurnOffNow(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffNow(uid, objectType, clientUID); }, "GKTurnOffNow", clientUID);
		}

		public void GKTurnOffInAutomatic(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffInAutomatic(uid, objectType, clientUID); }, "GKTurnOffInAutomatic", clientUID);
		}

		public void GKTurnOffNowInAutomatic(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffNowInAutomatic(uid, objectType, clientUID); }, "GKTurnOffNowInAutomatic", clientUID);
		}

		public void GKStop(Guid uid, GKBaseObjectType objectType, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKStop(uid, objectType, clientUID); }, "GKStop", clientUID);
		}

		public void GKStartMeasureMonitoring(Guid deviceUID, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKStartMeasureMonitoring(deviceUID, clientUID); }, "GKStartMeasureMonitoring", clientUID);
		}

		public void GKStopMeasureMonitoring(Guid deviceUID, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKStopMeasureMonitoring(deviceUID, clientUID); }, "GKStopMeasureMonitoring", clientUID);
		}

		public OperationResult<uint> GKGetReaderCode(Guid deviceUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetReaderCode(deviceUID, clientUID); }, "GKGetReaderCode", clientUID);
		}

		public void GKOpenSKDZone(Guid zoneUID, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKOpenSKDZone(zoneUID, clientUID); }, "GKOpenSKDZone", clientUID);
		}

		public void GKCloseSKDZone(Guid zoneUID, Guid clientUID)
		{
			SafeOperationCall(() => { FiresecService.GKCloseSKDZone(zoneUID, clientUID); }, "GKCloseSKDZone", clientUID);
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetAlsMeasure(alsUid, clientUID), "GetAlsMeasure", clientUID);
		}

		#endregion

		#region Automation

		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			return SafeOperationCall(() => { return FiresecService.RunProcedure(clientUID, procedureUID, args); }, "RunProcedure", clientUID);
		}

		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ProcedureCallbackResponse(procedureThreadUID, value, clientUID), "ProcedureCallbackResponse", clientUID);
		}

		public ProcedureProperties GetProperties(Guid layoutUID, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetProperties(layoutUID, clientUID); }, "GetProperties", clientUID);
		}

		public void SetVariableValue(Guid variableUid, object value, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.SetVariableValue(variableUid, value, clientUID), "SetVariableValue", clientUID);
		}

		public Variable GetVariable(Guid variableUid, Guid clientUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetVariable(variableUid, clientUID); }, "GetVariable", clientUID);
		}

		public void AddJournalItemA(string message, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.AddJournalItemA(message, clientUID), "AddJournalItem", clientUID);
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ControlGKDevice(deviceUid, command, clientUID), "ControlGKDevice", clientUID);
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.StartRecord(cameraUid, journalItemUid, eventUid, timeout, clientUID), "StartRecord", clientUID);
		}

		public void StopRecord(Guid cameraUid, Guid eventUid, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.StopRecord(cameraUid, eventUid, clientUID), "StopRecord", clientUID);
		}

		public void Ptz(Guid cameraUid, int ptzNumber, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.Ptz(cameraUid, ptzNumber, clientUID), "Ptz", clientUID);
		}

		public void RviAlarm(string name, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.RviAlarm(name, clientUID), "RviAlarm", clientUID);
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ControlFireZone(uid, commandType, clientUID), "ControlFireZone", clientUID);
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ControlGuardZone(uid, commandType, clientUID), "ControlGuardZone", clientUID);
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ControlDirection(uid, commandType, clientUID), "ControlDirection", clientUID);
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ControlGKDoor(uid, commandType, clientUID), "ControlGKDoor", clientUID);
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ControlDelay(uid, commandType, clientUID), "ControlDelay", clientUID);
		}

		public void ControlPumpStation(Guid uid, PumpStationCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ControlPumpStation(uid, commandType, clientUID), "ControlPumpStation", clientUID);
		}

		public void ControlMPT(Guid uid, MPTCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ControlMPT(uid, commandType, clientUID), "ControlMPT", clientUID);
		}

		public void ExportJournalA(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ExportJournalA(isExportJournal, isExportPassJournal, minDate, maxDate, path, clientUID), "ExportJournal", clientUID);
		}

		public void ExportOrganisationA(bool isWithDeleted, Guid organisationUid, string path, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ExportOrganisationA(isWithDeleted, organisationUid, path, clientUID), "ExportOrganisation", clientUID);
		}

		public void ExportOrganisationListA(bool isWithDeleted, string path, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ExportOrganisationListA(isWithDeleted, path, clientUID), "ExportOrganisationList", clientUID);
		}

		public void ExportConfigurationA(bool isExportDevices, bool isExportDoors, bool isExportZones, string path, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ExportConfigurationA(isExportDevices, isExportDoors, isExportZones, path, clientUID), "ExportConfiguration", clientUID);
		}

		public void ImportOrganisationA(bool isWithDeleted, string path, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ImportOrganisationA(isWithDeleted, path, clientUID), "ImportOrganisation", clientUID);
		}

		public void ImportOrganisationListA(bool isWithDeleted, string path, Guid clientUID)
		{
			SafeOperationCall(() => FiresecService.ImportOrganisationListA(isWithDeleted, path, clientUID), "ImportOrganisationList", clientUID);
		}
		#endregion
	}
}