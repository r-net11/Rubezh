using System;
using System.Collections.Generic;
using Firesec;
using Firesec.Models.Functions;
using FiresecAPI;

namespace Firesec
{
	public class FiresecSerializedClient
	{
		public NativeFiresecClient NativeFiresecClient { get; private set; }

		public FiresecSerializedClient()
		{
			NativeFiresecClient = new NativeFiresecClient();
		}

		public OperationResult<bool> Connect(string login, string password)
		{
			var result = NativeFiresecClient.Connect(login, password);

			if (result.Result)
			{
				NativeFiresecClient.NewEventAvaliable += new Action<int>(NewEventsAvailable);
				NativeFiresecClient.ProgressEvent += new Action<int, string, int, int>(FiresecEventAggregator_Progress);
			}
			return result;
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

		public OperationResult<bool> ResetStates(Firesec.Models.CoreState.config coreState)
		{
			return NativeFiresecClient.ResetStates(SerializerHelper.Serialize<Firesec.Models.CoreState.config>(coreState));
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

		public OperationResult<bool> AddToIgnoreList(List<string> devicePaths)
		{
			return NativeFiresecClient.AddToIgnoreList(devicePaths);
		}

		public OperationResult<bool> RemoveFromIgnoreList(List<string> devicePaths)
		{
			return NativeFiresecClient.RemoveFromIgnoreList(devicePaths);
		}

		public OperationResult<bool> AddUserMessage(string message)
		{
			return NativeFiresecClient.AddUserMessage(message);
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

		public OperationResult<string> GetConfigurationParameters(string devicePath, int paramNo)
		{
			return NativeFiresecClient.GetConfigurationParameters(devicePath, paramNo);
		}

		OperationResult<T> ConvertResultData<T>(OperationResult<string> result)
		{
			var resultData = new OperationResult<T>();
			resultData.HasError = result.HasError;
			resultData.Error = result.Error;
			if (result.HasError == false)
				resultData.Result = SerializerHelper.Deserialize<T>(result.Result);
			return resultData;
		}

		public void NewEventsAvailable(int eventMask)
		{
			if (NewEvent != null)
				NewEvent(eventMask);
		}

		void FiresecEventAggregator_Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
			if (Progress != null)
				Progress(stage, comment, percentComplete, bytesRW);
		}

		public event Action<int> NewEvent;
		public event Action<int, string, int, int> Progress;
	}
}