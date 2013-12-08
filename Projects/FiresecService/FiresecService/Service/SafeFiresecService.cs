using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.Skud;
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
		public IEnumerable<EmployeeCard> GetEmployees(EmployeeCardIndexFilter filter)
        {
            return SafeContext.Execute<IEnumerable<EmployeeCard>>(() => FiresecService.GetEmployees(filter));
        }
        public bool DeleteEmployee(int id)
        {
            return SafeContext.Execute<bool>(() => FiresecService.DeleteEmployee(id));
        }
        public EmployeeCardDetails GetEmployeeCard(int id)
        {
            return SafeContext.Execute<EmployeeCardDetails>(() => FiresecService.GetEmployeeCard(id));
        }
        public int SaveEmployeeCard(EmployeeCardDetails employeeCard)
        {
            return SafeContext.Execute<int>(() => FiresecService.SaveEmployeeCard(employeeCard));
        }
        public IEnumerable<EmployeeDepartment> GetEmployeeDepartments()
        {
            return SafeContext.Execute<IEnumerable<EmployeeDepartment>>(() => FiresecService.GetEmployeeDepartments());
        }
        public IEnumerable<EmployeeGroup> GetEmployeeGroups()
        {
            return SafeContext.Execute<IEnumerable<EmployeeGroup>>(() => FiresecService.GetEmployeeGroups());
        }
        public IEnumerable<EmployeePosition> GetEmployeePositions()
        {
            return SafeContext.Execute<IEnumerable<EmployeePosition>>(() => FiresecService.GetEmployeePositions());
        }
		#endregion

		#region GK
		public void AddJournalItem(JournalItem journalItem)
		{
			SafeOperationCall(() => { FiresecService.AddJournalItem(journalItem); }, "AddJournalItem");
		}

		public OperationResult<bool> GKWriteConfiguration(Guid deviceUID, bool writeFileToGK)
		{
			return SafeOperationCall(() => { return FiresecService.GKWriteConfiguration(deviceUID, writeFileToGK); }, "GKWriteConfiguration");
		}

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadConfiguration(deviceUID); }, "GKReadConfiguration");
		}

		public void GKUpdateFirmware(Guid deviceUID, string fileName)
		{
			SafeOperationCall(() => { FiresecService.GKUpdateFirmware(deviceUID, fileName); }, "GKUpdateFirmware");
		}

		public bool GKSyncronyseTime(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKSyncronyseTime(deviceUID); }, "GKSyncronyseTime");
		}

		public string GKGetDeviceInfo(Guid deviceUID)
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

		public OperationResult<bool> GKSetSingleParameter(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKSetSingleParameter(deviceUID); }, "GKSetSingleParameter");
		}

		public OperationResult<bool> GKGetSingleParameter(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetSingleParameter(deviceUID); }, "GKGetSingleParameter");
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
		#endregion
	}
}