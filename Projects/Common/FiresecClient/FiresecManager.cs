using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FiresecClient
{
    public class FiresecManager
    {
        public static List<Driver> Drivers { get; set; }
        public static DeviceConfiguration DeviceConfiguration { get; set; }
        public static DeviceConfigurationStates DeviceStates { get; set; }
        public static LibraryConfiguration LibraryConfiguration { get; set; }
        public static SystemConfiguration SystemConfiguration { get; set; }
        public static PlansConfiguration PlansConfiguration { get; set; }
        public static SecurityConfiguration SecurityConfiguration { get; set; }

        static SafeFiresecService _firesecService;

        public static bool Connect(string login, string password)
        {
            _firesecService = new SafeFiresecService(FiresecServiceFactory.Create());
            if (_firesecService.Connect(login, password))
            {
                FileHelper.Synchronize();
                Drivers = _firesecService.GetDrivers();
                SystemConfiguration = _firesecService.GetSystemConfiguration();
                LibraryConfiguration = _firesecService.GetLibraryConfiguration();
                PlansConfiguration = _firesecService.GetPlansConfiguration();
                SecurityConfiguration = _firesecService.GetSecurityConfiguration();
                if (SecurityConfiguration.UserRoles == null) SecurityConfiguration.UserRoles = new List<UserRole>();
                DeviceConfiguration = _firesecService.GetDeviceConfiguration();
                DeviceStates = _firesecService.GetStates();

                UpdateDrivers();
                UpdateConfiguration();
                UpdateStates();

                _firesecService.Subscribe();
                _userLogin = login;
                _firesecService.StartPing();

                return true;
            }

            return false;
        }

        public static bool Reconnect(string login, string password)
        {
            if (_firesecService.Reconnect(login, password))
            {
                _userLogin = login;
                return true;
            }
            return false;
        }

        public static void UpdateDrivers()
        {
            foreach (var driver in Drivers)
            {
                driver.ImageSource = FileHelper.GetIconFilePath(driver.ImageSource) + ".ico";
            }
        }

        public static void UpdateConfiguration()
        {
            DeviceConfiguration.Update();

            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);

                if (device.Driver.IsIndicatorDevice || device.IndicatorLogic != null)
                {
                    var indicatorDevice = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.IndicatorLogic.DeviceUID);
                    device.IndicatorLogic.Device = indicatorDevice;
                }

                if (device.Driver.IsZoneLogicDevice)
                {
                    foreach (var clause in device.ZoneLogic.Clauses)
                    {
                        if (clause.DeviceUID != Guid.Empty)
                        {
                            clause.Device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.DeviceUID);
                        }
                    }
                }
            }
        }

        public static void UpdateStates()
        {
            foreach (var deviceState in DeviceStates.DeviceStates)
            {
                deviceState.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceState.UID);

                foreach (var state in deviceState.States)
                {
                    state.DriverState = deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == state.Code);
                }

                foreach (var parentState in deviceState.ParentStates)
                {
                    parentState.ParentDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == parentState.ParentDeviceId);
                    parentState.DriverState = parentState.ParentDevice.Driver.States.FirstOrDefault(x => x.Code == parentState.Code);
                }
            }
        }

        static string _userLogin;
        public static User CurrentUser
        {
            get { return SecurityConfiguration.Users.FirstOrDefault(x => x.Login == _userLogin); }
        }

        public static void Disconnect()
        {
            _firesecService.StopPing();
            _firesecService.Disconnect();
            FiresecServiceFactory.Dispose();
        }

        public static void SetConfiguration()
        {
            _firesecService.SetSystemConfiguration(SystemConfiguration);
            _firesecService.SetSecurityConfiguration(SecurityConfiguration);
            _firesecService.SetLibraryConfiguration(LibraryConfiguration);
            _firesecService.SetDeviceConfiguration(DeviceConfiguration);
            _firesecService.SetPlansConfiguration(PlansConfiguration);
        }

        public static void AddToIgnoreList(List<Guid> deviceUIDs)
        {
            _firesecService.AddToIgnoreList(deviceUIDs);
        }

        public static void RemoveFromIgnoreList(List<Guid> deviceUIDs)
        {
            _firesecService.RemoveFromIgnoreList(deviceUIDs);
        }

        public static void ResetStates(List<ResetItem> resetItems)
        {
            _firesecService.ResetStates(resetItems);
        }

        public static void AddUserMessage(string message)
        {
            _firesecService.AddUserMessage(message);
        }

        public static void AddJournalRecord(JournalRecord journalRecord)
        {
            _firesecService.AddJournalRecord(journalRecord);
        }

        public static void ExecuteCommand(Guid deviceUID, string methodName)
        {
            _firesecService.ExecuteCommand(deviceUID, methodName);
        }

        public static List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            return _firesecService.ReadJournal(startIndex, count);
        }

        public static IEnumerable<JournalRecord> GetFilteredJournal(JournalFilter journalFilter)
        {
            return _firesecService.GetFilteredJournal(journalFilter);
        }

        public static IEnumerable<JournalRecord> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            return _firesecService.GetFilteredArchive(archiveFilter);
        }

        public static IEnumerable<JournalRecord> GetDistinctRecords()
        {
            return _firesecService.GetDistinctRecords();
        }

        public static DateTime GetArchiveStartDate()
        {
            return _firesecService.GetArchiveStartDate();
        }

        public static List<string> GetFileNamesList(string directory)
        {
            return _firesecService.GetFileNamesList(directory);
        }

        public static Dictionary<string, string> GetDirectoryHash(string directory)
        {
            return _firesecService.GetDirectoryHash(directory);
        }

        public static Stream GetFile(string filepath)
        {
            return _firesecService.GetFile(filepath);
        }

        public static DeviceConfiguration AutoDetectDevice(Guid deviceUID, bool fastSearch)
        {
            return _firesecService.DeviceAutoDetectChildren(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID, fastSearch);
        }

        public static DeviceConfiguration DeviceReadConfiguration(Guid deviceUID)
        {
            return _firesecService.DeviceReadConfiguration(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID);
        }

        public static void DeviceWriteConfiguration(Guid deviceUID)
        {
            _firesecService.DeviceWriteConfiguration(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID);
        }

        public static void WriteAllDeviceConfiguration()
        {
            _firesecService.DeviceWriteAllConfiguration(DeviceConfiguration);
        }

        public static string ReadDeviceJournal(Guid deviceUID)
        {
            return _firesecService.DeviceReadEventLog(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID);
        }

        public static void SynchronizeDevice(Guid deviceUID)
        {
            _firesecService.DeviceDatetimeSync(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID);
        }

        public static void DeviceRestart(Guid deviceUID)
        {
            _firesecService.DeviceRestart(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID);
        }

        public static string DeviceUpdateFirmware(Guid deviceUID, string content)
        {
            return _firesecService.DeviceUpdateFirmware(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID, content);
        }

        public static string DeviceVerifyFirmwareVersion(Guid deviceUID, string content)
        {
            return _firesecService.DeviceVerifyFirmwareVersion(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID, content);
        }

        public static string DeviceGetInformation(Guid deviceUID)
        {
            return _firesecService.DeviceGetInformation(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID);
        }

        public static string DeviceGetSerialList(Guid deviceUID)
        {
            return _firesecService.DeviceGetSerialList(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID);
        }

        public static void SetPassword(Guid deviceUID, DevicePasswordType devicePasswordType, string password)
        {
            _firesecService.DeviceSetPassword(DeviceConfiguration.CopyOneBranch(deviceUID), deviceUID, devicePasswordType, password);
        }

        public static void LoadFromFile(string fileName)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                FiresecManager.DeviceConfiguration = (DeviceConfiguration) dataContractSerializer.ReadObject(fileStream);
            }

            UpdateConfiguration();
        }

        public static void SaveToFile(string fileName)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, FiresecManager.DeviceConfiguration);
            }
        }

        public static void Test()
        {
            _firesecService.Test();
        }

        public static void StopProgress()
        {
            _firesecService.StopProgress();
        }
    }
}