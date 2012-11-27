using System;
using System.Collections.Generic;
using System.Text;
using Common;
using FiresecAPI;
using FSAgentClient;

namespace Firesec
{
	public partial class NativeFiresecClient : FS_Types.IFS_CallBack
	{
        public FSAgent FSAgent { get; set; }

		public OperationResult<string> GetCoreConfig()
		{
			return FSAgent.GetCoreConfig();
			//return SafeCall<string>(() => { return ReadFromStream(Connection.GetCoreConfig()); }, "GetCoreConfig");
		}
		public OperationResult<string> GetPlans()
		{
			return FSAgent.GetPlans();
			//return SafeCall<string>(() => { return Connection.GetCoreAreasW(); }, "GetPlans");
		}
		public OperationResult<string> GetMetadata()
		{
			return FSAgent.GetMetadata();
			//return SafeCall<string>(() => { return ReadFromStream(Connection.GetMetaData()); }, "GetMetadata");
		}
		public OperationResult<string> GetCoreState()
		{
			return FSAgent.GetCoreState();
			//return SafeCall<string>(() => { return ReadFromStream(Connection.GetCoreState()); }, "GetCoreState");
		}
		public OperationResult<string> GetCoreDeviceParams()
		{
			return FSAgent.GetCoreDeviceParams();
			//return SafeCall<string>(() => { return Connection.GetCoreDeviceParams(); }, "GetCoreDeviceParams");
		}
		public OperationResult<bool> SetNewConfig(string coreConfig)
		{
			return FSAgent.SetNewConfig(coreConfig);
			//return SafeCall<bool>(() => { Connection.SetNewConfig(coreConfig); return true; }, "SetNewConfig");
		}

		public OperationResult<string> ReadEvents(int fromId, int limit)
		{
			return FSAgent.ReadEvents(fromId, limit);
			//return SafeCall<string>(() => { return Connection.ReadEvents(fromId, limit); }, "ReadEvents");
		}
		public OperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters, ref int reguestId)
		{
			var operationResult = FSAgent.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters);
			var result = new OperationResult<string>()
			{
				Error = operationResult.Error,
				HasError = operationResult.HasError
			};
			if (operationResult.Result != null)
			{
				reguestId = operationResult.Result.ReguestId;
				result.Result = operationResult.Result.Result;
			}
			return result;

