using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.IO;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Converters;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Cryptography;
using System;

namespace FiresecService
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, MaxItemsInObjectGraph = 2147483647)]
    public class FiresecService : IFiresecService
    {
        IFiresecCallback _currentCallback;

        public void Connect()
        {
            _currentCallback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
            CallbackManager.Add(_currentCallback);
        }

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
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
                FileStream fileStream = new FileStream("H:/SystemConfiguration.xml", FileMode.Open);
                FiresecManager.SystemConfiguration = (SystemConfiguration)dataContractSerializer.ReadObject(fileStream);
                fileStream.Close();

                return FiresecManager.SystemConfiguration;
            }
            catch
            {
                return FiresecManager.SystemConfiguration;
            }
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            FiresecManager.SystemConfiguration = systemConfiguration;

            DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
            FileStream fileStream = new FileStream("H:/SystemConfiguration.xml", FileMode.Create);
            dataContractSerializer.WriteObject(fileStream, FiresecManager.SystemConfiguration);
            fileStream.Close();
        }

        public List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            var internalJournal = FiresecInternalClient.ReadEvents(startIndex, count);

            List<JournalRecord> journalItems = new List<JournalRecord>();
            if ((internalJournal != null) && (internalJournal.Journal != null))
            {
                foreach (var innerJournalItem in internalJournal.Journal)
                {
                    var journalItem = JournalConverter.Convert(innerJournalItem);
                    journalItems.Add(journalItem);
                }
            }

            return journalItems;
        }

        public void AddToIgnoreList(List<string> deviceIds)
        {
            List<string> devicePaths = new List<string>();
            foreach (var id in deviceIds)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x=>x.Id == id);
                devicePaths.Add(device.PlaceInTree);
            }
            FiresecInternalClient.AddToIgnoreList(devicePaths);
        }

        public void RemoveFromIgnoreList(List<string> deviceIds)
        {
            List<string> devicePaths = new List<string>();
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
            FiresecInternalClient.ExecuteCommand(device.PlaceInTree, methodName);
        }

        public Dictionary<string, string> GetHashAndNameFiles(string directory)
        {
            Dictionary<string, string> hashTable = new Dictionary<string, string>();
            List<string> HashListSoundFiles = new List<string>();
            string currentDirectory = Directory.GetCurrentDirectory() + @"\" + directory;
            DirectoryInfo dir = new DirectoryInfo(currentDirectory);
            FileInfo[] files = dir.GetFiles();
            byte[] hash;
            StringBuilder sBuilder = new StringBuilder();
            foreach (FileInfo fInfo in files)
            {
                sBuilder.Clear();
                using (FileStream fileStream = fInfo.Open(FileMode.Open))
                {
                    hash = MD5.Create().ComputeHash(fileStream);
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sBuilder.Append(hash[i].ToString());
                    }
                }
                
                hashTable.Add(sBuilder.ToString(), fInfo.Name);
            }
            return hashTable;
        }

        public Stream GetFile(string dirNameAndFileName)
        {
            string filePath = Directory.GetCurrentDirectory() + @"\" + dirNameAndFileName;
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public string Ping()
        {
            return "Pong";
        }
    }
}
