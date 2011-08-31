using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Converters;
using FiresecService.DatabaseConverter;
using FiresecServiceRunner;

namespace FiresecService
{
    [ServiceBehavior(MaxItemsInObjectGraph = 2147483647, UseSynchronizationContext = true, InstanceContextMode = InstanceContextMode.PerSession)]
    public class FiresecService : IFiresecService, IDisposable
    {
        readonly static string SystemConfigurationFileName = "SystemConfiguration.xml";
        readonly static string DeviceLibraryConfigurationFileName = "DeviceLibrary.xml";
        readonly static string PlansConfigurationFileName = "PlansConfiguration.xml";
        IFiresecCallback _currentCallback;
        string _userName;

        string ConfigurationDirectory(string FileNameOrDirectory)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Configuration", FileNameOrDirectory);
        }

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
            return FiresecManager.DeviceConfiguration;
        }

        public DeviceConfigurationStates GetStates()
        {
            return FiresecManager.DeviceConfigurationStates;
        }

        public void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
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
            return FiresecManager.SecurityConfiguration;
        }

        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            FiresecManager.SecurityConfiguration = securityConfiguration;
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            FiresecManager.SystemConfiguration = new SystemConfiguration();
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
                using (var fileStream = new FileStream(SystemConfigurationFileName, FileMode.Open))
                {
                    FiresecManager.SystemConfiguration = (SystemConfiguration) dataContractSerializer.ReadObject(fileStream);
                }

                return FiresecManager.SystemConfiguration;
            }
            catch
            {
                return FiresecManager.SystemConfiguration;
            }
        }

        public LibraryConfiguration GetLibraryConfiguration()
        {
            FiresecManager.LibraryConfiguration = new LibraryConfiguration();
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(LibraryConfiguration));
                using (var fileStream = new FileStream(ConfigurationDirectory(DeviceLibraryConfigurationFileName), FileMode.Open))
                {
                    FiresecManager.LibraryConfiguration = (LibraryConfiguration) dataContractSerializer.ReadObject(fileStream);
                }

                return FiresecManager.LibraryConfiguration;
            }
            catch
            {
                return FiresecManager.LibraryConfiguration;
            }
        }

        public PlansConfiguration GetPlansConfiguration()
        {
            //var plans = FiresecInternalClient.GetPlans();
            //var plansConfiguration = PlansConverter.Convert(plans);
            //return plansConfiguration;

            FiresecManager.PlansConfiguration = new PlansConfiguration();
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
                using (var fileStream = new FileStream(ConfigurationDirectory(PlansConfigurationFileName), FileMode.Open))
                {
                    FiresecManager.PlansConfiguration = (PlansConfiguration) dataContractSerializer.ReadObject(fileStream);
                }

                return FiresecManager.PlansConfiguration;
            }
            catch
            {
                return FiresecManager.PlansConfiguration;
            }
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
            using (var fileStream = new FileStream(SystemConfigurationFileName, FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, systemConfiguration);
            }

            FiresecManager.SystemConfiguration = systemConfiguration;
        }

        public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(LibraryConfiguration));
            using (var fileStream = new FileStream(ConfigurationDirectory(DeviceLibraryConfigurationFileName), FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, libraryConfiguration);
            }

            FiresecManager.LibraryConfiguration = libraryConfiguration;
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
            using (var fileStream = new FileStream(ConfigurationDirectory(PlansConfigurationFileName), FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, plansConfiguration);
            }
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

        public List<JournalRecord> GetFilteredJournal(JournalFilter journalFilter)
        {
            var filteredJournal = new List<JournalRecord>();
            using (var dataContext = new FiresecDbConverterDataContext())
            {
                filteredJournal = dataContext.Journal.Cast<JournalRecord>().ToList();
            }

            return filteredJournal;
        }

        public void AddToIgnoreList(List<string> deviceIds)
        {
            var devicePaths = new List<string>();
            foreach (var id in deviceIds)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == id);
                devicePaths.Add(device.PlaceInTree);
            }

            FiresecInternalClient.AddToIgnoreList(devicePaths);
        }

        public void RemoveFromIgnoreList(List<string> deviceIds)
        {
            var devicePaths = new List<string>();
            foreach (var id in deviceIds)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == id);
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

        public void ExecuteCommand(string deviceId, string methodName)
        {
            var device = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId);
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
            var filePath = ConfigurationDirectory(directoryNameAndFileName);
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