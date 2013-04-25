using System;
using System.Collections.Generic;
using Firesec.Models.Functions;
using FiresecAPI;
using FiresecAPI.Models;
using FSAgentAPI;
using FSAgentClient;

namespace Firesec
{
    public class FiresecSerializedClient
    {
		public FSAgent FSAgent { get; set; }

        public FiresecSerializedClient()
        {
			FiresecDriverAuParametersHelper.FiresecSerializedClient = this;
        }

		public event Action<List<JournalRecord>> NewJournalRecords;
		public event Action<Firesec.Models.CoreState.config> StateChanged;
		public event Action<Firesec.Models.DeviceParameters.config> ParametersChanged;
		public event Action<int, string, int, int> ProgressEvent;

		public void SubscribeFsAgentEvents()
		{
			FSAgent.NewJournalRecords += new Action<List<FiresecAPI.Models.JournalRecord>>(FSAgent_NewJournalRecords);
			FSAgent.CoreConfigChanged += new Action<string>(FSAgent_CoreConfigChanged);
			FSAgent.CoreDeviceParamsChanged += new Action<string>(FSAgent_CoreDeviceParamsChanged);
			FSAgent.Progress += new Action<FSAgentAPI.FSProgressInfo>(FSAgent_Progress);
		}

		void FSAgent_NewJournalRecords(List<JournalRecord> journalRecords)
		{
			foreach (var journalRecord in journalRecords)
			{
				JournalConverter.SetDeviceCatogory(journalRecord);
			}
			if (NewJournalRecords != null)
				NewJournalRecords(journalRecords);
		}

		void FSAgent_CoreConfigChanged(string result)
		{
			var coreState = ConvertResultData<Firesec.Models.CoreState.config>(result);
			if (coreState.Result != null)
			{
				if (StateChanged != null)
					StateChanged(coreState.Result);
			}
		}

		void FSAgent_CoreDeviceParamsChanged(string result)
		{
			var coreParameters = ConvertResultData<Firesec.Models.DeviceParameters.config>(result);
			if (coreParameters.Result != null)
			{
				if (ParametersChanged != null)
					ParametersChanged(coreParameters.Result);
			}
		}

		void FSAgent_Progress(FSProgressInfo fsProgressInfo)
		{
			if (fsProgressInfo != null)
			{
				if (ProgressEvent != null)
					ProgressEvent(fsProgressInfo.Stage, fsProgressInfo.Comment, fsProgressInfo.PercentComplete, fsProgressInfo.BytesRW);
			}
		}

		OperationResult<T> ConvertResultData<T>(string result)
		{
			var resultData = new OperationResult<T>();
			resultData.Result = SerializerHelper.Deserialize<T>(result);
			return resultData;
		}

        public OperationResult<Firesec.Models.CoreConfiguration.config> GetCoreConfig()
        {
			var result = FSAgent.GetCoreConfig();
            return ConvertResultData<Firesec.Models.CoreConfiguration.config>(result);
        }

        public OperationResult<Firesec.Models.Plans.surfaces> GetPlans()
        {
			var result = FSAgent.GetPlans();
            return ConvertResultData<Firesec.Models.Plans.surfaces>(result);
        }

        public OperationResult<Firesec.Models.CoreState.config> GetCoreState()
        {
			var result = FSAgent.GetCoreState();
            return ConvertResultData<Firesec.Models.CoreState.config>(result);
        }

        public OperationResult<Firesec.Models.Metadata.config> GetMetaData()
        {
			var result = FSAgent.GetMetadata();
            return ConvertResultData<Firesec.Models.Metadata.config>(result);
        }

        public OperationResult<Firesec.Models.DeviceParameters.config> GetDeviceParams()
        {
			var result = FSAgent.GetCoreDeviceParams();
            return ConvertResultData<Firesec.Models.DeviceParameters.config>(result);
        }

        public OperationResult<Firesec.Models.Journals.document> ReadEvents(int fromId, int limit)
        {
			var result = FSAgent.ReadEvents(fromId, limit);
            return ConvertResultData<Firesec.Models.Journals.document>(result);
        }

        public OperationResult<bool> SetNewConfig(Firesec.Models.CoreConfiguration.config coreConfig)
        {
			return FSAgent.SetNewConfig(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig));
        }

