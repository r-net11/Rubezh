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

namespace RubezhService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class SafeRubezhService : IRubezhService
	{
		public RubezhService RubezhService { get; set; }

		public SafeRubezhService()
		{
			RubezhService = new RubezhService();
		}

		public void BeginOperation(string operationName)
		{
		}

		public void EndOperation()
		{
		}

		bool CheckClient(Guid clientUID)
		{
			return clientUID == RubezhService.UID || ClientsManager.ClientInfos.Any(x => x.UID == clientUID);
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
				Logger.Error(e, "Исключение при вызове SafeRubezhService.SafeOperationCall. operationName = " + operationName);
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
				Logger.Error(e, "Исключение при вызове SafeRubezhService.SafeOperationCall. OperationName = " + operationName);
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
				Logger.Error(e, "Исключение при вызове SafeRubezhService.SafeOperationCall. OperationName = " + operationName);
			}
		}

		public OperationResult<bool> Connect(ClientCredentials clientCredentials)
		{
			try
			{
				return RubezhService.Connect(clientCredentials);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeRubezhService.Connect");
				return OperationResult<bool>.FromError("Ошибка при выполнении операции на сервере" + "\n\r" + e.Message + "\n" + e.StackTrace);
			}
		}

		public void Disconnect(Guid clientUID)
		{
			try
			{
				RubezhService.Disconnect(clientUID);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeRubezhService.Disconnect");
			}
		}

		public void LayoutChanged(Guid clientUID, Guid layoutUID)
		{
			SafeOperationCall(clientUID, () => RubezhService.LayoutChanged(clientUID, layoutUID), "LayoutChanged");
		}
		public OperationResult<ServerState> GetServerState(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetServerState(clientUID); }, "GetServerState");
		}

		public string Ping(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.Ping(clientUID); }, "Ping");
		}

		public OperationResult<bool> ResetDB(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.ResetDB(clientUID); }, "ResetDB");
		}

		public PollResult Poll(Guid clientUID, int callbackIndex)
		{
			return SafeContext.Execute(() => RubezhService.Poll(clientUID, callbackIndex));
		}

		public OperationResult<SecurityConfiguration> GetSecurityConfiguration(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetSecurityConfiguration(clientUID); }, "GetSecurityConfiguration");
		}

		public void SetSecurityConfiguration(Guid clientUID, SecurityConfiguration securityConfiguration)
		{
			SafeOperationCall(clientUID, () => RubezhService.SetSecurityConfiguration(clientUID, securityConfiguration), "SetSecurityConfiguration");
		}

		public List<string> GetFileNamesList(Guid clientUID, string directory)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetFileNamesList(clientUID, directory); }, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(Guid clientUID, string directory)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetDirectoryHash(clientUID, directory); }, "GetDirectoryHash");
		}

		public System.IO.Stream GetServerAppDataFile(Guid clientUID, string dirAndFileName)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetServerAppDataFile(clientUID, dirAndFileName); }, "GetServerAppDataFile");
		}

		public Stream GetConfig(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetConfig(clientUID); }, "GetConfig");
		}

		public OperationResult<bool> SetRemoteConfig(Stream stream)
		{
			try
			{
				RubezhService.SetRemoteConfig(stream);
				return new OperationResult<bool>(true);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeRubezhService.SetRemoteConfig");
				return OperationResult<bool>.FromError(e.Message);
			}
		}

		public OperationResult<bool> SetLocalConfig(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.SetLocalConfig(clientUID); }, "SetLocalConfig");
		}

		public string Test(Guid clientUID, string arg)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.Test(clientUID, arg); }, "Test");
		}

		public OperationResult<RubezhLicenseInfo> GetLicenseInfo(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetLicenseInfo(clientUID); }, "GetLicenseInfo");
		}

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetMinJournalDateTime(clientUID), "GetMinJournalDateTime");
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(Guid clientUID, JournalFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetFilteredJournalItems(clientUID, filter), "GetFilteredJournalItems");
		}
		public OperationResult<bool> BeginGetJournal(JournalFilter filter, Guid clientUid, Guid journalClientUid)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => RubezhService.BeginGetJournal(filter, clientUid, journalClientUid));
		}
		public OperationResult<bool> AddJournalItem(Guid clientUID, JournalItem journalItem)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.AddJournalItem(clientUID, journalItem); }, "AddJournalItem");
		}

		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page, Guid clientUid, string userName)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => RubezhService.BeginGetArchivePage(filter, page, clientUid, userName));
		}

		public OperationResult<int> GetArchiveCount(Guid clientUID, JournalFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetArchiveCount(clientUID, filter), "GetArchiveCount");
		}
		#endregion

		#region GK

		public void CancelGKProgress(Guid clientUID, Guid progressCallbackUID, string userName)
		{
			SafeOperationCall(clientUID, () => { RubezhService.CancelGKProgress(clientUID, progressCallbackUID, userName); }, "CancelGKProgress");
		}

		public OperationResult<bool> GKWriteConfiguration(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKWriteConfiguration(clientUID, deviceUID); }, "GKWriteConfiguration");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKReadConfiguration(clientUID, deviceUID); }, "GKReadConfiguration");
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKReadConfigurationFromGKFile(clientUID, deviceUID); }, "GKReadConfigurationFromGKFile");
		}

		public Stream GetServerFile(Guid clientUID, string filePath)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetServerFile(clientUID, filePath); }, "GetServerFile");
		}

		public OperationResult<GKDevice> GKAutoSearch(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKAutoSearch(clientUID, deviceUID); }, "GKAutoSearch");
		}

		public OperationResult<bool> GKUpdateFirmware(Guid clientUID, Guid deviceUID, List<byte> firmwareBytes)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKUpdateFirmware(clientUID, deviceUID, firmwareBytes); }, "GKUpdateFirmware");
		}

		public OperationResult<bool> GKSyncronyseTime(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKSyncronyseTime(clientUID, deviceUID); }, "GKSyncronyseTime");
		}

		public OperationResult<string> GKGetDeviceInfo(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKGetDeviceInfo(clientUID, deviceUID); }, "GKGetDeviceInfo");
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKGetJournalItemsCount(clientUID, deviceUID); }, "GKGetJournalItemsCount");
		}

		public OperationResult<JournalItem> GKReadJournalItem(Guid clientUID, Guid deviceUID, int no)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKReadJournalItem(clientUID, deviceUID, no); }, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(Guid clientUID, Guid objectUID, List<byte> parameterBytes, List<GKProperty> deviceProperties)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKSetSingleParameter(clientUID, objectUID, parameterBytes, deviceProperties); }, "GKSetSingleParameter");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(Guid clientUID, Guid objectUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKGetSingleParameter(clientUID, objectUID); }, "GKGetSingleParameter");
		}

		public OperationResult<bool> GKRewriteAllSchedules(Guid clientUID, Guid gkDeviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKRewriteAllSchedules(clientUID, gkDeviceUID); }, "GKRewriteAllSchedules");
		}

		public OperationResult<bool> GKSetSchedule(Guid clientUID, GKSchedule schedule)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKSetSchedule(clientUID, schedule); }, "GKSetSchedule");
		}

		public OperationResult<bool> GKGetUsers(Guid clientUID, Guid gkDeviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKGetUsers(clientUID, gkDeviceUID); }, "GKGetUsers");
		}

		public OperationResult<bool> GKRewriteUsers(Guid clientUID, Guid gkDeviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKRewriteUsers(clientUID, gkDeviceUID); }, "GKRewriteUsers");
		}

		public OperationResult<List<GKUser>> GetGKUsers(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetGKUsers(clientUID, deviceUID); }, "GetGKUsers");
		}

		public OperationResult<bool> RewritePmfUsers(Guid clientUID, Guid uid, List<GKUser> users)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.RewritePmfUsers(clientUID, uid, users); }, "RewritePmfUsers");
		}

		public OperationResult<List<byte>> GKGKHash(Guid clientUID, Guid gkDeviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKGKHash(clientUID, gkDeviceUID); }, "GKGKHash");
		}

		public GKStates GKGetStates(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKGetStates(clientUID); }, "GKGetStates");
		}
		public void GKExecuteDeviceCommand(Guid clientUID, Guid deviceUID, GKStateBit stateBit, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKExecuteDeviceCommand(clientUID, userName, deviceUID, stateBit); }, "GKExecuteDeviceCommand");
		}

		public void GKReset(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKReset(clientUID, userName, uid, objectType); }, "GKReset");
		}

		public void GKResetFire1(Guid clientUID, Guid zoneUID)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKResetFire1(clientUID, zoneUID); }, "GKResetFire1");
		}

		public void GKResetFire2(Guid clientUID, Guid zoneUID)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKResetFire2(clientUID, zoneUID); }, "GKResetFire2");
		}

		public void GKSetAutomaticRegime(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKSetAutomaticRegime(clientUID, userName, uid, objectType); }, "GKSetAutomaticRegime");
		}

		public void GKSetManualRegime(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKSetManualRegime(clientUID, userName, uid, objectType); }, "GKSetManualRegime");
		}

		public void GKSetIgnoreRegime(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKSetIgnoreRegime(clientUID, userName, uid, objectType); }, "GKSetIgnoreRegime");
		}

		public void GKTurnOn(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKTurnOn(clientUID, userName, uid, objectType); }, "GKTurnOn");
		}

		public void GKTurnOnNow(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKTurnOnNow(clientUID, userName, uid, objectType); }, "GKTurnOnNow");
		}

		public void GKTurnOnInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKTurnOnInAutomatic(clientUID, uid, objectType); }, "GKTurnOnInAutomatic");
		}

		public void GKTurnOnNowInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKTurnOnNowInAutomatic(clientUID, uid, objectType); }, "GKTurnOnNowInAutomatic");
		}

		public void GKTurnOff(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKTurnOff(clientUID, userName, uid, objectType); }, "GKTurnOff");
		}

		public void GKTurnOffNow(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKTurnOffNow(clientUID, userName, uid, objectType); }, "GKTurnOffNow");
		}

		public void GKTurnOffInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKTurnOffInAutomatic(clientUID, userName, uid, objectType); }, "GKTurnOffInAutomatic");
		}

		public void GKTurnOffNowInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKTurnOffNowInAutomatic(clientUID, uid, objectType); }, "GKTurnOffNowInAutomatic");
		}

		public void GKTurnOnNowGlobalPimsInAutomatic(Guid clientUID)
		{
			SafeOperationCall(clientUID, () => RubezhService.GKTurnOnNowGlobalPimsInAutomatic(clientUID), "GKTurnOnNowGlobalPimsInAutomatic");
		}

		public void GKTurnOffNowGlobalPimsInAutomatic(Guid clientUID)
		{
			SafeOperationCall(clientUID, () => RubezhService.GKTurnOffNowGlobalPimsInAutomatic(clientUID), "GKTurnOffNowGlobalPimsInAutomatic");
		}

		public void GKStop(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKStop(clientUID, userName, uid, objectType); }, "GKStop");
		}

		public void SendOn2OPKS(Guid clientUID, Guid uid, GKBaseObjectType objectType, string userName = null)
		{
			SafeOperationCall(clientUID, () => { RubezhService.SendOn2OPKS(clientUID, userName, uid, objectType); }, "GKStop");
		}

		public void GKStartMeasureMonitoring(Guid clientUID, Guid deviceUID)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKStartMeasureMonitoring(clientUID, deviceUID); }, "GKStartMeasureMonitoring");
		}

		public void GKStopMeasureMonitoring(Guid clientUID, Guid deviceUID)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKStopMeasureMonitoring(clientUID, deviceUID); }, "GKStopMeasureMonitoring");
		}

		public OperationResult<uint> GKGetReaderCode(Guid clientUID, Guid deviceUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GKGetReaderCode(clientUID, deviceUID); }, "GKGetReaderCode");
		}

		public void GKOpenSKDZone(Guid clientUID, Guid zoneUID)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKOpenSKDZone(clientUID, zoneUID); }, "GKOpenSKDZone");
		}

		public void GKCloseSKDZone(Guid clientUID, Guid zoneUID)
		{
			SafeOperationCall(clientUID, () => { RubezhService.GKCloseSKDZone(clientUID, zoneUID); }, "GKCloseSKDZone");
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid clientUID, Guid alsUid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetAlsMeasure(clientUID, alsUid), "GetAlsMeasure");
		}

		#endregion

		#region Automation

		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.RunProcedure(clientUID, procedureUID, args); }, "RunProcedure");
		}

		public void ProcedureCallbackResponse(Guid clientUID, Guid procedureThreadUID, object value)
		{
			SafeOperationCall(clientUID, () => RubezhService.ProcedureCallbackResponse(clientUID, procedureThreadUID, value), "ProcedureCallbackResponse");
		}

		public ProcedureProperties GetProperties(Guid clientUID, Guid layoutUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetProperties(clientUID, layoutUID); }, "GetProperties");
		}

		public void SetVariableValue(Guid clientUID, Guid variableUid, object value)
		{
			SafeOperationCall(clientUID, () => RubezhService.SetVariableValue(clientUID, variableUid, value), "SetVariableValue");
		}

		public void AddJournalItemA(Guid clientUID, string message, Guid? objectUID = null)
		{
			SafeOperationCall(clientUID, () => RubezhService.AddJournalItemA(clientUID, message, objectUID), "AddJournalItem");
		}

		public void ControlGKDevice(Guid clientUID, Guid deviceUid, GKStateBit command)
		{
			SafeOperationCall(clientUID, () => RubezhService.ControlGKDevice(clientUID, deviceUid, command), "ControlGKDevice");
		}

		public void StartRecord(Guid clientUID, Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			SafeOperationCall(clientUID, () => RubezhService.StartRecord(clientUID, cameraUid, journalItemUid, eventUid, timeout), "StartRecord");
		}

		public void StopRecord(Guid clientUID, Guid cameraUid, Guid eventUid)
		{
			SafeOperationCall(clientUID, () => RubezhService.StopRecord(clientUID, cameraUid, eventUid), "StopRecord");
		}

		public void Ptz(Guid clientUID, Guid cameraUid, int ptzNumber)
		{
			SafeOperationCall(clientUID, () => RubezhService.Ptz(clientUID, cameraUid, ptzNumber), "Ptz");
		}

		public void RviAlarm(Guid clientUID, string name)
		{
			SafeOperationCall(clientUID, () => RubezhService.RviAlarm(clientUID, name), "RviAlarm");
		}

		public void RviOpenWindow(Guid clientUid, string name, int x, int y, int monitorNumber, string login, string ip)
		{
			SafeOperationCall(clientUid, () => RubezhService.RviOpenWindow(clientUid, name, x, y, monitorNumber, login, ip), "RviOpenWindow");
		}

		public void ControlFireZone(Guid clientUID, Guid uid, ZoneCommandType commandType)
		{
			SafeOperationCall(clientUID, () => RubezhService.ControlFireZone(clientUID, uid, commandType), "ControlFireZone");
		}

		public void ControlGuardZone(Guid clientUID, Guid uid, GuardZoneCommandType commandType)
		{
			SafeOperationCall(clientUID, () => RubezhService.ControlGuardZone(clientUID, uid, commandType), "ControlGuardZone");
		}

		public void ControlDirection(Guid clientUID, Guid uid, DirectionCommandType commandType)
		{
			SafeOperationCall(clientUID, () => RubezhService.ControlDirection(clientUID, uid, commandType), "ControlDirection");
		}

		public void ControlGKDoor(Guid clientUID, Guid uid, GKDoorCommandType commandType)
		{
			SafeOperationCall(clientUID, () => RubezhService.ControlGKDoor(clientUID, uid, commandType), "ControlGKDoor");
		}

		public void ControlDelay(Guid clientUID, Guid uid, DelayCommandType commandType)
		{
			SafeOperationCall(clientUID, () => RubezhService.ControlDelay(clientUID, uid, commandType), "ControlDelay");
		}

		public void ControlPumpStation(Guid clientUID, Guid uid, PumpStationCommandType commandType)
		{
			SafeOperationCall(clientUID, () => RubezhService.ControlPumpStation(clientUID, uid, commandType), "ControlPumpStation");
		}

		public void ControlMPT(Guid clientUID, Guid uid, MPTCommandType commandType)
		{
			SafeOperationCall(clientUID, () => RubezhService.ControlMPT(clientUID, uid, commandType), "ControlMPT");
		}

		public void ExportJournalA(Guid clientUID, bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			SafeOperationCall(clientUID, () => RubezhService.ExportJournalA(clientUID, isExportJournal, isExportPassJournal, minDate, maxDate, path), "ExportJournal");
		}

		public void ExportOrganisationA(Guid clientUID, bool isWithDeleted, Guid organisationUid, string path)
		{
			SafeOperationCall(clientUID, () => RubezhService.ExportOrganisationA(clientUID, isWithDeleted, organisationUid, path), "ExportOrganisation");
		}

		public void ExportOrganisationListA(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(clientUID, () => RubezhService.ExportOrganisationListA(clientUID, isWithDeleted, path), "ExportOrganisationList");
		}

		public void ExportConfigurationA(Guid clientUID, bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			SafeOperationCall(clientUID, () => RubezhService.ExportConfigurationA(clientUID, isExportDevices, isExportDoors, isExportZones, path), "ExportConfiguration");
		}

		public void ImportOrganisationA(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(clientUID, () => RubezhService.ImportOrganisationA(clientUID, isWithDeleted, path), "ImportOrganisation");
		}

		public void ImportOrganisationListA(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(clientUID, () => RubezhService.ImportOrganisationListA(clientUID, isWithDeleted, path), "ImportOrganisationList");
		}
		#endregion

		#region OPC DA Server

		public OperationResult<OpcDaServer[]> GetOpcDaServers(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetOpcDaServers(); }, "GetOpcDaServerNames");
		}

		public OperationResult<OpcDaElement[]> GetOpcDaServerGroupAndTags(Guid clientUID, OpcDaServer server)
		{
			return SafeOperationCall(clientUID, () =>
			{ return RubezhService.GetOpcDaServerGroupAndTags(server); }, "GetOpcDaServerGroupAndTags");
		}

		public OperationResult<OpcServerStatus> GetOpcDaServerStatus(Guid clientUID, OpcDaServer server)
		{
			return SafeOperationCall(clientUID, () =>
			{ return RubezhService.GetOpcDaServerStatus(server); }, "GetOpcDaServerStatus");
		}

		public OperationResult<OpcDaTagValue[]> ReadOpcDaServerTags(Guid clientUID, OpcDaServer server)
		{
			var result = SafeOperationCall(clientUID, () =>
			{ return RubezhService.ReadOpcDaServerTags(server); }, "ReadOpcDaServerTags");
			return result;
		}

		public OperationResult<bool> WriteOpcDaTag(Guid clientUID, Guid tagId, Object value)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.WriteOpcDaTag(tagId, value); },
				"WriteOpcDaTag");
		}
		public List<RviState> GetRviStates(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => { return RubezhService.GetRviStates(); }, "GetRviStates");
		}

		#endregion
	}
}