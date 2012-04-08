using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Timers;
using FiresecAPI;
using FiresecAPI.Models;
using Common;

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
        void OnConnectionLost(Exception ex)
        {
			Logger.Error(ex, "ConnectionLost");
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

        public string Connect(string clientCallbackAddress, string userName, string password)
        {
            try
            {
                return _iFiresecService.Connect(clientCallbackAddress, userName, password);
            }
			catch (Exception ex)
            {
                OnConnectionLost(ex);
            }
            return "Не удается соединиться с сервером";
        }

        public string Reconnect(string userName, string password)
        {
            try
            {
                return _iFiresecService.Reconnect(userName, password);
            }
            catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return "Не удается соединиться с сервером";
        }

        public void Disconnect()
        {
            try
            {
                _iFiresecService.Disconnect();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public void Subscribe()
        {
            try
            {
                _iFiresecService.Subscribe();
            }
			catch (Exception ex)
            {
                OnConnectionLost(ex);
            }
        }

        public void CancelProgress()
        {
            try
            {
                _iFiresecService.CancelProgress();
            }
			catch (Exception ex)
            {
                OnConnectionLost(ex);
            }
        }

        public List<Driver> GetDrivers()
        {
            try
            {
                return _iFiresecService.GetDrivers();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            try
            {
                return _iFiresecService.GetDeviceConfiguration();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            try
            {
                _iFiresecService.SetDeviceConfiguration(deviceConfiguration);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public void DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                _iFiresecService.DeviceWriteConfiguration(deviceConfiguration, deviceUID);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public void DeviceWriteAllConfiguration(DeviceConfiguration deviceConfiguration)
        {
            try
            {
                _iFiresecService.DeviceWriteAllConfiguration(deviceConfiguration);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public DeviceConfiguration DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return _iFiresecService.DeviceReadConfiguration(deviceConfiguration, deviceUID);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public bool DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password)
        {
            try
            {
                return _iFiresecService.DeviceSetPassword(deviceConfiguration, deviceUID, devicePasswordType, password);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return false;
            }
        }

        public bool DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return _iFiresecService.DeviceDatetimeSync(deviceConfiguration, deviceUID);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return false;
            }
        }

        public string DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return _iFiresecService.DeviceGetInformation(deviceConfiguration, deviceUID);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public List<string> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return _iFiresecService.DeviceGetSerialList(deviceConfiguration, deviceUID);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public string DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
        {
            try
            {
                return _iFiresecService.DeviceUpdateFirmware(deviceConfiguration, deviceUID, bytes, fileName);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public string DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
        {
            try
            {
                return _iFiresecService.DeviceVerifyFirmwareVersion(deviceConfiguration, deviceUID, bytes, fileName);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public string DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return _iFiresecService.DeviceReadEventLog(deviceConfiguration, deviceUID);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public DeviceConfiguration DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
        {
            try
            {
                return _iFiresecService.DeviceAutoDetectChildren(deviceConfiguration, deviceUID, fastSearch);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public List<DeviceCustomFunction> DeviceCustomFunctionList(Guid driverUID)
        {
            try
            {
                return _iFiresecService.DeviceCustomFunctionList(driverUID);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public string DeviceCustomFunctionExecute(DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
        {
            try
            {
                return _iFiresecService.DeviceCustomFunctionExecute(deviceConfiguration, deviceUID, functionName);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public string DeviceGetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return _iFiresecService.DeviceGetGuardUsersList(deviceConfiguration, deviceUID);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public string DeviceSetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID, string users)
        {
            try
            {
                return _iFiresecService.DeviceSetGuardUsersList(deviceConfiguration, deviceUID, users);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public string DeviceGetMDS5Data(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return _iFiresecService.DeviceGetMDS5Data(deviceConfiguration, deviceUID);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return null;
            }
        }

        public PlansConfiguration GetPlansConfiguration()
        {
            try
            {
                return _iFiresecService.GetPlansConfiguration();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            try
            {
                _iFiresecService.SetPlansConfiguration(plansConfiguration);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            try
            {
                return _iFiresecService.GetSystemConfiguration();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            try
            {
                _iFiresecService.SetSystemConfiguration(systemConfiguration);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public LibraryConfiguration GetLibraryConfiguration()
        {
            try
            {
                return _iFiresecService.GetLibraryConfiguration();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
        {
            try
            {
                _iFiresecService.SetLibraryConfiguration(libraryConfiguration);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            try
            {
                return _iFiresecService.GetSecurityConfiguration();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            try
            {
                _iFiresecService.SetSecurityConfiguration(securityConfiguration);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public DeviceConfigurationStates GetStates()
        {
            try
            {
                return _iFiresecService.GetStates();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            try
            {
                return _iFiresecService.ReadJournal(startIndex, count);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public IEnumerable<JournalRecord> GetFilteredJournal(JournalFilter journalFilter)
        {
            try
            {
                return _iFiresecService.GetFilteredJournal(journalFilter);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public IEnumerable<JournalRecord> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            try
            {
                return _iFiresecService.GetFilteredArchive(archiveFilter);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public IEnumerable<JournalRecord> GetDistinctRecords()
        {
            try
            {
                return _iFiresecService.GetDistinctRecords();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public DateTime GetArchiveStartDate()
        {
            try
            {
                return _iFiresecService.GetArchiveStartDate();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return DateTime.Now;
        }

        public void AddToIgnoreList(List<Guid> deviceUIDs)
        {
            try
            {
                _iFiresecService.AddToIgnoreList(deviceUIDs);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public void RemoveFromIgnoreList(List<Guid> deviceUIDs)
        {
            try
            {
                _iFiresecService.RemoveFromIgnoreList(deviceUIDs);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public void ResetStates(List<ResetItem> resetItems)
        {
            try
            {
                _iFiresecService.ResetStates(resetItems);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public void AddUserMessage(string message)
        {
            try
            {
                _iFiresecService.AddUserMessage(message);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public void AddJournalRecord(JournalRecord journalRecord)
        {
            try
            {
                _iFiresecService.AddJournalRecord(journalRecord);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public void ExecuteCommand(Guid deviceUID, string methodName)
        {
            try
            {
                _iFiresecService.ExecuteCommand(deviceUID, methodName);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public string CheckHaspPresence()
        {
            try
            {
                return _iFiresecService.CheckHaspPresence();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
                return "Нет связи с сервером Firesec-2";
            }
        }

        public List<string> GetFileNamesList(string directory)
        {
            try
            {
                return _iFiresecService.GetFileNamesList(directory);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            try
            {
                return _iFiresecService.GetDirectoryHash(directory);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public System.IO.Stream GetFile(string dirAndFileName)
        {
            try
            {
                return _iFiresecService.GetFile(dirAndFileName);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public void ConvertConfiguration()
        {
            try
            {
                _iFiresecService.ConvertConfiguration();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public void ConvertJournal()
        {
            try
            {
                _iFiresecService.ConvertJournal();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public string Ping()
        {
            try
            {
                var result = _iFiresecService.Ping();
                OnConnectionAppeared();

                return result;
            }
			//catch (CommunicationObjectFaultedException)
			//{
			//    OnConnectionLost();
			//}
			//catch (InvalidOperationException)
			//{
			//    OnConnectionLost();
			//}
			//catch (CommunicationException)
			//{
			//    OnConnectionLost();
			//}
            catch (Exception ex)
            {
                OnConnectionLost(ex);
            }
            return null;
        }

        public string Test()
        {
            try
            {
                return _iFiresecService.Test();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }

        public void SetXDeviceConfiguration(XFiresecAPI.XDeviceConfiguration xDeviceConfiguration)
        {
            try
            {
                _iFiresecService.SetXDeviceConfiguration(xDeviceConfiguration);
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
        }

        public XFiresecAPI.XDeviceConfiguration GetXDeviceConfiguration()
        {
            try
            {
                return _iFiresecService.GetXDeviceConfiguration();
            }
			catch (Exception ex)
            {
				OnConnectionLost(ex);
            }
            return null;
        }
    }
}