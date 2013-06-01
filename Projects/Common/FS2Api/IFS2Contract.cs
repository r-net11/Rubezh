using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Models;

namespace FS2Api
{
	[ServiceContract]
	public interface IFS2Contract
	{
		#region Main
		[OperationContract]
		List<FS2Callbac> Poll(Guid clientUID);

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
		void AddToIgnoreList(List<Guid> deviceUIDs);

		[OperationContract]
		void RemoveFromIgnoreList(List<Guid> deviceUIDs);

		[OperationContract]
		void ResetStates(string states);

		[OperationContract]
		void SetZoneGuard(Guid deviceUID, string localZoneNo);

		[OperationContract]
		void UnSetZoneGuard(Guid deviceUID, string localZoneNo);

		[OperationContract]
		void AddUserMessage(string message);

		[OperationContract]
		OperationResult<string> ExecuteRuntimeDeviceMethod(Guid deviceUID, string methodName, string parameters);

		[OperationContract]
		OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName);

		[OperationContract]
		OperationResult<bool> CheckHaspPresence();
		#endregion

		#region Administrator
		[OperationContract]
		OperationResult<bool> SetNewConfig(DeviceConfiguration deviceConfiguration);

		[OperationContract]
		OperationResult<bool> DeviceWriteConfig(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<bool> DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password);

		[OperationContract]
		OperationResult<bool> DeviceDatetimeSync(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID);

		[OperationContract]
		OperationResult<string> DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName);

		[OperationContract]
		OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName);

		[OperationContract]
		OperationResult<DeviceConfiguration> DeviceReadConfig(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<string> DeviceReadEventLog(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch);

		[OperationContract]
		OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(DriverType driverType);

		[OperationContract]
		OperationResult<string> DeviceCustomFunctionExecute(Guid deviceUID, bool isUSB, string functionName);

		[OperationContract]
		OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> DeviceSetGuardUsersList(Guid deviceUID, string users);

		[OperationContract]
		OperationResult<string> DeviceGetMDS5Data(Guid deviceUID);
		#endregion
	}
}