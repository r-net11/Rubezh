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
using Common;
using FiresecServiceRunner;

namespace FiresecService
{    
    [ServiceBehavior(MaxItemsInObjectGraph = 2147483647, UseSynchronizationContext=true, InstanceContextMode=InstanceContextMode.PerSession)]
    public class FiresecService : IFiresecService, IDisposable
    {
        IFiresecCallback _currentCallback;

        public void Connect()
        {
            _currentCallback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
            CallbackManager.Add(_currentCallback);


            MainWindow.AddMessage("Connected " + OperationContext.Current.SessionId);
        }

        [OperationBehavior(ReleaseInstanceMode=ReleaseInstanceMode.AfterCall)]
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
                FileStream fileStream = new FileStream("SystemConfiguration.xml", FileMode.Open);
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
            FileStream fileStream = new FileStream("SystemConfiguration.xml", FileMode.Create);
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

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            return HashHelper.GetDirectoryHash(directory);
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

        public void Dispose()
        {
            Disconnect();
        }

        public string Test()
        {
            //throw(new FaultException<Exception>(new Exception("Test"), new FaultReason("Test")));
            return "Test";
        }
    }
}
