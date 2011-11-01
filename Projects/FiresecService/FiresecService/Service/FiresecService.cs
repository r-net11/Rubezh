using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public IFiresecCallback Callback { get; private set; }
        string _userLogin;
        string _userName;
        string _userIpAddress;

        public static readonly object Locker = new object();

        public string Connect(string login, string password)
        {
            lock (Locker)
            {
                if (CheckLogin(login, password))
                {
                    if (CheckRemoteAccessPermissions(login))
                    {
                        MainWindow.AddMessage("Connected. UserName = " + login + ". SessionId = " + OperationContext.Current.SessionId);
                        DatabaseHelper.AddInfoMessage(_userName, "Вход пользователя в систему");

                        Callback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
                        CallbackManager.Add(this);

                        return null;
                    }
                    return "У пользователя " + login + " нет прав на подкючение к удаленному серверу c хоста: " + _userIpAddress;
                }
                return "Неверный логин или пароль";
            }
        }

        bool CheckRemoteAccessPermissions(string login)
        {
            return true;
            var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            _userIpAddress = endpoint.Address;

            if (CheckHostIps("localhost"))
                return true;

            var remoteAccessPermissions = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login).RemoreAccess;
            if (remoteAccessPermissions == null)
                return false;

            switch (remoteAccessPermissions.RemoteAccessType)
            {
                case RemoteAccessType.RemoteAccessBanned:
                    return false;

                case RemoteAccessType.RemoteAccessAllowed:
                    return true;

                case RemoteAccessType.SelectivelyAllowed:
                    foreach (var hostNameOrIpAddress in remoteAccessPermissions.HostNameOrAddressList)
                    {
                        if (CheckHostIps(hostNameOrIpAddress))
                            return true;
                    }
                    break;
            }
            return false;
        }

        bool CheckHostIps(string hostNameOrIpAddress)
        {
            try
            {
                var addressList = Dns.GetHostEntry(hostNameOrIpAddress).AddressList;
                return addressList.Any(ip => ip.ToString() == _userIpAddress);
            }
            catch
            {
                return false;
            }
        }

        public string Reconnect(string login, string password)
        {
            var oldUserFileName = _userName;
            if (CheckLogin(login, password))
            {
                DatabaseHelper.AddInfoMessage(oldUserFileName, "Дежурство сдал");
                DatabaseHelper.AddInfoMessage(_userName, "Дежурство принял");

                return null;
            }
            return "Неверный логин или пароль";
        }

        bool CheckLogin(string login, string password)
        {
            var user = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login);
            if (user == null || !HashHelper.CheckPass(password, user.PasswordHash))
                return false;

            _userLogin = login;
            SetUserFullName();

            return true;
        }

        [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.AfterCall)]
        public void Disconnect()
        {
            DatabaseHelper.AddInfoMessage(_userName, "Выход пользователя из системы");
            CallbackManager.Remove(this);
        }

        public bool IsSubscribed { get; set; }

        public void Subscribe()
        {
            IsSubscribed = true;
        }

        public List<Driver> GetDrivers()
        {
            lock (Locker)
            {
                return FiresecManager.Drivers;
            }
        }

        public DeviceConfigurationStates GetStates()
        {
            lock (Locker)
            {
                return FiresecManager.DeviceConfigurationStates;
            }
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            lock (Locker)
            {
                return FiresecManager.DeviceConfiguration;
            }
        }

        public void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            ConfigurationFileManager.SetDeviceConfiguration(deviceConfiguration);
            FiresecManager.DeviceConfiguration = deviceConfiguration;

            ConfigurationConverter.ConvertBack(deviceConfiguration, true);
            //FiresecInternalClient.SetNewConfig(ConfigurationConverter.FiresecConfiguration);
        }

        public void DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceWriteConfig(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public void DeviceWriteAllConfiguration(DeviceConfiguration deviceConfiguration)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            foreach (var device in deviceConfiguration.Devices)
            {
                if (device.Driver.CanWriteDatabase)
                {
                    FiresecInternalClient.DeviceWriteConfig(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
                }
            }
        }

        public void DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceSetPassword(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, password, (int) devicePasswordType);
        }

        public void DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceDatetimeSync(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public void DeviceRestart(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceRestart(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public string DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceGetInformation(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public List<string> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            string serials = FiresecInternalClient.DeviceGetSerialList(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
            return serials.Split(';').ToList();
        }

        public string DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
        {
            lock (Locker)
            {
                fileName = Guid.NewGuid().ToString();
                Directory.CreateDirectory("Temp");
                fileName = Directory.GetCurrentDirectory() + "\\Temp\\" + fileName;
                using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                fileName = "D://XHC//sborka2_23rc35.HXC";

                ConfigurationConverter.ConvertBack(deviceConfiguration, false);
                var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                return FiresecInternalClient.DeviceUpdateFirmware(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, fileName);
            }
        }

        public string DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
        {
            lock (Locker)
            {
                fileName = Guid.NewGuid().ToString();
                Directory.CreateDirectory("Temp");
                fileName = Directory.GetCurrentDirectory() + "\\Temp\\" + fileName;
                using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                fileName = "D://XHC//sborka2_23rc35.HXC";

                ConfigurationConverter.ConvertBack(deviceConfiguration, false);
                var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                return FiresecInternalClient.DeviceVerifyFirmwareVersion(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, fileName);
            }
        }

        public string DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceReadEventLog(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public DeviceConfiguration DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            var config = FiresecInternalClient.DeviceAutoDetectChildren(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, fastSearch);
            if (config == null)
                return null;

            ConfigurationConverter.DeviceConfiguration = new DeviceConfiguration();
            ConfigurationConverter.FiresecConfiguration = config;
            DeviceConverter.Convert();
            return ConfigurationConverter.DeviceConfiguration;
        }

        public DeviceConfiguration DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            var config = FiresecInternalClient.DeviceReadConfig(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
            if (config == null)
                return null;

            ConfigurationConverter.DeviceConfiguration = new DeviceConfiguration();
            ConfigurationConverter.FiresecConfiguration = config;
            DeviceConverter.Convert();
            return ConfigurationConverter.DeviceConfiguration;
        }

        public List<DeviceCustomFunction> DeviceCustomFunctionList(Guid driverUID)
        {
            var functions = FiresecInternalClient.DeviceCustomFunctionList(driverUID.ToString().ToUpper());
            return DeviceCustomFunctionConverter.Convert(functions);
        }

        public string DeviceCustomFunctionExecute(DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceCustomFunctionExecute(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, functionName);
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            lock (Locker)
            {
                return FiresecManager.SecurityConfiguration;
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
                return FiresecManager.SystemConfiguration;
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
                return FiresecManager.LibraryConfiguration;
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
                return FiresecManager.PlansConfiguration;
            }
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
            FiresecManager.PlansConfiguration = plansConfiguration;
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
            var filterHelper = new ArchiveFilterHelper(archiveFilter);

            return DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                SkipWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.EndDate : journal.DeviceTime > archiveFilter.EndDate).
                TakeWhile(journal => archiveFilter.UseSystemDate ? journal.SystemTime > archiveFilter.StartDate : journal.DeviceTime > archiveFilter.StartDate).
                Where(journal => filterHelper.FilterByEvents(journal)).
                Where(journal => filterHelper.FilterBySubsystems(journal)).
                Where(journal => filterHelper.FilterByDevices(journal));
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
                DatabaseHelper.AddInfoMessage(_userName, "Вход пользователя в систему");

                return "Test";
            }
        }

        public void StopProgress()
        {
            MustStopProgress = true;
        }

        public bool MustStopProgress;

        void SetUserFullName()
        {
            var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string userIp = endpoint.Address;

            var addressList = Dns.GetHostEntry("localhost").AddressList;
            if (addressList.Any(ip => ip.ToString() == userIp))
                userIp = "localhost";

            var user = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == _userLogin);
            _userName = user.Name + " (" + userIp + ")";
        }
    }
}