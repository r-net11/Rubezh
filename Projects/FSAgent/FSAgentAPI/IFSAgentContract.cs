using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using FiresecAPI.Models;
using FiresecAPI;

namespace FSAgentAPI
{
	[ServiceContract]
	public interface IFSAgentContract
	{
		[OperationContract]
        List<FSAgentCallbac> Poll(Guid clientUID);

		[OperationContract]
		void CanceProgress();

		[OperationContract]
		void AddToIgnoreList(List<string> devicePaths);

		[OperationContract]
		void RemoveFromIgnoreList(List<string> devicePaths);

		[OperationContract]
		void ResetStates(string states);

		[OperationContract]
		void SetZoneGuard(string placeInTree, string localZoneNo);

		[OperationContract]
		void UnSetZoneGuard(string placeInTree, string localZoneNo);

		[OperationContract]
		void AddUserMessage(string message);

		[OperationContract]
		OperationResult<string> GetCoreConfig();

		[OperationContract]
		OperationResult<string> GetPlans();

		[OperationContract]
		OperationResult<string> GetMetadata();

		[OperationContract]
		OperationResult<string> GetCoreState();

		[OperationContract]
		OperationResult<string> GetCoreDeviceParams();

		[OperationContract]
		OperationResult<bool> SetNewConfig(string coreConfig);

		[OperationContract]
		OperationResult<string> ReadEvents(int fromId, int limit);

		[OperationContract]
		OperationResult<StringRequestIdResult> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters);

		[OperationContract]
		OperationResult<bool> ExecuteCommand(string devicePath, string methodName);

		[OperationContract]
		OperationResult<bool> CheckHaspPresence();

		[OperationContract]
		OperationResult<bool> DeviceWriteConfig(string coreConfig, string devicePath);

		[OperationContract]
		OperationResult<bool> DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser);

		[OperationContract]
		OperationResult<bool> DeviceDatetimeSync(string coreConfig, string devicePath);

		[OperationContract]
		OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath);

		[OperationContract]
		OperationResult<string> DeviceGetSerialList(string coreConfig, string devicePath);

		[OperationContract]
		OperationResult<string> DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName);

		[OperationContract]
		OperationResult<string> DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName);

		[OperationContract]
		OperationResult<string> DeviceReadConfig(string coreConfig, string devicePath);

		[OperationContract]
		OperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath);

		[OperationContract]
		OperationResult<string> DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch);

		[OperationContract]
		OperationResult<string> DeviceCustomFunctionList(string driverUID);

		[OperationContract]
		OperationResult<string> DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName);

		[OperationContract]
		OperationResult<string> DeviceGetGuardUsersList(string coreConfig, string devicePath);

		[OperationContract]
		OperationResult<bool> DeviceSetGuardUsersList(string coreConfig, string devicePath, string users);

		[OperationContract]
		OperationResult<string> DeviceGetMDS5Data(string coreConfig, string devicePath);
	}
}