using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI;

namespace FSAgentAPI
{
	[ServiceContract]
	public interface IFSAgentContract
    {
        #region Main
        [OperationContract]
        List<FSAgentCallbac> Poll(Guid clientUID);

		[OperationContract]
		void CancelPoll(Guid clientUID);

        [OperationContract]
        void CancelProgress();
        #endregion

        #region Common
        [OperationContract]
        OperationResult<string> GetCoreConfig();

        [OperationContract]
        OperationResult<string> GetMetadata();
        #endregion

        #region Monitor
        [OperationContract]
        OperationResult<string> GetCoreState();

        [OperationContract]
        OperationResult<string> GetCoreDeviceParams();

        [OperationContract]
        OperationResult<string> ReadEvents(int fromId, int limit);

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
        OperationResult<StringRequestIdResult> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters);

        [OperationContract]
        OperationResult<bool> ExecuteCommand(string devicePath, string methodName);

        [OperationContract]
        OperationResult<bool> CheckHaspPresence();
        #endregion

        #region Administrator
        [OperationContract]
        OperationResult<bool> SetNewConfig(string coreConfig);

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
        OperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath, int type);

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

        [OperationContract]
        OperationResult<string> GetPlans();
        #endregion
	}
}