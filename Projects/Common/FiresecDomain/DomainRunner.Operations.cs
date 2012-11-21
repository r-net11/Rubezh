using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace FiresecDomain
{
	public partial class DomainRunner
	{
		public OperationResult<string> GetCoreConfig()
		{
			return NativeFiresecClient.GetCoreConfig();
		}
		public OperationResult<string> GetPlans()
		{
			return NativeFiresecClient.GetPlans();
		}
		public OperationResult<string> GetMetadata()
		{
			return NativeFiresecClient.GetMetadata();
		}
		public OperationResult<string> GetCoreState()
		{
			return NativeFiresecClient.GetCoreState();
		}
		public OperationResult<string> GetCoreDeviceParams()
		{
			return NativeFiresecClient.GetCoreDeviceParams();
		}
		public OperationResult<bool> SetNewConfig(string coreConfig)
		{
			return NativeFiresecClient.SetNewConfig(coreConfig);
		}

		public OperationResult<string> ReadEvents(int fromId, int limit)
		{
			return NativeFiresecClient.ReadEvents(fromId, limit);
		}
		public OperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters, ref int reguestId)
		{
			return NativeFiresecClient.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters, ref reguestId);
		}
		public OperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters)
		{
			return NativeFiresecClient.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters);
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
		public void ResetStates(string states)
		{
			NativeFiresecClient.ResetStates(states);
		}
		public void SetZoneGuard(string placeInTree, string localZoneNo)
		{
			NativeFiresecClient.SetZoneGuard(placeInTree, localZoneNo);
		}
		public void UnSetZoneGuard(string placeInTree, string localZoneNo)
		{
			NativeFiresecClient.UnSetZoneGuard(placeInTree, localZoneNo);
		}
		public OperationResult<bool> AddUserMessage(string message)
		{
			return NativeFiresecClient.AddUserMessage(message);
		}

		public OperationResult<bool> DeviceWriteConfig(string coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceWriteConfig(coreConfig, devicePath);
		}
		public OperationResult<bool> DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
		{
			return NativeFiresecClient.DeviceSetPassword(coreConfig, devicePath, password, deviceUser);
		}
		public OperationResult<bool> DeviceDatetimeSync(string coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceDatetimeSync(coreConfig, devicePath);
		}
		public OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceGetInformation(coreConfig, devicePath);
		}
		public OperationResult<string> DeviceGetSerialList(string coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceGetSerialList(coreConfig, devicePath);
		}
		public OperationResult<string> DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
		{
			return NativeFiresecClient.DeviceUpdateFirmware(coreConfig, devicePath, fileName);
		}
		public OperationResult<string> DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
		{
			return NativeFiresecClient.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName);
		}
		public OperationResult<string> DeviceReadConfig(string coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceReadConfig(coreConfig, devicePath);
		}
		public OperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceReadEventLog(coreConfig, devicePath);
		}
		public OperationResult<string> DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
		{
			return NativeFiresecClient.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch);
		}
		public OperationResult<string> DeviceCustomFunctionList(string driverUID)
		{
			return NativeFiresecClient.DeviceCustomFunctionList(driverUID);
		}
		public OperationResult<string> DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
		{
			return NativeFiresecClient.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName);
		}
		public OperationResult<string> DeviceGetGuardUsersList(string coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceGetGuardUsersList(coreConfig, devicePath);
		}
		public OperationResult<bool> DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
		{
			return NativeFiresecClient.DeviceSetGuardUsersList(coreConfig, devicePath, users);
		}
		public OperationResult<string> DeviceGetMDS5Data(string coreConfig, string devicePath)
		{
			return NativeFiresecClient.DeviceGetMDS5Data(coreConfig, devicePath);
		}

		string ConvertDeviceList(List<string> devicePaths)
		{
			var devicePatsString = new StringBuilder();
			foreach (string device in devicePaths)
			{
				devicePatsString.AppendLine(device);
			}
			return devicePatsString.ToString().TrimEnd();
		}
	}
}