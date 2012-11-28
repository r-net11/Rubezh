using System;
using System.Collections.Generic;
using Common;
using Firesec.Models.Functions;
using FiresecAPI;
using Infrastructure.Common;

namespace Firesec
{
    public class FiresecSerializedClient
    {
        public NativeFiresecClient NativeFiresecClient { get; private set; }

        public FiresecSerializedClient()
        {
            try
            {
                NativeFiresecClient = new NativeFiresecClient();
                FiresecDriverAuParametersHelper.FiresecSerializedClient = this;
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecSerializedClient.FiresecSerializedClient");
                LoadingErrorManager.Add(e);
            }
        }

        public OperationResult<Firesec.Models.CoreConfiguration.config> GetCoreConfig()
        {
            var result = NativeFiresecClient.GetCoreConfig();
            return ConvertResultData<Firesec.Models.CoreConfiguration.config>(result);
        }

        public OperationResult<Firesec.Models.Plans.surfaces> GetPlans()
        {
            var result = NativeFiresecClient.GetPlans();
            return ConvertResultData<Firesec.Models.Plans.surfaces>(result);
        }

        public OperationResult<Firesec.Models.CoreState.config> GetCoreState()
        {
            var result = NativeFiresecClient.GetCoreState();
            return ConvertResultData<Firesec.Models.CoreState.config>(result);
        }

        public OperationResult<Firesec.Models.Metadata.config> GetMetaData()
        {
            var result = NativeFiresecClient.GetMetadata();
            return ConvertResultData<Firesec.Models.Metadata.config>(result);
        }

        public OperationResult<Firesec.Models.DeviceParameters.config> GetDeviceParams()
        {
            var result = NativeFiresecClient.GetCoreDeviceParams();
            return ConvertResultData<Firesec.Models.DeviceParameters.config>(result);
        }

        public OperationResult<Firesec.Models.Journals.document> ReadEvents(int fromId, int limit)
        {
            var result = NativeFiresecClient.ReadEvents(fromId, limit);
            return ConvertResultData<Firesec.Models.Journals.document>(result);
        }

        public OperationResult<bool> SetNewConfig(Firesec.Models.CoreConfiguration.config coreConfig)
        {
            return NativeFiresecClient.SetNewConfig(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig));
        }

        public OperationResult<bool> DeviceWriteConfig(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
            return NativeFiresecClient.DeviceWriteConfig(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters, ref int reguestId)
        {
            return NativeFiresecClient.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters, ref reguestId);
        }

        public OperationResult<bool> ExecuteCommand(string devicePath, string methodName)
        {
            return NativeFiresecClient.ExecuteCommand(devicePath, methodName);
        }

        public OperationResult<bool> CheckHaspPresence()
        {
            return NativeFiresecClient.CheckHaspPresence();
        }

        public void AddToIgnoreList(List<string> devicePaths)
        {
            NativeFiresecClient.AddToIgnoreList(devicePaths);
        }

        public void RemoveFromIgnoreList(List<string> devicePaths)
        {
            NativeFiresecClient.RemoveFromIgnoreList(devicePaths);
        }

        public void ResetStates(Firesec.Models.CoreState.config coreState)
        {
            NativeFiresecClient.ResetStates(SerializerHelper.Serialize<Firesec.Models.CoreState.config>(coreState));
        }

        public void SetZoneGuard(string placeInTree, string localZoneNo)
        {
            NativeFiresecClient.SetZoneGuard(placeInTree, localZoneNo);
        }

        public void UnSetZoneGuard(string placeInTree, string localZoneNo)
        {
            NativeFiresecClient.UnSetZoneGuard(placeInTree, localZoneNo);
        }

        public void AddUserMessage(string message)
        {
            NativeFiresecClient.AddUserMessage(message);
        }

        public OperationResult<bool> DeviceSetPassword(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, string password, int deviceUser)
        {
            return NativeFiresecClient.DeviceSetPassword(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, password, deviceUser);
        }

        public OperationResult<bool> DeviceDatetimeSync(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
            return NativeFiresecClient.DeviceDatetimeSync(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<string> DeviceGetInformation(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
            return NativeFiresecClient.DeviceGetInformation(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<string> DeviceGetSerialList(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
            return NativeFiresecClient.DeviceGetSerialList(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<string> DeviceUpdateFirmware(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, string fileName)
        {
            return NativeFiresecClient.DeviceUpdateFirmware(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, fileName);
        }

        public OperationResult<string> DeviceVerifyFirmwareVersion(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, string fileName)
        {
            return NativeFiresecClient.DeviceVerifyFirmwareVersion(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, fileName);
        }

        public OperationResult<string> DeviceReadEventLog(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
            return NativeFiresecClient.DeviceReadEventLog(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<Firesec.Models.CoreConfiguration.config> DeviceAutoDetectChildren(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, bool fastSearch)
        {
            var result = NativeFiresecClient.DeviceAutoDetectChildren(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, fastSearch);
            return ConvertResultData<Firesec.Models.CoreConfiguration.config>(result);
        }

        public OperationResult<Firesec.Models.CoreConfiguration.config> DeviceReadConfig(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
            var result = NativeFiresecClient.DeviceReadConfig(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
            return ConvertResultData<Firesec.Models.CoreConfiguration.config>(result);
        }

        public OperationResult<functions> DeviceCustomFunctionList(string driverUID)
        {
            var result = NativeFiresecClient.DeviceCustomFunctionList(driverUID);
            return ConvertResultData<functions>(result);
        }

        public OperationResult<string> DeviceCustomFunctionExecute(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, string functionName)
        {
            return NativeFiresecClient.DeviceCustomFunctionExecute(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, functionName);
        }

        public OperationResult<string> DeviceGetGuardUsersList(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
            return NativeFiresecClient.DeviceGetGuardUsersList(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<bool> DeviceSetGuardUsersList(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, string users)
        {
            return NativeFiresecClient.DeviceSetGuardUsersList(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, users);
        }

        public OperationResult<string> DeviceGetMDS5Data(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
            return NativeFiresecClient.DeviceGetMDS5Data(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        OperationResult<T> ConvertResultData<T>(OperationResult<string> result)
        {
            var resultData = new OperationResult<T>()
            {
                HasError = result.HasError,
                Error = result.Error
            };
            if (result.HasError == false)
                resultData.Result = SerializerHelper.Deserialize<T>(result.Result);
            return resultData;
        }
    }
}