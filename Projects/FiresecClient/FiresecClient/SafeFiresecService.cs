using System;
using System.Collections.Generic;
using System.ServiceModel;
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

        public bool Connect(string userName, string passwordHash)
        {
            try
            {
                return _iFiresecService.Connect(userName, passwordHash);
            }
            catch
            {
                OnConnectionLost();
            }
            return false;
        }

        public bool Reconnect(string userName, string passwordHash)
        {
            try
            {
                return _iFiresecService.Reconnect(userName, passwordHash);
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

        public void WriteConfiguration(string deviceId)
        {
            try
            {
                _iFiresecService.WriteConfiguration(deviceId);
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

        public void AddToIgnoreList(List<string> deviceIds)
        {
            try
            {
                _iFiresecService.AddToIgnoreList(deviceIds);
            }
            catch
            {
                OnConnectionLost();
            }
        }

        public void RemoveFromIgnoreList(List<string> deviceIds)
        {
            try
            {
                _iFiresecService.RemoveFromIgnoreList(deviceIds);
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

        public void ExecuteCommand(string deviceId, string methodName)
        {
            try
            {
                _iFiresecService.ExecuteCommand(deviceId, methodName);
            }
            catch
            {
                OnConnectionLost();
            }
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