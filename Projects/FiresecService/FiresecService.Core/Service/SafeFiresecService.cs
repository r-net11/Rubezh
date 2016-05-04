using Common;
using OpcClientSdk;
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
			return clientUID == FiresecService.UID || ClientsManager.ClientInfos.Any(x => x.UID == clientUID);
		}

		OperationResult<T> SafeOperationCall<T>(Guid clientUID, Func<OperationResult<T>> func, string operationName)
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

		T SafeOperationCall<T>(Guid clientUID, Func<T> func, string operationName)
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

		void SafeOperationCall(Guid clientUID, Action action, string operationName)
		{
			//if (!CheckClient(clientUID)) throw new InvalidOperationException("Попытка вызова метода неавторизванным клиентом. OperationName = " + operationName);
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

		public OperationResult<bool> Connect(ClientCredentials clientCredentials)
		{
			try
			{
				return FiresecService.Connect(clientCredentials);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.Connect");
				return OperationResult<bool>.FromError("Ошибка при выполнении операции на сервере" + "\n\r" + e.Message + "\n" + e.StackTrace);
			}
		}

		public void Disconnect(Guid clientUID)
		{
			try
			{
				FiresecService.Disconnect(clientUID);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.Disconnect");
			}
		}

		public void LayoutChanged(Guid clientUID, Guid layoutUID)
		{
			SafeOperationCall(clientUID, () => FiresecService.LayoutChanged(clientUID, layoutUID), "LayoutChanged");
		}
		public OperationResult<ServerState> GetServerState(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetServerState(clientUID); }, "GetServerState");
		}

		public string Ping(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.Ping(clientUID); }, "Ping");
		}

		public OperationResult<bool> ResetDB(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.ResetDB(clientUID); }, "ResetDB");
		}

		public PollResult Poll(Guid clientUID, int callbackIndex)
		{
			return SafeContext.Execute(() => FiresecService.Poll(clientUID, callbackIndex));
		}

		public OperationResult<SecurityConfiguration> GetSecurityConfiguration(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetSecurityConfiguration(clientUID); }, "GetSecurityConfiguration");
		}

		public void SetSecurityConfiguration(Guid clientUID, SecurityConfiguration securityConfiguration)
		{
			SafeOperationCall(clientUID, () => FiresecService.SetSecurityConfiguration(clientUID, securityConfiguration), "SetSecurityConfiguration");
		}

		public List<string> GetFileNamesList(Guid clientUID, string directory)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetFileNamesList(clientUID, directory); }, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(Guid clientUID, string directory)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetDirectoryHash(clientUID, directory); }, "GetDirectoryHash");
		}

		public System.IO.Stream GetServerAppDataFile(Guid clientUID, string dirAndFileName)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetServerAppDataFile(clientUID, dirAndFileName); }, "GetServerAppDataFile");
		}

		public Stream GetConfig(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetConfig(clientUID); }, "GetConfig");
		}

		public OperationResult<bool> SetRemoteConfig(Stream stream)
		{
			try
			{
				FiresecService.SetRemoteConfig(stream);
				return new OperationResult<bool>(true);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SetRemoteConfig");
				return OperationResult<bool>.FromError(e.Message);
			}
		}

		public OperationResult<bool> SetLocalConfig(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.SetLocalConfig(clientUID); }, "SetLocalConfig");
		}

		public string Test(Guid clientUID, string arg)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.Test(clientUID, arg); }, "Test");
		}

		public OperationResult<FiresecLicenseInfo> GetLicenseInfo(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetLicenseInfo(clientUID); }, "GetLicenseInfo");
		}

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetMinJournalDateTime(clientUID), "GetMinJournalDateTime");
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(Guid clientUID, JournalFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetFilteredJournalItems(clientUID, filter), "GetFilteredJournalItems");
		}
		public OperationResult<bool> BeginGetJournal(JournalFilter filter, Guid clientUid, Guid journalClientUid)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.BeginGetJournal(filter, clientUid, journalClientUid));
		}
		public OperationResult<bool> AddJournalItem(Guid clientUID, JournalItem journalItem)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.AddJournalItem(clientUID, journalItem); }, "AddJournalItem");
		}

		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page, Guid clientUid, string userName)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.BeginGetArchivePage(filter, page, clientUid, userName));
		}

		public OperationResult<int> GetArchiveCount(Guid clientUID, JournalFilter filter)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetArchiveCount(clientUID, filter), "GetArchiveCount");
		}
		#endregion

		#region GK

		public void CancelGKProgress(Guid clientUID, Guid progressCallbackUID, string userName)
		{
			SafeOperationCall(clientUID, () => { FiresecService.CancelGKProgress(clientUID, progressCallbackUID, userName); }, "CancelGKProgress");
		}

		public OperationResult<bool> GKWriteConfiguration(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKWriteConfiguration(clientUID, deviceUID); }, "GKWriteConfiguration");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKReadConfiguration(clientUID, deviceUID); }, "GKReadConfiguration");
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKReadConfigurationFromGKFile(clientUID, deviceUID); }, "GKReadConfigurationFromGKFile");
		}

		public Stream GetServerFile(Guid clientUID, string filePath)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetServerFile(clientUID, filePath); }, "GetServerFile");
		}

		public OperationResult<GKDevice> GKAutoSearch(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKAutoSearch(clientUID, deviceUID); }, "GKAutoSearch");
		}

		public OperationResult<bool> GKUpdateFirmware(Guid clientUID, Guid deviceUID, List<byte> firmwareBytes)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKUpdateFirmware(clientUID, deviceUID, firmwareBytes); }, "GKUpdateFirmware");
		}

		public OperationResult<bool> GKSyncronyseTime(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKSyncronyseTime(clientUID, deviceUID); }, "GKSyncronyseTime");
		}

		public OperationResult<string> GKGetDeviceInfo(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKGetDeviceInfo(clientUID, deviceUID); }, "GKGetDeviceInfo");
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKGetJournalItemsCount(clientUID, deviceUID); }, "GKGetJournalItemsCount");
		}

		public OperationResult<JournalItem> GKReadJournalItem(Guid clientUID, Guid deviceUID, int no)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKReadJournalItem(clientUID, deviceUID, no); }, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(Guid clientUID, Guid objectUID, List<byte> parameterBytes, List<GKProperty> deviceProperties)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKSetSingleParameter(clientUID, objectUID, parameterBytes, deviceProperties); }, "GKSetSingleParameter");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(Guid clientUID, Guid objectUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKGetSingleParameter(clientUID, objectUID); }, "GKGetSingleParameter");
		}

		public OperationResult<bool> GKRewriteAllSchedules(Guid clientUID, Guid gkDeviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKRewriteAllSchedules(clientUID, gkDeviceUID); }, "GKRewriteAllSchedules");
		}

		public OperationResult<bool> GKSetSchedule(Guid clientUID, GKSchedule schedule)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKSetSchedule(clientUID, schedule); }, "GKSetSchedule");
		}

		public OperationResult<bool> GKGetUsers(Guid clientUID, Guid gkDeviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKGetUsers(clientUID, gkDeviceUID); }, "GKGetUsers");
		}

		public OperationResult<bool> GKRewriteUsers(Guid clientUID, Guid gkDeviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKRewriteUsers(clientUID, gkDeviceUID); }, "GKRewriteUsers");
		}

		public OperationResult<List<GKUser>> GetGKUsers(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetGKUsers(clientUID, deviceUID); }, "GetGKUsers");
		}

		public OperationResult<bool> RewritePmfUsers(Guid clientUID, Guid uid, List<GKUser> users)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.RewritePmfUsers(clientUID, uid, users); }, "RewritePmfUsers");
		}

		public OperationResult<List<byte>> GKGKHash(Guid clientUID, Guid gkDeviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKGKHash(clientUID, gkDeviceUID); }, "GKGKHash");
		}

		public GKStates GKGetStates(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKGetStates(clientUID); }, "GKGetStates");
		}
		public void GKExecuteDeviceCommand(Guid clientUID, Guid deviceUID, GKStateBit stateBit, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKExecuteDeviceCommand(clientUID, userName, deviceUID, stateBit); }, "GKExecuteDeviceCommand");
		}

		public void GKReset(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKReset(clientUID, userName, uid, objectType); }, "GKReset");
		}

		public void GKResetFire1(Guid clientUID, Guid zoneUID)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKResetFire1(clientUID, zoneUID); }, "GKResetFire1");
		}

		public void GKResetFire2(Guid clientUID, Guid zoneUID)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKResetFire2(clientUID, zoneUID); }, "GKResetFire2");
		}

		public void GKSetAutomaticRegime(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKSetAutomaticRegime(clientUID, userName, uid, objectType); }, "GKSetAutomaticRegime");
		}

		public void GKSetManualRegime(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKSetManualRegime(clientUID, userName, uid, objectType); }, "GKSetManualRegime");
		}

		public void GKSetIgnoreRegime(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKSetIgnoreRegime(clientUID, userName, uid, objectType); }, "GKSetIgnoreRegime");
		}

		public void GKTurnOn(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKTurnOn(clientUID, userName, uid, objectType); }, "GKTurnOn");
		}

		public void GKTurnOnNow(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKTurnOnNow(clientUID, userName, uid, objectType); }, "GKTurnOnNow");
		}

		public void GKTurnOnInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKTurnOnInAutomatic(clientUID, uid, objectType); }, "GKTurnOnInAutomatic");
		}

		public void GKTurnOnNowInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKTurnOnNowInAutomatic(clientUID, uid, objectType); }, "GKTurnOnNowInAutomatic");
		}

		public void GKTurnOff(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKTurnOff(clientUID, userName, uid, objectType); }, "GKTurnOff");
		}

		public void GKTurnOffNow(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKTurnOffNow(clientUID, userName, uid, objectType); }, "GKTurnOffNow");
		}

		public void GKTurnOffInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKTurnOffInAutomatic(clientUID, userName, uid, objectType); }, "GKTurnOffInAutomatic");
		}

		public void GKTurnOffNowInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKTurnOffNowInAutomatic(clientUID, uid, objectType); }, "GKTurnOffNowInAutomatic");
		}

		public void GKTurnOnNowGlobalPimsInAutomatic(Guid clientUID)
		{
			SafeOperationCall(clientUID, () => FiresecService.GKTurnOnNowGlobalPimsInAutomatic(clientUID), "GKTurnOnNowGlobalPimsInAutomatic");
		}

		public void GKTurnOffNowGlobalPimsInAutomatic(Guid clientUID)
		{
			SafeOperationCall(clientUID, () => FiresecService.GKTurnOffNowGlobalPimsInAutomatic(clientUID), "GKTurnOffNowGlobalPimsInAutomatic");
		}

		public void GKStop(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKStop(clientUID, userName, uid, objectType); }, "GKStop");
		}

		public void GKStartMeasureMonitoring(Guid clientUID, Guid deviceUID)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKStartMeasureMonitoring(clientUID, deviceUID); }, "GKStartMeasureMonitoring");
		}

		public void GKStopMeasureMonitoring(Guid clientUID, Guid deviceUID)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKStopMeasureMonitoring(clientUID, deviceUID); }, "GKStopMeasureMonitoring");
		}

		public OperationResult<uint> GKGetReaderCode(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GKGetReaderCode(clientUID, deviceUID); }, "GKGetReaderCode");
		}

		public void GKOpenSKDZone(Guid clientUID, Guid zoneUID)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKOpenSKDZone(clientUID, zoneUID); }, "GKOpenSKDZone");
		}

		public void GKCloseSKDZone(Guid clientUID, Guid zoneUID)
		{
			SafeOperationCall(clientUID, () => { FiresecService.GKCloseSKDZone(clientUID, zoneUID); }, "GKCloseSKDZone");
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid clientUID, Guid alsUid)
		{
			return SafeOperationCall(clientUID, () => FiresecService.GetAlsMeasure(clientUID, alsUid), "GetAlsMeasure");
		}

		#endregion

		#region Automation

		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.RunProcedure(clientUID, procedureUID, args); }, "RunProcedure");
		}

		public void ProcedureCallbackResponse(Guid clientUID, Guid procedureThreadUID, object value)
		{
			SafeOperationCall(clientUID, () => FiresecService.ProcedureCallbackResponse(clientUID, procedureThreadUID, value), "ProcedureCallbackResponse");
		}

		public ProcedureProperties GetProperties(Guid clientUID, Guid layoutUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetProperties(clientUID, layoutUID); }, "GetProperties");
		}

		public void SetVariableValue(Guid clientUID, Guid variableUid, object value)
		{
			SafeOperationCall(clientUID, () => FiresecService.SetVariableValue(clientUID, variableUid, value), "SetVariableValue");
		}

		public void AddJournalItemA(Guid clientUID, string message, Guid? objectUID = null)
		{
			SafeOperationCall(clientUID, () => FiresecService.AddJournalItemA(clientUID, message, objectUID), "AddJournalItem");
		}

		public void ControlGKDevice(Guid clientUID, Guid deviceUid, GKStateBit command)
		{
			SafeOperationCall(clientUID, () => FiresecService.ControlGKDevice(clientUID, deviceUid, command), "ControlGKDevice");
		}

		public void StartRecord(Guid clientUID, Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			SafeOperationCall(clientUID, () => FiresecService.StartRecord(clientUID, cameraUid, journalItemUid, eventUid, timeout), "StartRecord");
		}

		public void StopRecord(Guid clientUID, Guid cameraUid, Guid eventUid)
		{
			SafeOperationCall(clientUID, () => FiresecService.StopRecord(clientUID, cameraUid, eventUid), "StopRecord");
		}

		public void Ptz(Guid clientUID, Guid cameraUid, int ptzNumber)
		{
			SafeOperationCall(clientUID, () => FiresecService.Ptz(clientUID, cameraUid, ptzNumber), "Ptz");
		}

		public void RviAlarm(Guid clientUID, string name)
		{
			SafeOperationCall(clientUID, () => FiresecService.RviAlarm(clientUID, name), "RviAlarm");
		}

		public void RviOpenWindow(Guid clientUid, string name, int x, int y, int monitorNumber, string login, string ip)
		{
			SafeOperationCall(clientUid, () => FiresecService.RviOpenWindow(clientUid, name, x, y, monitorNumber, login, ip), "RviOpenWindow");
		}

		public void ControlFireZone(Guid clientUID, Guid uid, ZoneCommandType commandType)
		{
			SafeOperationCall(clientUID, () => FiresecService.ControlFireZone(clientUID, uid, commandType), "ControlFireZone");
		}

		public void ControlGuardZone(Guid clientUID, Guid uid, GuardZoneCommandType commandType)
		{
			SafeOperationCall(clientUID, () => FiresecService.ControlGuardZone(clientUID, uid, commandType), "ControlGuardZone");
		}

		public void ControlDirection(Guid clientUID, Guid uid, DirectionCommandType commandType)
		{
			SafeOperationCall(clientUID, () => FiresecService.ControlDirection(clientUID, uid, commandType), "ControlDirection");
		}

		public void ControlGKDoor(Guid clientUID, Guid uid, GKDoorCommandType commandType)
		{
			SafeOperationCall(clientUID, () => FiresecService.ControlGKDoor(clientUID, uid, commandType), "ControlGKDoor");
		}

		public void ControlDelay(Guid clientUID, Guid uid, DelayCommandType commandType)
		{
			SafeOperationCall(clientUID, () => FiresecService.ControlDelay(clientUID, uid, commandType), "ControlDelay");
		}

		public void ControlPumpStation(Guid clientUID, Guid uid, PumpStationCommandType commandType)
		{
			SafeOperationCall(clientUID, () => FiresecService.ControlPumpStation(clientUID, uid, commandType), "ControlPumpStation");
		}

		public void ControlMPT(Guid clientUID, Guid uid, MPTCommandType commandType)
		{
			SafeOperationCall(clientUID, () => FiresecService.ControlMPT(clientUID, uid, commandType), "ControlMPT");
		}

		public void ExportJournalA(Guid clientUID, bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			SafeOperationCall(clientUID, () => FiresecService.ExportJournalA(clientUID, isExportJournal, isExportPassJournal, minDate, maxDate, path), "ExportJournal");
		}

		public void ExportOrganisationA(Guid clientUID, bool isWithDeleted, Guid organisationUid, string path)
		{
			SafeOperationCall(clientUID, () => FiresecService.ExportOrganisationA(clientUID, isWithDeleted, organisationUid, path), "ExportOrganisation");
		}

		public void ExportOrganisationListA(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(clientUID, () => FiresecService.ExportOrganisationListA(clientUID, isWithDeleted, path), "ExportOrganisationList");
		}

		public void ExportConfigurationA(Guid clientUID, bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			SafeOperationCall(clientUID, () => FiresecService.ExportConfigurationA(clientUID, isExportDevices, isExportDoors, isExportZones, path), "ExportConfiguration");
		}

		public void ImportOrganisationA(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(clientUID, () => FiresecService.ImportOrganisationA(clientUID, isWithDeleted, path), "ImportOrganisation");
		}

		public void ImportOrganisationListA(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(clientUID, () => FiresecService.ImportOrganisationListA(clientUID, isWithDeleted, path), "ImportOrganisationList");
		}
		#endregion

		#region OPC DA Server

		public OperationResult<OpcDaServer[]> GetOpcDaServers(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetOpcDaServers(); }, "GetOpcDaServerNames");
		}

		public OperationResult<OpcDaElement[]> GetOpcDaServerGroupAndTags(Guid clientUID, OpcDaServer server)
		{
			return SafeOperationCall(clientUID, () =>
			{ return FiresecService.GetOpcDaServerGroupAndTags(server); }, "GetOpcDaServerGroupAndTags");
		}

		public OperationResult<OpcServerStatus> GetOpcDaServerStatus(Guid clientUID, OpcDaServer server)
		{
			return SafeOperationCall(clientUID, () =>
			{ return FiresecService.GetOpcDaServerStatus(server); }, "GetOpcDaServerStatus");
		}

		public OperationResult<OpcDaTagValue[]> ReadOpcDaServerTags(Guid clientUID, OpcDaServer server)
		{
			var result = SafeOperationCall(clientUID, () =>
			{ return FiresecService.ReadOpcDaServerTags(server); }, "ReadOpcDaServerTags");
			return result;
		}

		public OperationResult<bool> WriteOpcDaTag(Guid clientUID, Guid tagId, Object value)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.WriteOpcDaTag(tagId, value); },
				"WriteOpcDaTag");
		}
		public List<RviState> GetRviStates(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return FiresecService.GetRviStates(); }, "GetRviStates");
		}

		#endregion
	}
}