        public OperationResult<bool> DeviceWriteConfig(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
			return FSAgent.DeviceWriteConfig(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters, ref int reguestId)
        {
			var callResult = FSAgent.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters);
			var operationResult = new OperationResult<string>()
			{
				Error = callResult.Error,
				HasError = callResult.HasError
			};
			if (callResult.Result != null)
			{
				reguestId = callResult.Result.ReguestId;
				operationResult.Result = callResult.Result.Result;
			}
			return operationResult;
        }

        public OperationResult<bool> ExecuteCommand(string devicePath, string methodName)
        {
			return FSAgent.ExecuteCommand(devicePath, methodName);
        }

        public OperationResult<bool> CheckHaspPresence()
        {
			return FSAgent.CheckHaspPresence();
        }

        public void AddToIgnoreList(List<string> devicePaths)
        {
			FSAgent.AddToIgnoreList(devicePaths);
        }

        public void RemoveFromIgnoreList(List<string> devicePaths)
        {
			FSAgent.RemoveFromIgnoreList(devicePaths);
        }

        public void ResetStates(Firesec.Models.CoreState.config coreState)
        {
			FSAgent.ResetStates(SerializerHelper.Serialize<Firesec.Models.CoreState.config>(coreState));
        }

        public void SetZoneGuard(string placeInTree, string localZoneNo)
        {
			FSAgent.SetZoneGuard(placeInTree, localZoneNo);
        }

        public void UnSetZoneGuard(string placeInTree, string localZoneNo)
        {
			FSAgent.UnSetZoneGuard(placeInTree, localZoneNo);
        }

        public void AddUserMessage(string message)
        {
			FSAgent.AddUserMessage(message);
        }

        public OperationResult<bool> DeviceSetPassword(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, string password, int deviceUser)
        {
			return FSAgent.DeviceSetPassword(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, password, deviceUser);
        }

        public OperationResult<bool> DeviceDatetimeSync(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
			return FSAgent.DeviceDatetimeSync(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<string> DeviceGetInformation(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
			return FSAgent.DeviceGetInformation(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<string> DeviceGetSerialList(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
			return FSAgent.DeviceGetSerialList(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<string> DeviceUpdateFirmware(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, string fileName)
        {
			return FSAgent.DeviceUpdateFirmware(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, fileName);
        }

        public OperationResult<string> DeviceVerifyFirmwareVersion(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, string fileName)
        {
			return FSAgent.DeviceVerifyFirmwareVersion(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, fileName);
        }

		public OperationResult<string> DeviceReadEventLog(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, int type)
        {
			return FSAgent.DeviceReadEventLog(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, type);
        }

        public OperationResult<Firesec.Models.CoreConfiguration.config> DeviceAutoDetectChildren(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, bool fastSearch)
        {
			var result = FSAgent.DeviceAutoDetectChildren(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, fastSearch);
            return ConvertResultData<Firesec.Models.CoreConfiguration.config>(result);
        }

        public OperationResult<Firesec.Models.CoreConfiguration.config> DeviceReadConfig(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
			var result = FSAgent.DeviceReadConfig(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
            return ConvertResultData<Firesec.Models.CoreConfiguration.config>(result);
        }

        public OperationResult<functions> DeviceCustomFunctionList(string driverUID)
        {
			var result = FSAgent.DeviceCustomFunctionList(driverUID);
            return ConvertResultData<functions>(result);
        }

        public OperationResult<string> DeviceCustomFunctionExecute(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, string functionName)
        {
			return FSAgent.DeviceCustomFunctionExecute(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, functionName);
        }

        public OperationResult<string> DeviceGetGuardUsersList(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
			return FSAgent.DeviceGetGuardUsersList(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
        }

        public OperationResult<bool> DeviceSetGuardUsersList(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath, string users)
        {
			return FSAgent.DeviceSetGuardUsersList(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath, users);
        }

        public OperationResult<string> DeviceGetMDS5Data(Firesec.Models.CoreConfiguration.config coreConfig, string devicePath)
        {
			return FSAgent.DeviceGetMDS5Data(SerializerHelper.Serialize<Firesec.Models.CoreConfiguration.config>(coreConfig), devicePath);
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