using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.Skud;
using FiresecService.ViewModels;

namespace FiresecService.Service
{
    [ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = true,
    InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
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

        public OperationResult<bool> Connect(ClientCredentials clientCredentials, bool isNew)
        {
            return SafeOperationCall(() => { return FiresecService.Connect(clientCredentials, isNew); }, "Connect");
        }

        public OperationResult<bool> Reconnect(string userName, string password)
        {
            return SafeOperationCall(() => { return FiresecService.Reconnect(userName, password); }, "Reconnect");
        }

        public void Disconnect()
        {
            SafeOperationCall(() => { FiresecService.Disconnect(); }, "Disconnect");
        }

        public string GetStatus()
        {
            return SafeOperationCall(() => { return FiresecService.GetStatus(); }, "GetStatus");
        }

		public DriversConfiguration GetDriversConfiguration()
        {
			return SafeOperationCall(() => { return FiresecService.GetDriversConfiguration(); }, "GetDriversConfiguration");
        }

        public FiresecAPI.Models.DeviceConfiguration GetDeviceConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetDeviceConfiguration(); }, "GetDeviceConfiguration");
        }

        public OperationResult<bool> SetDeviceConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
        {
            return SafeOperationCall(() => { return FiresecService.SetDeviceConfiguration(deviceConfiguration); }, "SetDeviceConfiguration");
        }

        public FiresecAPI.Models.SystemConfiguration GetSystemConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetSystemConfiguration(); }, "GetSystemConfiguration");
        }

        public void SetSystemConfiguration(FiresecAPI.Models.SystemConfiguration systemConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetSystemConfiguration(systemConfiguration); }, "SetSystemConfiguration");
        }

        public FiresecAPI.Models.LibraryConfiguration GetLibraryConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetLibraryConfiguration(); }, "GetLibraryConfiguration");
        }

        public void SetLibraryConfiguration(FiresecAPI.Models.LibraryConfiguration libraryConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetLibraryConfiguration(libraryConfiguration); }, "SetLibraryConfiguration");
        }

        public FiresecAPI.Models.PlansConfiguration GetPlansConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetPlansConfiguration(); }, "GetPlansConfiguration");
        }

        public void SetPlansConfiguration(FiresecAPI.Models.PlansConfiguration plansConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetPlansConfiguration(plansConfiguration); }, "SetPlansConfiguration");
        }

        public FiresecAPI.Models.SecurityConfiguration GetSecurityConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); }, "GetSecurityConfiguration");
        }

        public void SetSecurityConfiguration(FiresecAPI.Models.SecurityConfiguration securityConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetSecurityConfiguration(securityConfiguration); }, "SetSecurityConfiguration");
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

		public void SetJournal(List<JournalRecord> journalRecords)
        {
			SafeOperationCall(() => { FiresecService.SetJournal(journalRecords); }, "ConvertJournal");
        }

        public string Ping()
        {
            return SafeOperationCall(() => { return FiresecService.Ping(); }, "Ping");
        }

        public string Test(string arg)
        {
            return SafeOperationCall(() => { return FiresecService.Test(arg); }, "Test");
        }

        public void SetXDeviceConfiguration(XFiresecAPI.XDeviceConfiguration xDeviceConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetXDeviceConfiguration(xDeviceConfiguration); }, "SetXDeviceConfiguration");
        }

        public XFiresecAPI.XDeviceConfiguration GetXDeviceConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetXDeviceConfiguration(); }, "GetXDeviceConfiguration");
        }

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

        public IAsyncResult BeginPoll(int index, DateTime dateTime, AsyncCallback asyncCallback, object state)
        {
            return SafeContext.Execute<IAsyncResult>(() => FiresecService.BeginPoll(index, dateTime, asyncCallback, state));
        }
        public List<CallbackResult> EndPoll(IAsyncResult asyncResult)
        {
            return SafeContext.Execute<List<CallbackResult>>(() => FiresecService.EndPoll(asyncResult));
        }
    }
}