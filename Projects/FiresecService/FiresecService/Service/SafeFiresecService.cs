using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.SKDDatabase;
using XFiresecAPI;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SafeFiresecService : IFiresecService
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

        public OperationResult<int> GetJournalLastId()
        {
            return SafeOperationCall(() => { return FiresecService.GetJournalLastId(); }, "GetJournalLastId");
        }

        public OperationResult<List<FiresecAPI.Models.JournalRecord>> GetFilteredJournal(FiresecAPI.Models.JournalFilter journalFilter)
        {
            return SafeOperationCall(() => { return FiresecService.GetFilteredJournal(journalFilter); }, "GetFilteredJournal");
        }

        public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            return SafeOperationCall(() => { return FiresecService.GetFilteredArchive(archiveFilter); }, "GetFilteredArchive");
        }

        public void BeginGetFilteredArchive(FiresecAPI.Models.ArchiveFilter archiveFilter)
        {
            SafeOperationCall(() => { FiresecService.BeginGetFilteredArchive(archiveFilter); }, "BeginGetFilteredArchive");
        }

        public OperationResult<List<FiresecAPI.Models.JournalDescriptionItem>> GetDistinctDescriptions()
        {
            return SafeOperationCall(() => { return FiresecService.GetDistinctDescriptions(); }, "GetDistinctDescriptions");
        }

        public OperationResult<DateTime> GetArchiveStartDate()
        {
            return SafeOperationCall(() => { return FiresecService.GetArchiveStartDate(); }, "GetArchiveStartDate");
        }

		public void AddJournalRecords(List<JournalRecord> journalRecords)
        {
            SafeOperationCall(() => { FiresecService.AddJournalRecords(journalRecords); }, "AddJournalRecords");
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

		public void SetJournal(List<JournalRecord> journalRecords)
        {
			SafeOperationCall(() => { FiresecService.SetJournal(journalRecords); }, "ConvertJournal");
        }

        public string Test(string arg)
        {
            return SafeOperationCall(() => { return FiresecService.Test(arg); }, "Test");
        }

		#region SKD
		public IEnumerable<Employee>GetEmployees(EmployeeFilter filter)
        {
            return SafeContext.Execute<IEnumerable<Employee>>(() => FiresecService.GetEmployees(filter));
        }
        public IEnumerable<Position> GetPositions(PositionFilter filter)
        {
            return SafeContext.Execute<IEnumerable<Position>>(() => FiresecService.GetPositions(filter));
        }
        public IEnumerable<Department> GetDepartments(DepartmentFilter filter)
        {
            return SafeContext.Execute<IEnumerable<Department>>(() => FiresecService.GetDepartments(filter));
        }
		public IEnumerable<SKDJournalItem> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return SafeContext.Execute<IEnumerable<SKDJournalItem>>(() => FiresecService.GetSKDJournalItems(filter));
		}
		public IEnumerable<Frame> GetFrames(FrameFilter filter)
		{
			return SafeContext.Execute<IEnumerable<Frame>>(() => FiresecService.GetFrames(filter));
		}
		public IEnumerable<Card> GetCards(CardFilter filter)
		{
			return SafeContext.Execute<IEnumerable<Card>>(() => FiresecService.GetCards(filter));
		}
		public IEnumerable<CardZoneLink> GetCardZoneLinks(CardZoneLinkFilter filter)
		{
			return SafeContext.Execute<IEnumerable<CardZoneLink>>(() => FiresecService.GetCardZoneLinks(filter));
		}
		public void SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			SafeContext.Execute(() => FiresecService.SaveSKDJournalItems(journalItems));
		}
		public void SaveFrames(IEnumerable<Frame> frames)
		{
			SafeContext.Execute(() => FiresecService.SaveFrames(frames));
		}
		public void SaveCards(IEnumerable<Card> items)
		{
			SafeContext.Execute(() => FiresecService.SaveCards(items));
		}
		public void SaveCardZoneLinks(IEnumerable<CardZoneLink> items)
		{
			SafeContext.Execute(() => FiresecService.SaveCardZoneLinks(items));
		}


		public OperationResult<string> SKDGetDeviceInfo(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetDeviceInfo(deviceUID); }, "SKDGetDeviceInfo");
		}
		public OperationResult<bool> SKDSyncronyseTime(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSyncronyseTime(deviceUID); }, "SKDSyncronyseTime");
		}
		public OperationResult<bool> SKDWriteConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteConfiguration(deviceUID); }, "SKDWriteConfiguration");
		}
		public OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.SKDUpdateFirmware(deviceUID, fileName); }, "SKDUpdateFirmware");
		}
		public void SKDSetRegimeOpen(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeOpen(deviceUID); }, "SKDSetIgnoreRegime");
		}
		public void SKDSetRegimeClose(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeClose(deviceUID); }, "SKDSetIgnoreRegime");
		}
		public void SKDSetRegimeControl(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeControl(deviceUID); }, "SKDSetRegimeControl");
		}
		public void SKDSetRegimeConversation(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeConversation(deviceUID); }, "SKDSetRegimeConversation");
		}
		public void SKDOpenDevice(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDOpenDevice(deviceUID); }, "SKDOpenDevice");
		}
		public void SKDCloseDevice(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDCloseDevice(deviceUID); }, "SKDCloseDevice");
		}
		public void SKDAllowReader(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDAllowReader(deviceUID); }, "SKDAllowReader");
		}

		public void SKDDenyReader(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDDenyReader(deviceUID); }, "SKDDenyReader");
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

		public OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no)
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


		public void AddJournalItem(JournalItem journalItem)
		{
			SafeOperationCall(() => { FiresecService.AddJournalItem(journalItem); }, "AddJournalItem");
		}

		public List<JournalItem> GetGKTopLastJournalItems(int count)
		{
			return SafeOperationCall(() => { return FiresecService.GetGKTopLastJournalItems(count); }, "GetGKTopLastJournalItems");
		}

		public void BeginGetGKFilteredArchive(XArchiveFilter archiveFilter)
		{
			SafeOperationCall(() => { FiresecService.BeginGetGKFilteredArchive(archiveFilter); }, "BeginGetGKFilteredArchive");
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
	}
}