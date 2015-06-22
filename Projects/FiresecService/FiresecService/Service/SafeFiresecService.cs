using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using Common;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.AutomationCallback;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.Models;

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

		public OperationResult<bool> Reconnect(Guid uid, string userName, string password)
		{
			return SafeOperationCall(() => { return FiresecService.Reconnect(uid, userName, password); }, "Reconnect");
		}

		public void Disconnect(Guid uid)
		{
			SafeOperationCall(() => { FiresecService.Disconnect(uid); }, "Disconnect");
		}

		public string Ping()
		{
			return SafeOperationCall(() => { return FiresecService.Ping(); }, "Ping");
		}

		public List<ServerTask> GetServerTasks()
		{
			return SafeOperationCall(() => { return FiresecService.GetServerTasks(); }, "GetServerTasks");
		}

		public OperationResult ResetDB()
		{
			return SafeOperationCall(() => { return FiresecService.ResetDB(); }, "ResetDB");
		}

		public List<CallbackResult> Poll(Guid uid)
		{
			return SafeContext.Execute<List<CallbackResult>>(() => FiresecService.Poll(uid));
		}

		public void NotifyClientsOnConfigurationChanged()
		{
			SafeOperationCall(() => { FiresecService.NotifyClientsOnConfigurationChanged(); }, "NotifyClientsOnConfigurationChanged");
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

		public System.IO.Stream GetFile(string dirAndFileName)
		{
			return SafeOperationCall(() => { return FiresecService.GetFile(dirAndFileName); }, "GetFile");
		}

		public Stream GetConfig()
		{
			return SafeOperationCall(() => { return FiresecService.GetConfig(); }, "GetConfig");
		}

		public void SetConfig(Stream stream)
		{
			SafeOperationCall(() => { FiresecService.SetConfig(stream); }, "SetConfig");
		}

		public void SetLocalConfig()
		{
			SafeOperationCall(() => { FiresecService.SetLocalConfig(); }, "SetLocalConfig");
		}

		public string Test(string arg)
		{
			return SafeOperationCall(() => { return FiresecService.Test(arg); }, "Test");
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
		public OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.BeginGetFilteredArchive(archiveFilter, archivePortionUID));
		}
		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			return SafeOperationCall(() => { return FiresecService.AddJournalItem(journalItem); }, "AddJournalItem");
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

		public Stream GKReadConfigurationFromGKFile(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadConfigurationFromGKFile(deviceUID); }, "GKReadConfigurationFromGKFile");
		}

		public OperationResult<GKDeviceConfiguration> GKAutoSearch(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKAutoSearch(deviceUID); }, "GKAutoSearch");
		}

		public OperationResult<bool> GKUpdateFirmware(Guid deviceUID, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.GKUpdateFirmware(deviceUID, fileName); }, "GKUpdateFirmware");
		}

		public OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<Guid> deviceUIDs)
		{
			var result = SafeOperationCall(() => { return FiresecService.GKUpdateFirmwareFSCS(hxcFileInfo, userName, deviceUIDs); }, "GKUpdateFirmwareFSCS");
			return result;
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

		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes)
		{
			return SafeOperationCall(() => { return FiresecService.GKSetSingleParameter(objectUID, parameterBytes); }, "GKSetSingleParameter");
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

		//public OperationResult<List<GKUser>> GKGetUsers(Guid gkDeviceUID)
		//{
		//	return SafeOperationCall(() => { return FiresecService.GKGetUsers(gkDeviceUID); }, "GKGetUsers");
		//}

		public OperationResult<bool> GKRewriteUsers(Guid gkDeviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKRewriteUsers(gkDeviceUID); }, "GKRewriteUsers");
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

		//public void GKOpenSKDZone(Guid zoneUID)
		//{
		//	SafeOperationCall(() => { FiresecService.GKOpenSKDZone(zoneUID); }, "GKOpenSKDZone");
		//}

		//public void GKCloseSKDZone(Guid zoneUID)
		//{
		//	SafeOperationCall(() => { FiresecService.GKCloseSKDZone(zoneUID); }, "GKCloseSKDZone");
		//}

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

		#endregion
	}
}