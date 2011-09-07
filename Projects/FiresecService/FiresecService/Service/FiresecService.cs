using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Converters;
using FiresecService.DatabaseConverter;
using FiresecService.Processor;
using FiresecServiceRunner;

namespace FiresecService
{
    [ServiceBehavior(MaxItemsInObjectGraph = 2147483647, UseSynchronizationContext = true, InstanceContextMode = InstanceContextMode.PerSession)]
    public class FiresecService : IFiresecService, IDisposable
    {
        readonly static FiresecDbConverterDataContext DataBaseContext = new FiresecDbConverterDataContext();

        IFiresecCallback _currentCallback;
        string _userName;

        public bool Connect(string userName, string passwordHash)
        {
            bool result = CheckLogin(userName, passwordHash);
            if (result)
            {
                _currentCallback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
                CallbackManager.Add(_currentCallback);
                MainWindow.AddMessage("Connected. UserName = " + userName + ". SessionId = " + OperationContext.Current.SessionId);
            }

            return result;
        }

        public bool Reconnect(string userName, string passwordHash)
        {
            return CheckLogin(userName, passwordHash);
        }

        bool CheckLogin(string userName, string passwordHash)
        {
            _userName = userName;
            return true;
        }

        [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.AfterCall)]
        public void Disconnect()
        {
            CallbackManager.Remove(_currentCallback);
        }

        public List<Driver> GetDrivers()
        {
            return FiresecManager.Drivers;
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            return ConfigurationFileManager.GetDeviceConfiguration();
        }

        public DeviceConfigurationStates GetStates()
        {
            return FiresecManager.DeviceConfigurationStates;
        }

        public void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            ConfigurationFileManager.SetDeviceConfiguration(deviceConfiguration);
            FiresecManager.DeviceConfiguration = deviceConfiguration;
            //FiresecManager.SetNewConfig();
        }

        public void WriteConfiguration(string deviceId)
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceId);
            FiresecInternalClient.DeviceWriteConfig(FiresecManager.CoreConfig, device.PlaceInTree);
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            return ConfigurationFileManager.GetSecurityConfiguration();
        }

        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            ConfigurationFileManager.SetSecurityConfiguration(securityConfiguration);
            FiresecManager.SecurityConfiguration = securityConfiguration;
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            return ConfigurationFileManager.GetSystemConfiguration();
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            ConfigurationFileManager.SetSystemConfiguration(systemConfiguration);
            FiresecManager.SystemConfiguration = systemConfiguration;
        }

        public LibraryConfiguration GetLibraryConfiguration()
        {
            return ConfigurationFileManager.GetLibraryConfiguration();
        }

        public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
        {
            ConfigurationFileManager.SetLibraryConfiguration(libraryConfiguration);
            FiresecManager.LibraryConfiguration = libraryConfiguration;
        }

        public PlansConfiguration GetPlansConfiguration()
        {
            return ConfigurationFileManager.GetPlansConfiguration();
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
        }

        public List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            var internalJournal = FiresecInternalClient.ReadEvents(startIndex, count);
            var journalRecords = new List<JournalRecord>();
            if (internalJournal != null && internalJournal.Journal.IsNotNullOrEmpty())
            {
                foreach (var innerJournalRecord in internalJournal.Journal)
                {
                    journalRecords.Add(JournalConverter.Convert(innerJournalRecord));
                }
            }

            return journalRecords;
        }

        public IEnumerable<JournalRecord> GetFilteredJournal(JournalFilter journalFilter)
        {
            return DataBaseContext.JournalRecords.AsEnumerable().
                Where(journal => journalFilter.CheckDaysConstraint(journal.SystemTime)).
                Where(journal => JournalFilterHelper.FilterRecord(journalFilter, journal)).
                Take(journalFilter.LastRecordsCount);
        }

        public IEnumerable<JournalRecord> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            return DataBaseContext.JournalRecords.AsEnumerable().
                SkipWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.EndDate : journal.DeviceTime > archiveFilter.EndDate).
                TakeWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.StartDate : journal.DeviceTime > archiveFilter.StartDate).
                Where(journal => archiveFilter.Descriptions.Any(description => description == journal.Description));
        }

        public IEnumerable<JournalRecord> GetDistinctRecords()
        {
            return DataBaseContext.JournalRecords.AsEnumerable().
                Select(x => x).Distinct(new JournalRecord());
        }

        public void AddToIgnoreList(List<Guid> deviceGuids)
        {
            var devicePaths = new List<string>();
            foreach (var guid in deviceGuids)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == guid);
                devicePaths.Add(device.PlaceInTree);
            }

            FiresecInternalClient.AddToIgnoreList(devicePaths);
        }

        public void RemoveFromIgnoreList(List<Guid> deviceGuids)
        {
            var devicePaths = new List<string>();
            foreach (var guid in deviceGuids)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == guid);
                devicePaths.Add(device.PlaceInTree);
            }

            FiresecInternalClient.RemoveFromIgnoreList(devicePaths);
        }

        public void ResetStates(List<ResetItem> resetItems)
        {
            FiresecResetHelper.ResetMany(resetItems);
        }

        public void AddUserMessage(string message)
        {
            FiresecInternalClient.AddUserMessage(message);
        }

        public void AddJournalRecord(JournalRecord journalRecord)
        {
            DatabaseHelper.AddJournalRecord(journalRecord);
        }

        public void ExecuteCommand(Guid deviceUID, string methodName)
        {
            var device = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
            if (device != null)
            {
                FiresecInternalClient.ExecuteCommand(device.PlaceInTree, methodName);
            }
        }

        public List<string> GetFileNamesList(string directory)
        {
            return HashHelper.GetFileNamesList(directory);
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            return HashHelper.GetDirectoryHash(directory);
        }

        public Stream GetFile(string directoryNameAndFileName)
        {
            var filePath = ConfigurationFileManager.ConfigurationDirectory(directoryNameAndFileName);
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public string Ping()
        {
            return "Pong";
        }

        public void Dispose()
        {
            Disconnect();
        }

        public string Test()
        {
            //FiresecException fault = new FiresecException();
            //throw new FaultException<FiresecException>(fault, new FaultReason("Test"));
            return "Test";
        }
    }
}