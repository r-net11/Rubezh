using FiresecAPI;
using System;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace FiresecClient
{
    public class SafeFiresecService : IFiresecService
    {
        public SafeFiresecService(IFiresecService iFiresecService)
        {
            _iFiresecService = iFiresecService;
        }

        IFiresecService _iFiresecService;

        public Action ConnectionLost;
        void OnConnectionLost()
        {
            if (ConnectionLost != null)
                ConnectionLost();
        }

        public Action ConnectionAppeared;
        void OnConnectionAppeared()
        {
            if (ConnectionAppeared != null)
                ConnectionAppeared();
        }

        public void Connect()
        {
            try
            {
                _iFiresecService.Connect();
            }
            catch
            {
                OnConnectionLost();
            }
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

        public List<string> GetSoundsFileName()
        {
            try
            {
                return _iFiresecService.GetSoundsFileName();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public Dictionary<string, string> GetHashAndNameSoundFiles()
        {
            try
            {
                return _iFiresecService.GetHashAndNameSoundFiles();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }

        public System.IO.Stream GetFile(string filepath)
        {
            try
            {
                return _iFiresecService.GetFile(filepath);
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
                return _iFiresecService.Ping();
            }
            catch
            {
                OnConnectionLost();
            }
            return null;
        }
    }
}