			//_reguestId++;
			//reguestId = _reguestId;
			//return SafeCall<string>(() =>
			//{
			//    return Connection.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters, _reguestId);
			//}, "ExecuteRuntimeDeviceMethod");
		}
		public OperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters)
		{
			return SafeCall<string>(() => { Connection.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters, _reguestId++); return null; }, "ExecuteRuntimeDeviceMethod");
		}

		public OperationResult<bool> ExecuteCommand(string devicePath, string methodName)
		{
			return FSAgent.ExecuteCommand(devicePath, methodName);
			//return SafeCall<bool>(() =>
			//{
			//    _reguestId++;
			//    var result = Connection.ExecuteRuntimeDeviceMethod(devicePath, methodName, "Test", _reguestId);
			//    return true;
			//}, "ExecuteCommand");
		}
		public OperationResult<bool> CheckHaspPresence()
		{
			return FSAgent.CheckHaspPresence();
			//return SafeCall<bool>(() =>
			//{
			//    string errorMessage = "";
			//    try
			//    {
			//        var result = Connection.CheckHaspPresence(out errorMessage);
			//        return result;
			//    }
			//    catch (Exception e)
			//    {
			//        Logger.Error(e, "Исключение при вызове NativeFiresecClient.CheckHaspPresence");
			//        return true;
			//    }
			//}, "CheckHaspPresence");
		}
		public void AddToIgnoreList(List<string> devicePaths)
		{
			FSAgent.AddToIgnoreList(devicePaths);
			//AddTask(() =>
			//    {
			//        SafeCall<bool>(() => { Connection.IgoreListOperation(ConvertDeviceList(devicePaths), true); return true; }, "AddToIgnoreList");
			//    });
		}
		public void RemoveFromIgnoreList(List<string> devicePaths)
		{
			FSAgent.RemoveFromIgnoreList(devicePaths);
			//AddTask(() =>
			//    {
			//        SafeCall<bool>(() => { Connection.IgoreListOperation(ConvertDeviceList(devicePaths), false); return true; }, "RemoveFromIgnoreList");
			//    });
		}
		public void ResetStates(string states)
		{
			FSAgent.ResetStates(states);
			//AddTask(() =>
			//    {
			//        SafeCall<bool>(() =>
			//        {
			//            Connection.ResetStates(states);
			//            return true;
			//        }, "ResetStates");
			//    });
		}
		public void SetZoneGuard(string placeInTree, string localZoneNo)
		{
			FSAgent.SetZoneGuard(placeInTree, localZoneNo);
			//AddTask(() =>
			//{
			//    SafeCall<bool>(() => { int reguestId = 0; ExecuteRuntimeDeviceMethod(placeInTree, "SetZoneToGuard", localZoneNo, ref reguestId); return true; }, "SetZoneGuard");
			//});
		}
		public void UnSetZoneGuard(string placeInTree, string localZoneNo)
		{
			FSAgent.UnSetZoneGuard(placeInTree, localZoneNo);
			//AddTask(() =>
			//{
			//    SafeCall<bool>(() => { int reguestId = 0; ExecuteRuntimeDeviceMethod(placeInTree, "UnSetZoneFromGuard", localZoneNo, ref reguestId); return true; }, "UnSetZoneGuard");
			//});
		}
		public void AddUserMessage(string message)
		{
			FSAgent.AddUserMessage(message);
			//return SafeCall<bool>(() => { Connection.StoreUserMessage(message); return true; }, "AddUserMessage");
		}

		public OperationResult<bool> DeviceWriteConfig(string coreConfig, string devicePath)
		{
			return FSAgent.DeviceWriteConfig(coreConfig, devicePath);
			//return SafeCall<bool>(() => { Connection.DeviceWriteConfig(coreConfig, devicePath); return true; }, "DeviceWriteConfig");
		}
		public OperationResult<bool> DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
		{
			return FSAgent.DeviceSetPassword(coreConfig, devicePath, password, deviceUser);
			//return SafeCall<bool>(() => { Connection.DeviceSetPassword(coreConfig, devicePath, password, deviceUser); return true; }, "DeviceSetPassword");
		}
		public OperationResult<bool> DeviceDatetimeSync(string coreConfig, string devicePath)
		{
			return FSAgent.DeviceDatetimeSync(coreConfig, devicePath);
			//return SafeCall<bool>(() => { Connection.DeviceDatetimeSync(coreConfig, devicePath); return true; }, "DeviceDatetimeSync");
		}
        public OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
        {
            return FSAgent.DeviceGetInformation(coreConfig, devicePath);
            //return SafeCall<string>(() => { return Connection.DeviceGetInformation(coreConfig, devicePath); }, "DeviceGetInformation");
        }
		public OperationResult<string> DeviceGetSerialList(string coreConfig, string devicePath)
		{
			return FSAgent.DeviceGetSerialList(coreConfig, devicePath);
			//return SafeCall<string>(() => { return Connection.DeviceGetSerialList(coreConfig, devicePath); }, "DeviceGetSerialList");
		}
		public OperationResult<string> DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
		{
			return FSAgent.DeviceUpdateFirmware(coreConfig, devicePath, fileName);
			//return SafeCall<string>(() => { return Connection.DeviceUpdateFirmware(coreConfig, devicePath, fileName); }, "DeviceUpdateFirmware");
		}
		public OperationResult<string> DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
		{
			return FSAgent.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName);
			//return SafeCall<string>(() => { return Connection.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName); }, "DeviceVerifyFirmwareVersion");
		}
		public OperationResult<string> DeviceReadConfig(string coreConfig, string devicePath)
		{
			return FSAgent.DeviceReadConfig(coreConfig, devicePath);
			//return SafeCall<string>(() => { return Connection.DeviceReadConfig(coreConfig, devicePath); }, "DeviceReadConfig");
		}
		public OperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath)
		{
			return FSAgent.DeviceReadEventLog(coreConfig, devicePath);
			//return SafeCall<string>(() => { return Connection.DeviceReadEventLog(coreConfig, devicePath, 0); }, "DeviceReadEventLog");
		}
		public OperationResult<string> DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
		{
			return FSAgent.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch);
			//return SafeCall<string>(() => { return Connection.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch); }, "DeviceAutoDetectChildren");
		}
		public OperationResult<string> DeviceCustomFunctionList(string driverUID)
		{
			return FSAgent.DeviceCustomFunctionList(driverUID);
			//return SafeCall<string>(() => { return Connection.DeviceCustomFunctionList(driverUID); }, "DeviceCustomFunctionList");
		}
		public OperationResult<string> DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
		{
			return FSAgent.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName);
			//return SafeCall<string>(() => { return Connection.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName); }, "DeviceCustomFunctionExecute");
		}
		public OperationResult<string> DeviceGetGuardUsersList(string coreConfig, string devicePath)
		{
			return FSAgent.DeviceGetGuardUsersList(coreConfig, devicePath);
			//return SafeCall<string>(() => { return Connection.DeviceGetGuardUsersList(coreConfig, devicePath); }, "DeviceGetGuardUsersList");
		}
		public OperationResult<bool> DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
		{
			return FSAgent.DeviceSetGuardUsersList(coreConfig, devicePath, users);
			//return SafeCall<bool>(() => { Connection.DeviceSetGuardUsersList(coreConfig, devicePath, users); return true; }, "DeviceSetGuardUsersList");
		}
		public OperationResult<string> DeviceGetMDS5Data(string coreConfig, string devicePath)
		{
			return FSAgent.DeviceGetMDS5Data(coreConfig, devicePath);
			//return SafeCall<string>(() => { return Connection.DeviceGetMDS5Data(coreConfig, devicePath); }, "DeviceGetMDS5Data");
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

		string ReadFromStream(mscoree.IStream stream)
		{
			var stringBuilder = new StringBuilder();
			try
			{
				unsafe
				{
					byte* unsafeBytes = stackalloc byte[1024];
					while (true)
					{
						var _intPtr = new IntPtr(unsafeBytes);
						uint bytesRead = 0;
						stream.Read(_intPtr, 1024, out bytesRead);
						if (bytesRead == 0)
							break;

						var bytes = new byte[bytesRead];
						for (int i = 0; i < bytesRead; ++i)
						{
							bytes[i] = unsafeBytes[i];
						}
						stringBuilder.Append(Encoding.Default.GetString(bytes));
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове NativeFiresecClient.ReadFromStream");
			}

			return stringBuilder.ToString();
		}
	}
}