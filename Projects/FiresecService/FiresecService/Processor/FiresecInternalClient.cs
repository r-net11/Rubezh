using System;
using System.Collections.Generic;
using Firesec;

namespace FiresecService
{
    public static class FiresecInternalClient
    {
        public static FiresecOperationResult<bool> Connect(string login, string password)
        {
            var result = DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.Connect(login, password);
            });

            if (result.Result)
            {
                FiresecEventAggregator.NewEventAvaliable += new Action<int>(NewEventsAvailable);
                DispatcherFiresecClient.Progress += new FiresecEventAggregator.ProgressDelegate(FiresecEventAggregator_Progress);
            }
            return result;
        }

        public static FiresecOperationResult<bool> Disconnect()
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.Disconnect();
            });
        }

        public static FiresecOperationResult<Firesec.CoreConfiguration.config> GetCoreConfig()
        {
            var result = DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() =>
            {
                return NativeFiresecClient.GetCoreConfig();
            });

            return DispatcherFiresecClient.ConvertResultData<Firesec.CoreConfiguration.config>(result);
        }

        public static FiresecOperationResult<Firesec.Plans.surfaces> GetPlans()
        {
            var result = DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() =>
            {
                return NativeFiresecClient.GetPlans();
            });

            return DispatcherFiresecClient.ConvertResultData<Firesec.Plans.surfaces>(result);
        }

        public static FiresecOperationResult<Firesec.CoreState.config> GetCoreState()
        {
            var result = DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() =>
            {
                return NativeFiresecClient.GetCoreState();
            });

            return DispatcherFiresecClient.ConvertResultData<Firesec.CoreState.config>(result);
        }

        public static FiresecOperationResult<Firesec.Metadata.config> GetMetaData()
        {
            var result = DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() =>
            {
                return NativeFiresecClient.GetMetadata();
            });

            return DispatcherFiresecClient.ConvertResultData<Firesec.Metadata.config>(result);
        }

        public static FiresecOperationResult<Firesec.DeviceParameters.config> GetDeviceParams()
        {
            var result = DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() =>
            {
                return NativeFiresecClient.GetCoreDeviceParams();
            });

            return DispatcherFiresecClient.ConvertResultData<Firesec.DeviceParameters.config>(result);
        }

        public static FiresecOperationResult<Firesec.Journals.document> ReadEvents(int fromId, int limit)
        {
            var result = DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() =>
            {
                return NativeFiresecClient.ReadEvents(fromId, limit);
            });

            return DispatcherFiresecClient.ConvertResultData<Firesec.Journals.document>(result);
        }

        public static FiresecOperationResult<bool> SetNewConfig(Firesec.CoreConfiguration.config coreConfig)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.SetNewConfig(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig));
                });
        }

        public static FiresecOperationResult<bool> DeviceWriteConfig(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.DeviceWriteConfig(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
                });
        }

        public static FiresecOperationResult<bool> ResetStates(Firesec.CoreState.config coreState)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.ResetStates(SerializerHelper.Serialize<Firesec.CoreState.config>(coreState));
                });
        }

        public static FiresecOperationResult<bool> ExecuteCommand(string devicePath, string methodName)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.ExecuteCommand(devicePath, methodName);
                });
        }

        public static FiresecOperationResult<string> CheckHaspPresence()
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() => {
                return NativeFiresecClient.CheckHaspPresence();
                });
        }

        public static FiresecOperationResult<bool> AddToIgnoreList(List<string> devicePaths)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.AddToIgnoreList(devicePaths);
                });
        }

        public static FiresecOperationResult<bool> RemoveFromIgnoreList(List<string> devicePaths)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.RemoveFromIgnoreList(devicePaths);
                });
        }

        public static FiresecOperationResult<bool> AddUserMessage(string message)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.AddUserMessage(message);
                });
        }

        public static FiresecOperationResult<bool> DeviceSetPassword(Firesec.CoreConfiguration.config coreConfig, string devicePath, string password, int deviceUser)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.DeviceSetPassword(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, password, deviceUser);
                });
        }

        public static FiresecOperationResult<bool> DeviceDatetimeSync(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.DeviceDatetimeSync(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
                });
        }

        public static FiresecOperationResult<string> DeviceGetInformation(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() => {
                return NativeFiresecClient.DeviceGetInformation(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
            });
        }

        public static FiresecOperationResult<string> DeviceGetSerialList(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() => {
                return NativeFiresecClient.DeviceGetSerialList(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
                });
        }

        public static FiresecOperationResult<string> DeviceUpdateFirmware(Firesec.CoreConfiguration.config coreConfig, string devicePath, string fileName)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() => {
                return NativeFiresecClient.DeviceUpdateFirmware(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, fileName);
                });
        }

        public static FiresecOperationResult<string> DeviceVerifyFirmwareVersion(Firesec.CoreConfiguration.config coreConfig, string devicePath, string fileName)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() => {
                return NativeFiresecClient.DeviceVerifyFirmwareVersion(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, fileName);
                });
        }

        public static FiresecOperationResult<string> DeviceReadEventLog(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() => {
                return NativeFiresecClient.DeviceReadEventLog(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
                });
        }

        public static FiresecOperationResult<Firesec.CoreConfiguration.config> DeviceAutoDetectChildren(Firesec.CoreConfiguration.config coreConfig, string devicePath, bool fastSearch)
        {
            var result = DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() =>
            {
                return NativeFiresecClient.DeviceAutoDetectChildren(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, fastSearch);
            });

            return DispatcherFiresecClient.ConvertResultData<Firesec.CoreConfiguration.config>(result);
        }

        public static FiresecOperationResult<Firesec.CoreConfiguration.config> DeviceReadConfig(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            var result = DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() =>
            {
                return NativeFiresecClient.DeviceReadConfig(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
            });

            return DispatcherFiresecClient.ConvertResultData<Firesec.CoreConfiguration.config>(result);
        }

        public static FiresecOperationResult<Firesec.DeviceCustomFunctions.functions> DeviceCustomFunctionList(string driverUID)
        {
            var result = DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() =>
            {
                return NativeFiresecClient.DeviceCustomFunctionList(driverUID);
            });

            return DispatcherFiresecClient.ConvertResultData<Firesec.DeviceCustomFunctions.functions>(result);
        }

        public static FiresecOperationResult<string> DeviceCustomFunctionExecute(Firesec.CoreConfiguration.config coreConfig, string devicePath, string functionName)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() => {
                return NativeFiresecClient.DeviceCustomFunctionExecute(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, functionName);
                });
        }

        public static FiresecOperationResult<string> DeviceGetGuardUsersList(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() => {
                return NativeFiresecClient.DeviceGetGuardUsersList(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
                });
        }

        public static FiresecOperationResult<bool> DeviceSetGuardUsersList(Firesec.CoreConfiguration.config coreConfig, string devicePath, string users)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<bool>>(() =>
            {
                return NativeFiresecClient.DeviceSetGuardUsersList(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, users);
            });
        }

        public static FiresecOperationResult<string> DeviceGetMDS5Data(Firesec.CoreConfiguration.config coreConfig, string devicePath)
        {
            return DispatcherFiresecClient.SafeExecute<FiresecOperationResult<string>>(() =>
            {
                return NativeFiresecClient.DeviceGetMDS5Data(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
            });
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