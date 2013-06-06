using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FS2Api;
using FiresecAPI;

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
		public OperationResult<DeviceConfiguration> GetDeviceConfiguration()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDeviceConfiguration(); }, "GetDeviceConfiguration");
		}

		public OperationResult<DriversConfiguration> GetDriversConfiguration()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDriversConfiguration(); }, "GetDriversConfiguration");
		}
		#endregion

		#region Monitor
		public OperationResult<List<DeviceState>> GetDeviceStates()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDeviceStates(); }, "GetDeviceStates");
		}

		public OperationResult<List<DeviceState>> GetDeviceParameters()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDeviceParameters(); }, "GetDeviceParameters");
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

		public OperationResult<string> ExecuteRuntimeDeviceMethod(Guid deviceUID, string methodName, string parameters)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Administrator
		public OperationResult<bool> SetNewConfig(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
		{
			return SafeOperationCall(() => { return FS2Contract.SetNewConfig(deviceConfiguration); }, "SetNewConfig");
		}

		public OperationResult<bool> DeviceWriteConfig(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceWriteConfig(deviceUID, isUSB); }, "DeviceWriteConfig");
		}

		public OperationResult<bool> DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceSetPassword(deviceUID, isUSB, devicePasswordType, password); }, "DeviceSetPassword");
		}

		public OperationResult<bool> DeviceDatetimeSync(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceDatetimeSync(deviceUID, isUSB); }, "DeviceDatetimeSync");
		}

		public OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetInformation(deviceUID, isUSB); }, "DeviceGetInformation");
		}

		public OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetSerialList(deviceUID); }, "DeviceGetSerialList");
		}

		public OperationResult<string> DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceUpdateFirmware(deviceUID, isUSB, fileName); }, "DeviceUpdateFirmware");
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceVerifyFirmwareVersion(deviceUID, isUSB, fileName); }, "DeviceVerifyFirmwareVersion");
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfig(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceReadConfig(deviceUID, isUSB); }, "DeviceReadConfig");
		}

		public OperationResult<List<FS2JournalItem>> DeviceReadEventLog(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceReadEventLog(deviceUID, isUSB); }, "DeviceReadEventLog");
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceAutoDetectChildren(deviceUID, fastSearch); }, "DeviceAutoDetectChildren");
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(DriverType driverType)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceCustomFunctionList(driverType); }, "DeviceCustomFunctionList");
		}

		public OperationResult<string> DeviceCustomFunctionExecute(Guid deviceUID, bool isUSB, string functionName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceCustomFunctionExecute(deviceUID, isUSB, functionName); }, "DeviceCustomFunctionExecute");
		}

		public OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetGuardUsersList(deviceUID); }, "DeviceGetGuardUsersList");
		}

		public OperationResult<bool> DeviceSetGuardUsersList(Guid deviceUID, string users)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceSetGuardUsersList(deviceUID, users); }, "DeviceSetGuardUsersList");
		}

		public OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetMDS5Data(deviceUID); }, "DeviceGetMDS5Data");
		}

		public OperationResult<bool> SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			return SafeOperationCall(() => { return FS2Contract.SetConfigurationParameters(deviceUID, properties); }, "SetConfigurationParameters");
		}

		public OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.GetConfigurationParameters(deviceUID); }, "GetConfigurationParameters");
		}
		#endregion
	}
}