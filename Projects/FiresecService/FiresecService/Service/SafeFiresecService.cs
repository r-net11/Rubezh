using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using Common;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.Journal;

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

		public OperationResult<T> CreateEmptyOperationResult<T>(string message)
		{
			var operationResult = new OperationResult<T>
			{
				Result = default(T),
				HasError = true,
				Error = "Ошибка при выполнении операции на сервере" + "\n\r" + message
			};
			return operationResult;
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
				return CreateEmptyOperationResult<T>(e.Message + "\n" + e.StackTrace);
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
		public void BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			SafeOperationCall(() => { FiresecService.BeginGetFilteredArchive(archiveFilter, archivePortionUID); }, "BeginGetFilteredArchive");
		}
		public List<JournalEventDescriptionType> GetDistinctEventDescriptions()
		{
			return SafeContext.Execute<List<JournalEventDescriptionType>>(() => FiresecService.GetDistinctEventDescriptions());
		}
		public List<JournalEventNameType> GetDistinctEventNames()
		{
			return SafeContext.Execute<List<JournalEventNameType>>(() => FiresecService.GetDistinctEventNames());
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

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadConfiguration(deviceUID); }, "GKReadConfiguration");
		}

		public OperationResult<XDeviceConfiguration> GKReadConfigurationFromGKFile(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadConfigurationFromGKFile(deviceUID); }, "GKReadConfigurationFromGKFile");
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

		public OperationResult<XJournalItem> GKReadJournalItem(Guid deviceUID, int no)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadJournalItem(deviceUID, no); }, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes)
		{
			return SafeOperationCall(() => { return FiresecService.GKSetSingleParameter(objectUID, parameterBytes); }, "GKSetSingleParameter");
		}

		public OperationResult<List<XProperty>> GKGetSingleParameter(Guid objectUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetSingleParameter(objectUID); }, "GKGetSingleParameter");
		}

		public OperationResult<List<byte>> GKGKHash(Guid gkDeviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGKHash(gkDeviceUID); }, "GKGKHash");
		}

		public GKStates GKGetStates()
		{
			return SafeOperationCall(() => { return FiresecService.GKGetStates(); }, "GKGetStates");
		}
		public void GKExecuteDeviceCommand(Guid deviceUID, XStateBit stateBit)
		{
			SafeOperationCall(() => { FiresecService.GKExecuteDeviceCommand(deviceUID, stateBit); }, "GKExecuteDeviceCommand");
		}

		public void GKReset(Guid uid, XBaseObjectType objectType)
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

		public void GKSetAutomaticRegime(Guid uid, XBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKSetAutomaticRegime(uid, objectType); }, "GKSetAutomaticRegime");
		}

		public void GKSetManualRegime(Guid uid, XBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKSetManualRegime(uid, objectType); }, "GKSetManualRegime");
		}

		public void GKSetIgnoreRegime(Guid uid, XBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKSetIgnoreRegime(uid, objectType); }, "GKSetIgnoreRegime");
		}

		public void GKTurnOn(Guid uid, XBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOn(uid, objectType); }, "GKTurnOn");
		}

		public void GKTurnOnNow(Guid uid, XBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnNow(uid, objectType); }, "GKTurnOnNow");
		}

		public void GKTurnOff(Guid uid, XBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOff(uid, objectType); }, "GKTurnOff");
		}

		public void GKTurnOffNow(Guid uid, XBaseObjectType objectType)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffNow(uid, objectType); }, "GKTurnOffNow");
		}

		public void GKStop(Guid uid, XBaseObjectType objectType)
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

		public List<XJournalItem> GetGKTopLastJournalItems(int count)
		{
			return SafeOperationCall(() => { return FiresecService.GetGKTopLastJournalItems(count); }, "GetGKTopLastJournalItems");
		}

		public void BeginGetGKFilteredArchive(XArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			SafeOperationCall(() => { FiresecService.BeginGetGKFilteredArchive(archiveFilter, archivePortionUID); }, "BeginGetGKFilteredArchive");
		}

		public List<string> GetDistinctGKJournalNames()
		{
			return SafeOperationCall(() => { return FiresecService.GetDistinctGKJournalNames(); }, "GetDistinctGKJournalNames");
		}

		public List<string> GetDistinctGKJournalDescriptions()
		{
			return SafeOperationCall(() => { return FiresecService.GetDistinctGKJournalDescriptions(); }, "GetDistinctGKJournalDescriptions");
		}
		#endregion

		#region Automation
		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args)
		{
			return SafeOperationCall(() => { return FiresecService.RunProcedure(procedureUID, args); }, "RunProcedure");
		}
		#endregion
	}
}