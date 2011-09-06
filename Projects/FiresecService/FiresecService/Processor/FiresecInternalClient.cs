using System;
using System.Collections.Generic;
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

        public static Firesec.CoreConfiguration.config GetCoreConfig()
        {
            return SerializerHelper.Deserialize<Firesec.CoreConfiguration.config>(DispatcherFiresecClient.GetCoreConfig());
        }

        public static Firesec.Plans.surfaces GetPlans()
        {
            return SerializerHelper.Deserialize<Firesec.Plans.surfaces>(DispatcherFiresecClient.GetPlans());
        }

        public static Firesec.CoreState.config GetCoreState()
        {
            return SerializerHelper.Deserialize<Firesec.CoreState.config>(DispatcherFiresecClient.GetCoreState());
        }

        public static Firesec.Metadata.config GetMetaData()
        {
            return SerializerHelper.Deserialize<Firesec.Metadata.config>(DispatcherFiresecClient.GetMetaData());
        }

        public static Firesec.DeviceParameters.config GetDeviceParams()
        {
            return SerializerHelper.Deserialize<Firesec.DeviceParameters.config>(DispatcherFiresecClient.GetCoreDeviceParams());
        }

        public static Firesec.Journals.document ReadEvents(int fromId, int limit)
        {
            return SerializerHelper.Deserialize<Firesec.Journals.document>(DispatcherFiresecClient.ReadEvents(fromId, limit));
        }

        public static void SetNewConfig(Firesec.CoreConfiguration.config coreConfig)
        {
            var stringConfig = SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig);
            DispatcherFiresecClient.SetNewConfig(stringConfig);
        }

        public static void DeviceWriteConfig(Firesec.CoreConfiguration.config coreConfig, string DevicePath)
        {
            DispatcherFiresecClient.DeviceWriteConfig(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), DevicePath);
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