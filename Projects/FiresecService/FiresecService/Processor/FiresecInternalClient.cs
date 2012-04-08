using System;
using System.Collections.Generic;
using Firesec;

namespace FiresecService
{
    public static class FiresecInternalClient
    {
        public static bool Connect(string login, string password)
        {
            if (DispatcherFiresecClient.Connect(login, password))
            {
                FiresecEventAggregator.NewEventAvaliable += new Action<int>(NewEventsAvailable);
                DispatcherFiresecClient.Progress += new FiresecEventAggregator.ProgressDelegate(FiresecEventAggregator_Progress);

                return true;
            }
            return false;
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
            DispatcherFiresecClient.SetNewConfig(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig));
        }

        public static string DeviceWriteConfig(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.DeviceWriteConfig(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public static void ResetStates(Firesec.CoreState.config coreState)
        {
            DispatcherFiresecClient.ResetStates(SerializerHelper.Serialize<Firesec.CoreState.config>(coreState));
        }

        public static void ExecuteCommand(string devicePath, string methodName)
        {
            DispatcherFiresecClient.ExecuteCommand(devicePath, methodName);
        }

        public static string CheckHaspPresence()
        {
            return DispatcherFiresecClient.CheckHaspPresence();
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

        public static bool DeviceSetPassword(Firesec.CoreConfiguration.config coreConfig, string devicePath, string password, int deviceUser)
        {
            return DispatcherFiresecClient.DeviceSetPassword(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, password, deviceUser);
        }

        public static bool DeviceDatetimeSync(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.DeviceDatetimeSync(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public static string DeviceGetInformation(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.DeviceGetInformation(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public static string DeviceGetSerialList(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.DeviceGetSerialList(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public static string DeviceUpdateFirmware(Firesec.CoreConfiguration.config coreConfig, string devicePath, string fileName)
        {
            return DispatcherFiresecClient.DeviceUpdateFirmware(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, fileName);
        }

        public static string DeviceVerifyFirmwareVersion(Firesec.CoreConfiguration.config coreConfig, string devicePath, string fileName)
        {
            return DispatcherFiresecClient.DeviceVerifyFirmwareVersion(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, fileName);
        }

        public static string DeviceReadEventLog(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.DeviceReadEventLog(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public static Firesec.CoreConfiguration.config DeviceAutoDetectChildren(Firesec.CoreConfiguration.config coreConfig, string devicePath, bool fastSearch)
        {
            var stringConfig = DispatcherFiresecClient.DeviceAutoDetectChildren(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, fastSearch);
            return SerializerHelper.Deserialize<Firesec.CoreConfiguration.config>(stringConfig);
        }

        public static Firesec.CoreConfiguration.config DeviceReadConfig(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            var stringConfig = DispatcherFiresecClient.DeviceReadConfig(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
            return SerializerHelper.Deserialize<Firesec.CoreConfiguration.config>(stringConfig);
        }

        public static Firesec.DeviceCustomFunctions.functions DeviceCustomFunctionList(string driverUID)
        {
            var stringFunctions = DispatcherFiresecClient.DeviceCustomFunctionList(driverUID);
            return SerializerHelper.Deserialize<Firesec.DeviceCustomFunctions.functions>(stringFunctions);
        }

        public static string DeviceCustomFunctionExecute(Firesec.CoreConfiguration.config coreConfig, string devicePath, string functionName)
        {
            return DispatcherFiresecClient.DeviceCustomFunctionExecute(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, functionName);
        }

        public static string DeviceGetGuardUsersList(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.DeviceGetGuardUsersList(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public static string DeviceSetGuardUsersList(Firesec.CoreConfiguration.config coreConfig, string devicePath, string users)
        {
            return DispatcherFiresecClient.DeviceSetGuardUsersList(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, users);
        }

        public static string DeviceGetMDS5Data(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.DeviceGetMDS5Data(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public static void NewEventsAvailable(int eventMask)
        {
            if (NewEvent != null)
                NewEvent(eventMask);
        }

        static bool FiresecEventAggregator_Progress(int stage, string comment, int percentComplete, int bytesRW)
        {
            if (Progress != null)
                return Progress(stage, comment, percentComplete, bytesRW);
            return true;
        }

        public static event Action<int> NewEvent;
        public static event FiresecEventAggregator.ProgressDelegate Progress;
    }
}