using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;

namespace FS2Client
{
	public partial class FS2ClientContract
	{
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

		public FiresecAPI.OperationResult<string> GetCoreConfig()
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> GetMetadata()
		{
			throw new NotImplementedException();
		}

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

		public FiresecAPI.OperationResult<bool> SetNewConfig(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<bool> DeviceWriteConfig(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<bool> DeviceSetPassword(Guid deviceUID, string password, int deviceUser)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<bool> DeviceDatetimeSync(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceDatetimeSync(deviceUID, isUSB); }, "DeviceDatetimeSync");
		}

		public FiresecAPI.OperationResult<string> DeviceGetInformation(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> DeviceGetSerialList(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> DeviceUpdateFirmware(Guid deviceUID, string fileName)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, string fileName)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<DeviceConfiguration> DeviceReadConfig(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceReadConfig(deviceUID, isUSB); }, "DeviceReadConfig");
		}

		public FiresecAPI.OperationResult<string> DeviceReadEventLog(Guid deviceUID, int type)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> DeviceCustomFunctionList(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> DeviceCustomFunctionExecute(Guid deviceUID, string functionName)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<bool> DeviceSetGuardUsersList(Guid deviceUID, string users)
		{
			throw new NotImplementedException();
		}

		public FiresecAPI.OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			throw new NotImplementedException();
		}
	}
}