using System.Collections.Generic;
using System.ServiceModel;
using System.IO;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Converters;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Cryptography;

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

        public void WriteConfiguration(string devicePath)
        {
            FiresecInternalClient.DeviceWriteConfig(FiresecManager.CoreConfig, devicePath);
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            //DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
            //FileStream fileStream = new FileStream("D:/SystemConfiguration.xml", FileMode.Open);
            //FiresecManager.SystemConfiguration = (SystemConfiguration)dataContractSerializer.ReadObject(fileStream);
            //fileStream.Close();

            return FiresecManager.SystemConfiguration;
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            FiresecManager.SystemConfiguration = systemConfiguration;

            DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
            FileStream fileStream = new FileStream("D:/SystemConfiguration.xml", FileMode.Create);
            dataContractSerializer.WriteObject(fileStream, FiresecManager.SystemConfiguration);
            fileStream.Close();
        }

        public List<JournalItem> ReadJournal(int startIndex, int count)
        {
            var internalJournal = FiresecInternalClient.ReadEvents(startIndex, count);

            List<JournalItem> journalItems = new List<JournalItem>();
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

        public void AddToIgnoreList(List<string> devicePaths)
        {
            FiresecInternalClient.AddToIgnoreList(devicePaths);
        }

        public void RemoveFromIgnoreList(List<string> devicePaths)
        {
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

        public void ExecuteCommand(string devicePath, string methodName)
        {
            FiresecInternalClient.ExecuteCommand(devicePath, methodName);
        }

        public List<string> GetSoundsFileName()
        {
            List<string> listSoungsFileName = new List<string>();
            var soungsFileName = Directory.GetFiles(@"C:\Program Files\Firesec\Sounds");
            foreach (string str in soungsFileName)
            {
                listSoungsFileName.Add(Path.GetFileName(str));
            }
            return listSoungsFileName;
        }

        public Dictionary<string, string> GetHashAndNameSoundFiles()
        {
            Dictionary<string, string> hashTable = new Dictionary<string, string>();
            List<string> HashListSoundFiles = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(@"C:\Program Files\Firesec\Sounds");
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

        public Stream GetFile(string filename)
        {
            string filePath = @"C:\Program Files\Firesec\Sounds\" + filename;

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File was not found", Path.GetFileName(filePath));

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
    }
}
