using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Converters;
using FiresecService.DatabaseConverter;
using FiresecService.Processor;
using FiresecServiceRunner;

namespace FiresecService
{
    [ServiceBehavior(MaxItemsInObjectGraph = 2147483647, UseSynchronizationContext = true,
        InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FiresecService : IFiresecService, IDisposable
    {
        public readonly static FiresecDbConverterDataContext DataBaseContext = new FiresecDbConverterDataContext();

        IFiresecCallback _currentCallback;
        string _userName;
        string _userFullName;

        public static readonly object Locker = new object();

        public bool Connect(string userName, string password)
        {
            lock (Locker)
            {
                if (CheckLogin(userName, password))
                {
                    MainWindow.AddMessage("Connected. UserName = " + userName + ". SessionId = " + OperationContext.Current.SessionId);
                    DatabaseHelper.AddInfoMessage(_userFullName, "Вход пользователя в систему");

                    return true;
                }
                return false;
            }
        }

        public bool Reconnect(string userName, string password)
        {
            var oldUserFileName = _userFullName;
            if (CheckLogin(userName, password))
            {
                DatabaseHelper.AddInfoMessage(oldUserFileName, "Дежурство сдал");
                DatabaseHelper.AddInfoMessage(_userFullName, "Дежурство принял");

                return true;
            }
            return false;
        }

        bool CheckLogin(string userName, string password)
        {
            var user = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Name == userName);
            if (user == null || !HashHelper.CheckPass(password, user.PasswordHash))
                return false;

            _userName = userName;
            SetUserFullName();

            return true;
        }

        [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.AfterCall)]
        public void Disconnect()
        {
            DatabaseHelper.AddInfoMessage(_userFullName, "Выход пользователя из системы");
            CallbackManager.Remove(_currentCallback);
        }

        public void Subscribe()
        {
            _currentCallback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
            CallbackManager.Add(_currentCallback);
        }

        public List<Driver> GetDrivers()
        {
            lock (Locker)
            {
                return FiresecManager.Drivers;
            }
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            lock (Locker)
            {
                return ConfigurationFileManager.GetDeviceConfiguration();
            }
        }

        public DeviceConfigurationStates GetStates()
        {
            lock (Locker)
            {
                return FiresecManager.DeviceConfigurationStates;
            }
        }

        public void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            ConfigurationFileManager.SetDeviceConfiguration(deviceConfiguration);
            FiresecManager.DeviceConfiguration = deviceConfiguration;
            //FiresecManager.SetNewConfig();
        }

        public void WriteConfiguration(Guid deviceUID)
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceWriteConfig(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public void DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceSetPassword(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, password, (int) devicePasswordType);
        }

        public void DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceDatetimeSync(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public string DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceGetInformation(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public string DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceReadEventLog(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public DeviceConfiguration DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            var config = FiresecInternalClient.DeviceAutoDetectChildren(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, fastSearch);
            if (config == null)
                return null;

            ConfigurationConverter.DeviceConfiguration = new DeviceConfiguration();
            ConfigurationConverter.FiresecConfiguration = config;
            DeviceConverter.Convert();
            return ConfigurationConverter.DeviceConfiguration;
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            lock (Locker)
            {
                return ConfigurationFileManager.GetSecurityConfiguration();
            }
        }

        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            ConfigurationFileManager.SetSecurityConfiguration(securityConfiguration);
            FiresecManager.SecurityConfiguration = securityConfiguration;
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            lock (Locker)
            {
                return ConfigurationFileManager.GetSystemConfiguration();
            }
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            ConfigurationFileManager.SetSystemConfiguration(systemConfiguration);
            FiresecManager.SystemConfiguration = systemConfiguration;
        }

        public LibraryConfiguration GetLibraryConfiguration()
        {
            lock (Locker)
            {
                return ConfigurationFileManager.GetLibraryConfiguration();
            }
        }

        public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
        {
            ConfigurationFileManager.SetLibraryConfiguration(libraryConfiguration);
            FiresecManager.LibraryConfiguration = libraryConfiguration;
        }

        public PlansConfiguration GetPlansConfiguration()
        {
            lock (Locker)
            {
                return ConfigurationFileManager.GetPlansConfiguration();
            }
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
        }

        public List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            lock (Locker)
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
        }

        public IEnumerable<JournalRecord> GetFilteredJournal(JournalFilter journalFilter)
        {
            return DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                Where(journal => journalFilter.CheckDaysConstraint(journal.SystemTime)).
                Where(journal => JournalFilterHelper.FilterRecord(journalFilter, journal)).
                Take(journalFilter.LastRecordsCount);
        }

        public IEnumerable<JournalRecord> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            if (archiveFilter.Descriptions.IsNotNullOrEmpty())
            {
                if (archiveFilter.Subsystems.IsNotNullOrEmpty())
                {
                    return DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                        SkipWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.EndDate : journal.DeviceTime > archiveFilter.EndDate).
                        TakeWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.StartDate : journal.DeviceTime > archiveFilter.StartDate).
                        Where(journal => archiveFilter.Descriptions.Any(description => description == journal.Description)).
                        Where(journal => archiveFilter.Subsystems.Any(subsystem => subsystem == journal.SubsystemType));
                }
                return DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                    SkipWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.EndDate : journal.DeviceTime > archiveFilter.EndDate).
                    TakeWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.StartDate : journal.DeviceTime > archiveFilter.StartDate).
                    Where(journal => archiveFilter.Descriptions.Any(description => description == journal.Description));
            }
            return DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                SkipWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.EndDate : journal.DeviceTime > archiveFilter.EndDate).
                TakeWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.StartDate : journal.DeviceTime > archiveFilter.StartDate);
        }

        public IEnumerable<JournalRecord> GetDistinctRecords()
        {
            return DataBaseContext.JournalRecords.AsEnumerable().
                Select(x => x).Distinct(new JournalRecord());
        }

        public DateTime GetArchiveStartDate()
        {
            return DataBaseContext.JournalRecords.AsEnumerable().First().SystemTime;
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
            journalRecord.User = _userFullName;
            DatabaseHelper.AddJournalRecord(journalRecord);
            CallbackManager.OnNewJournalRecord(journalRecord);
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
            lock (Locker)
            {
                return HashHelper.GetFileNamesList(directory);
            }
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            lock (Locker)
            {
                return HashHelper.GetDirectoryHash(directory);
            }
        }

        public Stream GetFile(string directoryNameAndFileName)
        {
            lock (Locker)
            {
                var filePath = ConfigurationFileManager.ConfigurationDirectory(directoryNameAndFileName);
                return new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
        }

        public string Ping()
        {
            lock (Locker)
            {
                return "Pong";
            }
        }

        public void Dispose()
        {
            Disconnect();
        }

        public string Test()
        {
            lock (Locker)
            {
                DatabaseHelper.AddInfoMessage(_userFullName, "Вход пользователя в систему");

                return "Test";
            }
        }

        public void StopProgress()
        {
            MustStopProgress = true;
        }

        public static bool MustStopProgress;

        void SetUserFullName()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            if (ip == "127.0.0.1")
                ip = "localhost";

            var user = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Name == _userName);
            _userFullName = user.FullName + " (" + ip + ")";
        }
    }
}