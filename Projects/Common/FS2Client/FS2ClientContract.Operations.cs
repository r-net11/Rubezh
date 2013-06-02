using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FS2Api;

namespace FS2Client
{
	public partial class FS2ClientContract
	{
		#region Main
		public List<FS2Callbac> Poll(Guid clientUID)
		{
			return SafeOperationCall(() => { return FS2Contract.Poll(clientUID); }, "Poll");
		}

		public void CancelPoll(Guid clientUID)
		{
			SafeOperationCall(() => { FS2Contract.CancelPoll(clientUID); }, "CancelPoll");
		}

		public void CancelProgress()
		{
			SafeOperationCall(() => { FS2Contract.CancelProgress(); }, "CanceProgress");
		}
		#endregion

		#region Common
		public FiresecAPI.OperationResult<string> GetCoreConfig()
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> GetMetadata()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Monitor
		public FiresecAPI.OperationResult<string> GetCoreState()
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> GetCoreDeviceParams()
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> ReadEvents(int fromId, int limit)
		{
			throw new NotImplementedException();
		}

		public void AddToIgnoreList(List<Guid> deviceUIDs)
		{
			throw new NotImplementedException();
		}

		public void RemoveFromIgnoreList(List<Guid> deviceUIDs)
		{
			throw new NotImplementedException();
		}

		public void ResetStates(string states)
		{
			throw new NotImplementedException();
		}

		public void SetZoneGuard(Guid deviceUID, string localZoneNo)
		{
			throw new NotImplementedException();
		}

		public void UnSetZoneGuard(Guid deviceUID, string localZoneNo)
		{
			throw new NotImplementedException();
		}

		public void AddUserMessage(string message)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> ExecuteRuntimeDeviceMethod(Guid deviceUID, string methodName, string parameters)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<bool> CheckHaspPresence()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Administrator
		public FiresecAPI.OperationResult<bool> SetNewConfig(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
		{
			return SafeOperationCall(() => { return FS2Contract.SetNewConfig(deviceConfiguration); }, "SetNewConfig");
		}

		public FiresecAPI.OperationResult<bool> DeviceWriteConfig(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceWriteConfig(deviceUID, isUSB); }, "DeviceWriteConfig");
		}

		public FiresecAPI.OperationResult<bool> DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceSetPassword(deviceUID, isUSB, devicePasswordType, password); }, "DeviceSetPassword");
		}

		public FiresecAPI.OperationResult<bool> DeviceDatetimeSync(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceDatetimeSync(deviceUID, isUSB); }, "DeviceDatetimeSync");
		}

		public FiresecAPI.OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetInformation(deviceUID, isUSB); }, "DeviceGetInformation");
		}

		public FiresecAPI.OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetSerialList(deviceUID); }, "DeviceGetSerialList");
		}

		public FiresecAPI.OperationResult<string> DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceUpdateFirmware(deviceUID, isUSB, fileName); }, "DeviceUpdateFirmware");
		}

		public FiresecAPI.OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceVerifyFirmwareVersion(deviceUID, isUSB, fileName); }, "DeviceVerifyFirmwareVersion");
		}

		public FiresecAPI.OperationResult<DeviceConfiguration> DeviceReadConfig(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceReadConfig(deviceUID, isUSB); }, "DeviceReadConfig");
		}

		public FiresecAPI.OperationResult<List<FS2JournalItem>> DeviceReadEventLog(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceReadEventLog(deviceUID, isUSB); }, "DeviceReadEventLog");
		}

		public FiresecAPI.OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceAutoDetectChildren(deviceUID, fastSearch); }, "DeviceAutoDetectChildren");
		}

		public FiresecAPI.OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(DriverType driverType)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceCustomFunctionList(driverType); }, "DeviceCustomFunctionList");
		}

		public FiresecAPI.OperationResult<string> DeviceCustomFunctionExecute(Guid deviceUID, bool isUSB, string functionName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceCustomFunctionExecute(deviceUID, isUSB, functionName); }, "DeviceCustomFunctionExecute");
		}

		public FiresecAPI.OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetGuardUsersList(deviceUID); }, "DeviceGetGuardUsersList");
		}

		public FiresecAPI.OperationResult<bool> DeviceSetGuardUsersList(Guid deviceUID, string users)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceSetGuardUsersList(deviceUID, users); }, "DeviceSetGuardUsersList");
		}

		public FiresecAPI.OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetMDS5Data(deviceUID); }, "DeviceGetMDS5Data");
		}

		public FiresecAPI.OperationResult<bool> SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			return SafeOperationCall(() => { return FS2Contract.SetConfigurationParameters(deviceUID, properties); }, "SetConfigurationParameters");
		}

		public FiresecAPI.OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.GetConfigurationParameters(deviceUID); }, "GetConfigurationParameters");
		}
		#endregion
	}
}