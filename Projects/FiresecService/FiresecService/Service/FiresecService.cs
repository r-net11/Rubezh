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
using System.ServiceModel.Channels;

namespace FiresecService
{
    [ServiceBehavior(MaxItemsInObjectGraph = 2147483647, UseSynchronizationContext = true,
        InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode=ConcurrencyMode.Single)]
    public class FiresecService : IFiresecService, IDisposable
    {
        readonly static FiresecDbConverterDataContext DataBaseContext = new FiresecDbConverterDataContext();

        IFiresecCallback _currentCallback;
        string _userName;

        public static object locker = new object();

        public bool Connect(string userName, string password)
        {
            lock (locker)
            {
                bool result = CheckLogin(userName, password);
                if (result)
                {
                    MainWindow.AddMessage("Connected. UserName = " + userName + ". SessionId = " + OperationContext.Current.SessionId);
                    DatabaseHelper.AddInfoMessage(_userName, "Вход пользователя в систему");
                }

                return result;
            }
        }

        public bool Reconnect(string userName, string password)
        {
            var oldUser = GetUserFullName();
            
            var result = CheckLogin(userName, password);

            if (result)
            {
                var newUser = GetUserFullName();
                DatabaseHelper.AddInfoMessage(_userName, "Дежурство сдал");
                DatabaseHelper.AddInfoMessage(_userName, "Дежурство принял");
            }

            return result;
        }

        bool CheckLogin(string userName, string password)
        {
            var user = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Name == userName);
            if (user == null)
                return false;

            if (PasswordHashChecker.Check(password, user.PasswordHash) == false)
                return false;

            _userName = userName;
            return true;
        }

        [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.AfterCall)]
        public void Disconnect()
        {
            CallbackManager.Remove(_currentCallback);
            DatabaseHelper.AddInfoMessage(_userName, "Выход пользователя из системы");
        }

        public void Subscribe()
        {
            _currentCallback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
            CallbackManager.Add(_currentCallback);
        }

        public List<Driver> GetDrivers()
        {
            lock (locker)
            {
                return FiresecManager.Drivers;
            }
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            lock (locker)
            {
                return ConfigurationFileManager.GetDeviceConfiguration();
            }
        }

        public DeviceConfigurationStates GetStates()
        {
            lock (locker)
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

        public void WriteConfiguration(string deviceId)
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceId);
            FiresecInternalClient.DeviceWriteConfig(FiresecManager.CoreConfig, device.PlaceInTree);
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            lock (locker)
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
            lock (locker)
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
            lock (locker)
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
            lock (locker)
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
            lock (locker)
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
            lock (locker)
            {
                return DataBaseContext.JournalRecords.AsEnumerable().
                    Where(journal => journalFilter.CheckDaysConstraint(journal.SystemTime)).
                    Where(journal => JournalFilterHelper.FilterRecord(journalFilter, journal)).
                    Take(journalFilter.LastRecordsCount);
            }
        }

        public IEnumerable<JournalRecord> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            lock (locker)
            {
                return DataBaseContext.JournalRecords.AsEnumerable().
                    SkipWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.EndDate : journal.DeviceTime > archiveFilter.EndDate).
                    TakeWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.StartDate : journal.DeviceTime > archiveFilter.StartDate).
                Where(journal => archiveFilter.Descriptions.Any(description => description == journal.Description)).
                Where(journal => archiveFilter.Subsystems.Any(subsystem => subsystem == journal.SubsystemType));
            }
        }

        public IEnumerable<JournalRecord> GetDistinctRecords()
        {
            lock (locker)
            {
                return DataBaseContext.JournalRecords.AsEnumerable().
                    Select(x => x).Distinct(new JournalRecord());
            }
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
            journalRecord.User = _userName;
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
            lock (locker)
            {
                return HashHelper.GetFileNamesList(directory);
            }
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            lock (locker)
            {
                return HashHelper.GetDirectoryHash(directory);
            }
        }

        public Stream GetFile(string directoryNameAndFileName)
        {
            lock (locker)
            {
                var filePath = ConfigurationFileManager.ConfigurationDirectory(directoryNameAndFileName);
                return new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
        }

        public string Ping()
        {
            lock (locker)
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
            lock (locker)
            {


                DatabaseHelper.AddInfoMessage(GetUserFullName(), "Вход пользователя в систему");

                return "Test";
            }
        }

        string GetUserFullName()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            if (ip == "127.0.0.1")
                ip = "localhost";

            var user = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Name == _userName);

            string fullUserName = user.FullName + " (" + ip + ")";
            return fullUserName;
        }
    }
}