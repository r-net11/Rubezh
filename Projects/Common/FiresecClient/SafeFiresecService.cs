using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Timers;
using FiresecAPI;
using FiresecAPI.Models;
using Common;
using FiresecAPI.Models.Skud;

namespace FiresecClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class SafeFiresecService : IFiresecService
    {
        public SafeFiresecService(IFiresecService iFiresecService)
        {
            _iFiresecService = iFiresecService;
        }

        IFiresecService _iFiresecService;

        public static event Action ConnectionLost;
        void OnConnectionLost()
        {
            if (_isConnected == false)
                return;

            if (ConnectionLost != null)
                ConnectionLost();

            _isConnected = false;
        }

        public static event Action ConnectionAppeared;
        void OnConnectionAppeared()
        {
            if (_isConnected == true)
                return;

            if (ConnectionAppeared != null)
                ConnectionAppeared();

            _isConnected = true;
        }

        bool _isConnected = true;

        System.Timers.Timer _pingTimer;

        public void StartPing()
        {
            if (_pingTimer != null)
            {
                _pingTimer = new System.Timers.Timer();
                _pingTimer.Elapsed += new ElapsedEventHandler(OnTimerPing);
                _pingTimer.Interval = 1000;
                _pingTimer.Enabled = true;
            }
        }

        public void StopPing()
        {
            if (_pingTimer != null)
            {
                _pingTimer.Enabled = false;
                _pingTimer.Dispose();
            }
        }

        private void OnTimerPing(object source, ElapsedEventArgs e)
        {
            Ping();
        }

        public string Ping()
        {
            try
            {
                var result = _iFiresecService.Ping();
                OnConnectionAppeared();
                return result;
            }
            catch (CommunicationObjectFaultedException)
            {
                OnConnectionLost();
            }
            catch (InvalidOperationException)
            {
                OnConnectionLost();
            }
            catch (CommunicationException)
            {
                OnConnectionLost();
            }
            catch (Exception)
            {
                OnConnectionLost();
            }
            return null;
        }

        OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> func)
        {
            try
            {
                var result = func();
                if (result != null)
                    return result;
            }
            catch(Exception e)
            {
                Logger.Error(e);
                OnConnectionLost();
            }
            var operationResult = new OperationResult<T>()
            {
                HasError = true,
                Error = "Ошибка при при вызове операции на клиенте"
            };
            return operationResult;
        }

        T SafeOperationCall<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch(Exception e)
            {
                Logger.Error(e);
                OnConnectionLost();
            }
            return default(T);
        }

        void SafeOperationCall(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                OnConnectionLost();
            }
        }

        public OperationResult<bool> Connect(string clientType, string clientCallbackAddress, string userName, string password)
        {
            return SafeOperationCall(() =>
            {
                try
                {
                    return _iFiresecService.Connect(clientType, clientCallbackAddress, userName, password);
                }
                catch { }
                return new OperationResult<bool>()
                {
                    Result = false,
                    HasError = true,
                    Error = "Не удается соединиться с сервером"
                };
            });
        }

        public OperationResult<bool> Reconnect(string userName, string password)
        {
            return SafeOperationCall(() => { return _iFiresecService.Reconnect(userName, password); });
        }

        public void Disconnect()
        {
            SafeOperationCall(() => { _iFiresecService.Disconnect(); });
        }

        public void Subscribe()
        {
            SafeOperationCall(() => { _iFiresecService.Subscribe(); });
        }

        public string GetStatus()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetStatus(); });
        }

        public void CancelProgress()
        {
            SafeOperationCall(() => { _iFiresecService.CancelProgress(); });
        }

        public List<Driver> GetDrivers()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetDrivers(); });
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetDeviceConfiguration(); });
        }

        public OperationResult<DeviceConfiguration> DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceReadConfiguration(deviceConfiguration, deviceUID); });
        }

        public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceAutoDetectChildren(deviceConfiguration, deviceUID, fastSearch); });
        }

        public OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(Guid driverUID)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceCustomFunctionList(driverUID); });
        }

        public PlansConfiguration GetPlansConfiguration()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetPlansConfiguration(); });
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            SafeOperationCall(() => { _iFiresecService.SetPlansConfiguration(plansConfiguration); });
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetSystemConfiguration(); });
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            SafeOperationCall(() => { _iFiresecService.SetSystemConfiguration(systemConfiguration); });
        }

        public LibraryConfiguration GetLibraryConfiguration()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetLibraryConfiguration(); });
        }

        public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
        {
            SafeOperationCall(() => { _iFiresecService.SetLibraryConfiguration(libraryConfiguration); });
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetSecurityConfiguration(); });
        }

        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            SafeOperationCall(() => { _iFiresecService.SetSecurityConfiguration(securityConfiguration); });
        }

        public DeviceConfigurationStates GetStates()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetStates(); });
        }

        public OperationResult<List<JournalRecord>> ReadJournal(int startIndex, int count)
        {
            return SafeOperationCall(() => { return _iFiresecService.ReadJournal(startIndex, count); });
        }

        public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
        {
            return SafeOperationCall(() => { return _iFiresecService.GetFilteredJournal(journalFilter); });
        }

        public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            return SafeOperationCall(() => { return _iFiresecService.GetFilteredArchive(archiveFilter); });
        }

        public OperationResult<List<JournalRecord>> GetDistinctRecords()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetDistinctRecords(); });
        }

        public OperationResult<DateTime> GetArchiveStartDate()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetArchiveStartDate(); });
        }

        public void AddJournalRecord(JournalRecord journalRecord)
        {
            SafeOperationCall(() => { _iFiresecService.AddJournalRecord(journalRecord); });
        }

        public List<string> GetFileNamesList(string directory)
        {
            return SafeOperationCall(() => { return _iFiresecService.GetFileNamesList(directory); });
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            return SafeOperationCall(() => { return _iFiresecService.GetDirectoryHash(directory); });
        }

        public System.IO.Stream GetFile(string dirAndFileName)
        {
            return SafeOperationCall(() => { return _iFiresecService.GetFile(dirAndFileName); });
        }

        public void ConvertConfiguration()
        {
            SafeOperationCall(() => { _iFiresecService.ConvertConfiguration(); });
        }

        public void ConvertJournal()
        {
            SafeOperationCall(() => { _iFiresecService.ConvertJournal(); });
        }

        public string Test()
        {
            return SafeOperationCall(() => { return _iFiresecService.Test(); });
        }

        public void SetXDeviceConfiguration(XFiresecAPI.XDeviceConfiguration xDeviceConfiguration)
        {
            SafeOperationCall(() => { _iFiresecService.SetXDeviceConfiguration(xDeviceConfiguration); });
        }

        public XFiresecAPI.XDeviceConfiguration GetXDeviceConfiguration()
        {
            return SafeOperationCall(() => { return _iFiresecService.GetXDeviceConfiguration(); });
        }

        public OperationResult<bool> SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            return SafeOperationCall(() => { return _iFiresecService.SetDeviceConfiguration(deviceConfiguration); });
        }

        public OperationResult<bool> DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceWriteConfiguration(deviceConfiguration, deviceUID); });
        }

        public OperationResult<bool> DeviceWriteAllConfiguration(DeviceConfiguration deviceConfiguration)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceWriteAllConfiguration(deviceConfiguration); });
        }

        public OperationResult<bool> DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceSetPassword(deviceConfiguration, deviceUID, devicePasswordType, password); });
        }

        public OperationResult<bool> DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceDatetimeSync(deviceConfiguration, deviceUID); });
        }

        public OperationResult<string> DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceGetInformation(deviceConfiguration, deviceUID); });
        }

        public OperationResult<List<string>> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceGetSerialList(deviceConfiguration, deviceUID); });
        }

        public OperationResult<string> DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceUpdateFirmware(deviceConfiguration, deviceUID, bytes, fileName); });
        }

        public OperationResult<string> DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceVerifyFirmwareVersion(deviceConfiguration, deviceUID, bytes, fileName); });
        }

        public OperationResult<string> DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceReadEventLog(deviceConfiguration, deviceUID); });
        }

        public OperationResult<string> DeviceCustomFunctionExecute(DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceCustomFunctionExecute(deviceConfiguration, deviceUID, functionName); });
        }

        public OperationResult<string> DeviceGetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceGetGuardUsersList(deviceConfiguration, deviceUID); });
        }

        public OperationResult<bool> DeviceSetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID, string users)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceSetGuardUsersList(deviceConfiguration, deviceUID, users); });
        }

        public OperationResult<string> DeviceGetMDS5Data(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            return SafeOperationCall(() => { return _iFiresecService.DeviceGetMDS5Data(deviceConfiguration, deviceUID); });
        }

        public OperationResult<bool> AddToIgnoreList(List<Guid> deviceUIDs)
        {
            return SafeOperationCall(() => { return _iFiresecService.AddToIgnoreList(deviceUIDs); });
        }

        public OperationResult<bool> RemoveFromIgnoreList(List<Guid> deviceUIDs)
        {
            return SafeOperationCall(() => { return _iFiresecService.RemoveFromIgnoreList(deviceUIDs); });
        }

        public void ResetStates(List<ResetItem> resetItems)
        {
            SafeOperationCall(() => { _iFiresecService.ResetStates(resetItems); });
        }

        public void AddUserMessage(string message)
        {
            SafeOperationCall(() => { _iFiresecService.AddUserMessage(message); });
        }

        public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
        {
            return SafeOperationCall(() => { return _iFiresecService.ExecuteCommand(deviceUID, methodName); });
        }

        public OperationResult<bool> CheckHaspPresence()
        {
            return SafeOperationCall(() => { return _iFiresecService.CheckHaspPresence(); });
        }

		public IEnumerable<EmployeeCardIndex> GetEmployees(EmployeeCardIndexFilter filter)
		{
			return SafeContext.Execute<IEnumerable<EmployeeCardIndex>>(() => _iFiresecService.GetEmployees(filter));
		}

		public bool DeleteEmployee(int id)
		{
			return SafeContext.Execute<bool>(() => _iFiresecService.DeleteEmployee(id));
		}

		public EmployeeCard GetEmployeeCard(int id)
		{
			return SafeContext.Execute<EmployeeCard>(() => _iFiresecService.GetEmployeeCard(id));
		}

		public int SaveEmployeeCard(EmployeeCard employeeCard)
		{
			return SafeContext.Execute<int>(() => _iFiresecService.SaveEmployeeCard(employeeCard));
		}
	}
}