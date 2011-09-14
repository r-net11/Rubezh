using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Timers;
using FiresecAPI;
using FiresecAPI.Models;

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

        public void StartPing()
        {
            System.Timers.Timer pingTimer = new System.Timers.Timer();
            pingTimer.Elapsed += new ElapsedEventHandler(OnTimerPing);
            pingTimer.Interval = 1000;
            pingTimer.Enabled = true;
        }

        private void OnTimerPing(object source, ElapsedEventArgs e)
        {
            Ping();
        }

        public bool Connect(string userName, string password)
        {
            try
            {
                return _iFiresecService.Connect(userName, password);
            }
            catch
            {
                OnConnectionLost();
            }
            return false;
        }

        public bool Reconnect(string userName, string password)
        {
            try
            {
                return _iFiresecService.Reconnect(userName, password);
            }
            catch
            {
                OnConnectionLost();
            }
            return false;
        }

        public void Disconnect()
        {
            try
            {
                _iFiresecService.Disconnect();
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public void Subscribe()
        {
            try
            {
                _iFiresecService.Subscribe();
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public List<Driver> GetDrivers()
        {
            try
            {
                return _iFiresecService.GetDrivers();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            try
            {
                return _iFiresecService.GetDeviceConfiguration();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            try
            {
                _iFiresecService.SetDeviceConfiguration(deviceConfiguration);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public void WriteConfiguration(Guid deviceUID)
        {
            try
            {
                _iFiresecService.WriteConfiguration(deviceUID);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public void DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, string password)
        {
            try
            {
                _iFiresecService.DeviceSetPassword(deviceConfiguration, deviceUID, password);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public PlansConfiguration GetPlansConfiguration()
        {
            try
            {
                return _iFiresecService.GetPlansConfiguration();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            try
            {
                _iFiresecService.SetPlansConfiguration(plansConfiguration);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            try
            {
                return _iFiresecService.GetSystemConfiguration();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            try
            {
                _iFiresecService.SetSystemConfiguration(systemConfiguration);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public LibraryConfiguration GetLibraryConfiguration()
        {
            try
            {
                return _iFiresecService.GetLibraryConfiguration();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
        {
            try
            {
                _iFiresecService.SetLibraryConfiguration(libraryConfiguration);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            try
            {
                return _iFiresecService.GetSecurityConfiguration();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            try
            {
                _iFiresecService.SetSecurityConfiguration(securityConfiguration);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public DeviceConfigurationStates GetStates()
        {
            try
            {
                return _iFiresecService.GetStates();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            try
            {
                return _iFiresecService.ReadJournal(startIndex, count);
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public IEnumerable<JournalRecord> GetFilteredJournal(JournalFilter journalFilter)
        {
            try
            {
                return _iFiresecService.GetFilteredJournal(journalFilter);
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public IEnumerable<JournalRecord> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            try
            {
                return _iFiresecService.GetFilteredArchive(archiveFilter);
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public IEnumerable<JournalRecord> GetDistinctRecords()
        {
            try
            {
                return _iFiresecService.GetDistinctRecords();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public DateTime GetArchiveStartDate()
        {
            try
            {
                return _iFiresecService.GetArchiveStartDate();
            }
            catch
            {
                OnConnectionLost();
            }
            return DateTime.Now;
        }

        public void AddToIgnoreList(List<Guid> deviceUIDs)
        {
            try
            {
                _iFiresecService.AddToIgnoreList(deviceUIDs);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public void RemoveFromIgnoreList(List<Guid> deviceUIDs)
        {
            try
            {
                _iFiresecService.RemoveFromIgnoreList(deviceUIDs);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public void ResetStates(List<ResetItem> resetItems)
        {
            try
            {
                _iFiresecService.ResetStates(resetItems);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public void AddUserMessage(string message)
        {
            try
            {
                _iFiresecService.AddUserMessage(message);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public void AddJournalRecord(JournalRecord journalRecord)
        {
            try
            {
                _iFiresecService.AddJournalRecord(journalRecord);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public void ExecuteCommand(Guid deviceUID, string methodName)
        {
            try
            {
                _iFiresecService.ExecuteCommand(deviceUID, methodName);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public List<string> GetFileNamesList(string directory)
        {
            try
            {
                return _iFiresecService.GetFileNamesList(directory);
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            try
            {
                return _iFiresecService.GetDirectoryHash(directory);
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public System.IO.Stream GetFile(string dirAndFileName)
        {
            try
            {
                return _iFiresecService.GetFile(dirAndFileName);
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
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

        public string Test()
        {
            try
            {
                return _iFiresecService.Test();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }
    }
}