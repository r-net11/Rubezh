using System;
using System.Collections.Generic;
using System.IO;
using Firesec;

namespace FiresecService
{
    public static class FiresecInternalClient
    {
        public static bool Connect(string login, string password)
        {
            bool result = DispatcherFiresecClient.Connect(login, password);
            if (result)
            {
                FiresecEventAggregator.NewEventAvaliable += new Action<int>(NewEventsAvailable);
            }
            return result;
        }

        public static void Disconnect()
        {
            DispatcherFiresecClient.Disconnect();
        }

        public static Firesec.CoreConfig.config GetCoreConfig()
        {
            return SerializerHelper.Deserialize<Firesec.CoreConfig.config>(DispatcherFiresecClient.GetCoreConfig());
        }

        public static Firesec.CoreState.config GetCoreState()
        {
            return SerializerHelper.Deserialize<Firesec.CoreState.config>(DispatcherFiresecClient.GetCoreState());
        }

        public static Firesec.Metadata.config GetMetaData()
        {
            return SerializerHelper.Deserialize<Firesec.Metadata.config>(DispatcherFiresecClient.GetMetaData());
        }

        public static Firesec.DeviceParams.config GetDeviceParams()
        {
            return SerializerHelper.Deserialize<Firesec.DeviceParams.config>(DispatcherFiresecClient.GetCoreDeviceParams());
        }

        public static Firesec.ReadEvents.document ReadEvents(int fromId, int limit)
        {
            return SerializerHelper.Deserialize<Firesec.ReadEvents.document>(DispatcherFiresecClient.ReadEvents(fromId, limit));
        }

        public static void SetNewConfig(Firesec.CoreConfig.config coreConfig)
        {
            var stringConfig = SerializerHelper.Serialize<Firesec.CoreConfig.config>(coreConfig);
            DispatcherFiresecClient.SetNewConfig(stringConfig);
        }

        public static void SaveConfigToFile(Firesec.CoreConfig.config coreConfig, string fileName)
        {
            var stringConfig = SerializerHelper.Serialize<Firesec.CoreConfig.config>(coreConfig);

            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(stringConfig);
            streamWriter.Close();
            fileStream.Close();
        }

        public static Firesec.CoreConfig.config LoadConfigFromFile(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);
            var stringConfig = streamReader.ReadToEnd();
            fileStream.Close();

            Firesec.CoreConfig.config coreConfig = SerializerHelper.Deserialize<Firesec.CoreConfig.config>(stringConfig);
            return coreConfig;
        }

        public static void DeviceWriteConfig(Firesec.CoreConfig.config coreConfig, string DevicePath)
        {
            DispatcherFiresecClient.DeviceWriteConfig(SerializerHelper.Serialize<Firesec.CoreConfig.config>(coreConfig), DevicePath);
        }

        public static void ResetStates(Firesec.CoreState.config coreState)
        {
            string stringCoreState = SerializerHelper.Serialize<Firesec.CoreState.config>(coreState);
            DispatcherFiresecClient.ResetStates(stringCoreState);
        }

        public static void ExecuteCommand(string devicePath, string methodName)
        {
            DispatcherFiresecClient.ExecuteCommand(devicePath, methodName);
        }

        public static void AddToIgnoreList(List<string> devicePaths)
        {
            DispatcherFiresecClient.AddToIgnoreList(devicePaths);
        }

        public static void RemoveFromIgnoreList(List<string> devicePaths)
        {
            DispatcherFiresecClient.RemoveFromIgnoreList(devicePaths);
        }

        public static void AddUserMessage(string message)
        {
            DispatcherFiresecClient.AddUserMessage(message);
        }

        public static void NewEventsAvailable(int eventMask)
        {
            if (NewEvent != null)
                NewEvent(eventMask);
        }

        public static event Action<int> NewEvent;
    }
}
