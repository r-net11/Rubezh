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

        public bool Connect(string userName, string passwordHash)
        {
            bool result = CheckLogin(userName, passwordHash);

            _currentCallback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
            CallbackManager.Add(_currentCallback);

            MainWindow.AddMessage("Connected. UserName = " + userName + ". SessionId = " + OperationContext.Current.SessionId);
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
            FiresecManager.SetNewConfig();
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

        public DeviceLibraryConfiguration GetDeviceLibraryConfiguration()
        {
            FiresecManager.DeviceLibraryConfiguration = new DeviceLibraryConfiguration();
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(DeviceLibraryConfiguration));
                using (var fileStream = new FileStream(DeviceLibraryConfigurationFileName, FileMode.Open))
                {
                    FiresecManager.DeviceLibraryConfiguration =
                        (DeviceLibraryConfiguration) dataContractSerializer.ReadObject(fileStream);
                }

                return FiresecManager.DeviceLibraryConfiguration;
            }
            catch
            {
                return FiresecManager.DeviceLibraryConfiguration;
            }
        }

        public PlansConfiguration GetPlansConfiguration()
        {
            FiresecManager.PlansConfiguration = new PlansConfiguration();
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
                using (var fileStream = new FileStream(PlansConfigurationFileName, FileMode.Open))
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

        public void SetDeviceLibraryConfiguration(DeviceLibraryConfiguration deviceLibraryConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(DeviceLibraryConfiguration));
            using (var fileStream = new FileStream(DeviceLibraryConfigurationFileName, FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, deviceLibraryConfiguration);
            }

            FiresecManager.DeviceLibraryConfiguration = deviceLibraryConfiguration;
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
            using (var fileStream = new FileStream(PlansConfigurationFileName, FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, plansConfiguration);
            }
        }

        public List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            var internalJournal = FiresecInternalClient.ReadEvents(startIndex, count);
            var journalRecords = new List<JournalRecord>();

            if (internalJournal != null && internalJournal.Journal != null && internalJournal.Journal.Length > 0)
            {
                foreach (var innerJournalRecord in internalJournal.Journal)
                {
                    journalRecords.Add(JournalConverter.Convert(innerJournalRecord));
                }
            }

            return journalRecords;
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
            string path = Directory.GetCurrentDirectory() + @"\" + directory;
            var fileNames = new List<string>();
            if (Directory.Exists(path))
            {
                foreach (var str in Directory.EnumerateFiles(path))
                {
                    fileNames.Add(Path.GetFileName(str));
                }
            }
            return fileNames;
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            return HashHelper.GetDirectoryHash(directory);
        }

        public Stream GetFile(string directoryNameAndFileName)
        {
            var filePath = Directory.GetCurrentDirectory() + @"\" + directoryNameAndFileName;
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