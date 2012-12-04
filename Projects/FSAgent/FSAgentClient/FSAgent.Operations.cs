using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;
using FiresecAPI.Models;
using FiresecAPI;

namespace FSAgentClient
{
    public partial class FSAgent
    {
        #region Main
        public List<FSAgentCallbac> Poll(Guid clientUID)
        {
			return SafeOperationCall(() => { return FSAgentContract.Poll(clientUID); }, "Poll");
        }
		public void CancelPoll(Guid clientUID)
		{
			SafeOperationCall(() => { FSAgentContract.CancelPoll(clientUID); }, "CancelPoll");
		}
		public void CancelProgress()
		{
			SafeOperationCall(() => { FSAgentContract.CancelProgress(); }, "CanceProgress");
		}
        #endregion

        #region Common
        public OperationResult<string> GetCoreConfig()
        {
            return SafeOperationCall(() => { return FSAgentContract.GetCoreConfig(); }, "GetCoreConfig");
        }
        public OperationResult<string> GetMetadata()
        {
            return SafeOperationCall(() => { return FSAgentContract.GetMetadata(); }, "GetMetadata");
        }
        #endregion

        #region Monitor
        public OperationResult<string> GetCoreState()
        {
            return SafeOperationCall(() => { return FSAgentContract.GetCoreState(); }, "GetCoreState");
        }
        public OperationResult<string> GetCoreDeviceParams()
        {
            return SafeOperationCall(() => { return FSAgentContract.GetCoreDeviceParams(); }, "GetCoreDeviceParams");
        }
        public OperationResult<string> ReadEvents(int fromId, int limit)
        {
            return SafeOperationCall(() => { return FSAgentContract.ReadEvents(fromId, limit); }, "ReadEvents");
        }
        public void AddToIgnoreList(List<string> devicePaths)
        {
            SafeOperationCall(() => { FSAgentContract.AddToIgnoreList(devicePaths); }, "AddToIgnoreList");
        }
        public void RemoveFromIgnoreList(List<string> devicePaths)
        {
            SafeOperationCall(() => { FSAgentContract.RemoveFromIgnoreList(devicePaths); }, "RemoveFromIgnoreList");
        }
        public void ResetStates(string states)
        {
            SafeOperationCall(() => { FSAgentContract.ResetStates(states); }, "ResetStates");
        }
        public void SetZoneGuard(string placeInTree, string localZoneNo)
        {
            SafeOperationCall(() => { FSAgentContract.SetZoneGuard(placeInTree, localZoneNo); }, "SetZoneGuard");
        }
        public void UnSetZoneGuard(string placeInTree, string localZoneNo)
        {
            SafeOperationCall(() => { FSAgentContract.UnSetZoneGuard(placeInTree, localZoneNo); }, "UnSetZoneGuard");
        }
        public void AddUserMessage(string message)
        {
            SafeOperationCall(() => { FSAgentContract.AddUserMessage(message); }, "AddUserMessage");
        }
        public OperationResult<StringRequestIdResult> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters)
        {
            return SafeOperationCall(() => { return FSAgentContract.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters); }, "ExecuteRuntimeDeviceMethod");
        }
        public OperationResult<bool> ExecuteCommand(string devicePath, string methodName)
        {
            return SafeOperationCall(() => { return FSAgentContract.ExecuteCommand(devicePath, methodName); }, "ExecuteCommand");
        }
        public OperationResult<bool> CheckHaspPresence()
        {
            return SafeOperationCall(() => { return FSAgentContract.CheckHaspPresence(); }, "CheckHaspPresence");
        }
        #endregion

        #region Administrator
        public OperationResult<string> GetPlans()
        {
            return SafeOperationCall(() => { return FSAgentContract.GetPlans(); }, "GetPlans");
        }
		public OperationResult<bool> SetNewConfig(string coreConfig)
		{
			return SafeOperationCall(() => { return FSAgentContract.SetNewConfig(coreConfig); }, "SetNewConfig");
		}
		public OperationResult<bool> DeviceWriteConfig(string coreConfig, string devicePath)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceWriteConfig(coreConfig, devicePath); }, "DeviceWriteConfig");
		}
		public OperationResult<bool> DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceSetPassword(coreConfig, devicePath, password, deviceUser); }, "DeviceSetPassword");
		}
		public OperationResult<bool> DeviceDatetimeSync(string coreConfig, string devicePath)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceDatetimeSync(coreConfig, devicePath); }, "DeviceDatetimeSync");
		}
		public OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceGetInformation(coreConfig, devicePath); }, "DeviceGetInformation");
		}
		public OperationResult<string> DeviceGetSerialList(string coreConfig, string devicePath)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceGetSerialList(coreConfig, devicePath); }, "DeviceGetSerialList");
		}
		public OperationResult<string> DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceUpdateFirmware(coreConfig, devicePath, fileName); }, "DeviceUpdateFirmware");
		}
		public OperationResult<string> DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName); }, "DeviceVerifyFirmwareVersion");
		}
		public OperationResult<string> DeviceReadConfig(string coreConfig, string devicePath)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceReadConfig(coreConfig, devicePath); }, "DeviceReadConfig");
		}
		public OperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceReadEventLog(coreConfig, devicePath); }, "DeviceReadEventLog");
		}
		public OperationResult<string> DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch); }, "DeviceAutoDetectChildren");
		}
		public OperationResult<string> DeviceCustomFunctionList(string driverUID)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceCustomFunctionList(driverUID); }, "DeviceCustomFunctionList");
		}
		public OperationResult<string> DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName); }, "DeviceCustomFunctionExecute");
		}
		public OperationResult<string> DeviceGetGuardUsersList(string coreConfig, string devicePath)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceGetGuardUsersList(coreConfig, devicePath); }, "DeviceGetGuardUsersList");
		}
		public OperationResult<bool> DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceSetGuardUsersList(coreConfig, devicePath, users); }, "DeviceSetGuardUsersList");
		}
		public OperationResult<string> DeviceGetMDS5Data(string coreConfig, string devicePath)
		{
			return SafeOperationCall(() => { return FSAgentContract.DeviceGetMDS5Data(coreConfig, devicePath); }, "DeviceGetMDS5Data");
        }
        #endregion
    }
}