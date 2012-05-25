using System;
using System.Collections.Generic;
using Firesec;

namespace FiresecService.Processor
{
	public class FiresecSerializedClient
	{
		NativeFiresecClient NativeFiresecClient;

		public FiresecSerializedClient()
		{
			NativeFiresecClient = new NativeFiresecClient();
		}

		public FiresecOperationResult<bool> Connect(string login, string password)
		{
			var result = NativeFiresecClient.Connect(login, password);

			if (result.Result)
			{
				NativeFiresecClient.NewEventAvaliable += new Action<int>(NewEventsAvailable);
				NativeFiresecClient.ProgressEvent += new Func<int, string, int, int, bool>(FiresecEventAggregator_Progress);
			}
			return result;
		}

		public FiresecOperationResult<Firesec.CoreConfiguration.config> GetCoreConfig()
		{
			var result = NativeFiresecClient.GetCoreConfig();
			return ConvertResultData<Firesec.CoreConfiguration.config>(result);
		}

		public FiresecOperationResult<Firesec.Plans.surfaces> GetPlans()
		{
			var result = NativeFiresecClient.GetPlans();
			return ConvertResultData<Firesec.Plans.surfaces>(result);
		}

		public FiresecOperationResult<Firesec.CoreState.config> GetCoreState()
		{
			var result = NativeFiresecClient.GetCoreState();
			return ConvertResultData<Firesec.CoreState.config>(result);
		}

		public FiresecOperationResult<Firesec.Metadata.config> GetMetaData()
		{
			var result = NativeFiresecClient.GetMetadata();
			return ConvertResultData<Firesec.Metadata.config>(result);
		}

		public FiresecOperationResult<Firesec.DeviceParameters.config> GetDeviceParams()
		{
			var result = NativeFiresecClient.GetCoreDeviceParams();
			return ConvertResultData<Firesec.DeviceParameters.config>(result);
		}

		public FiresecOperationResult<Firesec.Journals.document> ReadEvents(int fromId, int limit)
		{
			var result = NativeFiresecClient.ReadEvents(fromId, limit);
			return ConvertResultData<Firesec.Journals.document>(result);
		}

		public FiresecOperationResult<bool> SetNewConfig(Firesec.CoreConfiguration.config coreConfig)
		{
			return NativeFiresecClient.SetNewConfig(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig));
		}

		public FiresecOperationResult<bool> DeviceWriteConfig(Firesec.CoreConfiguration.config coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceWriteConfig(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
		}

		public FiresecOperationResult<bool> ResetStates(Firesec.CoreState.config coreState)
		{
			return NativeFiresecClient.ResetStates(SerializerHelper.Serialize<Firesec.CoreState.config>(coreState));
		}

		public FiresecOperationResult<bool> ExecuteCommand(string devicePath, string methodName)
		{
			return NativeFiresecClient.ExecuteCommand(devicePath, methodName);
		}

		public FiresecOperationResult<bool> CheckHaspPresence()
		{
			return NativeFiresecClient.CheckHaspPresence();
		}

		public FiresecOperationResult<bool> AddToIgnoreList(List<string> devicePaths)
		{
			return NativeFiresecClient.AddToIgnoreList(devicePaths);
		}

		public FiresecOperationResult<bool> RemoveFromIgnoreList(List<string> devicePaths)
		{
			return NativeFiresecClient.RemoveFromIgnoreList(devicePaths);
		}

		public FiresecOperationResult<bool> AddUserMessage(string message)
		{
			return NativeFiresecClient.AddUserMessage(message);
		}

		public FiresecOperationResult<bool> DeviceSetPassword(Firesec.CoreConfiguration.config coreConfig, string devicePath, string password, int deviceUser)
		{
			return NativeFiresecClient.DeviceSetPassword(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, password, deviceUser);
		}

		public FiresecOperationResult<bool> DeviceDatetimeSync(Firesec.CoreConfiguration.config coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceDatetimeSync(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
		}

		public FiresecOperationResult<string> DeviceGetInformation(Firesec.CoreConfiguration.config coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceGetInformation(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
		}

		public FiresecOperationResult<string> DeviceGetSerialList(Firesec.CoreConfiguration.config coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceGetSerialList(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
		}

		public FiresecOperationResult<string> DeviceUpdateFirmware(Firesec.CoreConfiguration.config coreConfig, string devicePath, string fileName)
		{
			return NativeFiresecClient.DeviceUpdateFirmware(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, fileName);
		}

		public FiresecOperationResult<string> DeviceVerifyFirmwareVersion(Firesec.CoreConfiguration.config coreConfig, string devicePath, string fileName)
		{
			return NativeFiresecClient.DeviceVerifyFirmwareVersion(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, fileName);
		}

		public FiresecOperationResult<string> DeviceReadEventLog(Firesec.CoreConfiguration.config coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceReadEventLog(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
		}

		public FiresecOperationResult<Firesec.CoreConfiguration.config> DeviceAutoDetectChildren(Firesec.CoreConfiguration.config coreConfig, string devicePath, bool fastSearch)
		{
			var result = NativeFiresecClient.DeviceAutoDetectChildren(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, fastSearch);
			return ConvertResultData<Firesec.CoreConfiguration.config>(result);
		}

		public FiresecOperationResult<Firesec.CoreConfiguration.config> DeviceReadConfig(Firesec.CoreConfiguration.config coreConfig, string devicePath)
		{
			var result = NativeFiresecClient.DeviceReadConfig(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
			return ConvertResultData<Firesec.CoreConfiguration.config>(result);
		}

		public FiresecOperationResult<Firesec.DeviceCustomFunctions.functions> DeviceCustomFunctionList(string driverUID)
		{
			var result = NativeFiresecClient.DeviceCustomFunctionList(driverUID);
			return ConvertResultData<Firesec.DeviceCustomFunctions.functions>(result);
		}

		public FiresecOperationResult<string> DeviceCustomFunctionExecute(Firesec.CoreConfiguration.config coreConfig, string devicePath, string functionName)
		{
			return NativeFiresecClient.DeviceCustomFunctionExecute(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, functionName);
		}

		public FiresecOperationResult<string> DeviceGetGuardUsersList(Firesec.CoreConfiguration.config coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceGetGuardUsersList(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
		}

		public FiresecOperationResult<bool> DeviceSetGuardUsersList(Firesec.CoreConfiguration.config coreConfig, string devicePath, string users)
		{
			return NativeFiresecClient.DeviceSetGuardUsersList(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath, users);
		}

		public FiresecOperationResult<string> DeviceGetMDS5Data(Firesec.CoreConfiguration.config coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceGetMDS5Data(SerializerHelper.Serialize<Firesec.CoreConfiguration.config>(coreConfig), devicePath);
		}

		FiresecOperationResult<T> ConvertResultData<T>(FiresecOperationResult<string> result)
		{
			var resultData = new FiresecOperationResult<T>();
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

		bool FiresecEventAggregator_Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
			if (Progress != null)
				return Progress(stage, comment, percentComplete, bytesRW);
			return true;
		}

		public event Action<int> NewEvent;
		//public event FiresecEventAggregator.ProgressDelegate Progress;
		public event Func<int, string, int, int, bool> Progress;
	}
}