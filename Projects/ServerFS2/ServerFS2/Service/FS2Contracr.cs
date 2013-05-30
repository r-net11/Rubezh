using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS2Api;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Models;
using Common;
using System.Threading;
using ServerFS2.Processor;

namespace ServerFS2.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class FS2Contracr : IFS2Contract
	{
		public List<FS2Callbac> Poll(Guid clientUID)
		{
			try
			{
				ClientsManager.Add(clientUID);

				var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID);
				if (clientInfo != null)
				{
					var result = CallbackManager.Get(clientInfo);
					if (result.Count == 0)
					{
						clientInfo.PollWaitEvent.WaitOne(TimeSpan.FromMinutes(1));
						result = CallbackManager.Get(clientInfo);
					}
					return result;
				}
				return new List<FS2Callbac>();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FS2Contract.Poll");
				return null;
			}
		}

		public void CancelPoll(Guid clientUID)
		{
			var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID);
			if (clientInfo != null)
			{
				clientInfo.PollWaitEvent.Set();
			}
		}

		public void CancelProgress()
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> GetCoreConfig()
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> GetMetadata()
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> GetCoreState()
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> GetCoreDeviceParams()
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> ReadEvents(int fromId, int limit)
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

		public OperationResult<bool> SetNewConfig(DeviceConfiguration deviceConfiguration)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> DeviceWriteConfig(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> DeviceSetPassword(Guid deviceUID, string password, int deviceUser)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> DeviceDatetimeSync(Guid deviceUID, bool isUSB)
		{
			return SafeCall<bool>(() =>
			{
				for (int i = 0; i < 10; i++)
				{
					var fs2ProgressInfo = new FS2ProgressInfo()
					{
						Comment = "Test Callbac " + i.ToString()
					};
					CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = fs2ProgressInfo });
					Thread.Sleep(1000);
				}
				var device = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
				return true;
				MainManager.DeviceDatetimeSync(device, isUSB);
				return true;
			}, "DeviceDatetimeSync");
		}

		public OperationResult<string> DeviceGetInformation(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> DeviceGetSerialList(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> DeviceUpdateFirmware(Guid deviceUID, string fileName)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, string fileName)
		{
			throw new NotImplementedException();
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfig(Guid deviceUID, bool isUSB)
		{
			return SafeCall<DeviceConfiguration>(() =>
			{
				for (int i = 0; i < 10; i++)
				{
					var fs2ProgressInfo = new FS2ProgressInfo()
					{
						Comment = "Test Callbac " + i.ToString()
					};
					CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = fs2ProgressInfo });
					Thread.Sleep(1000);
				}
				var device = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
				return null;
				return MainManager.DeviceReadConfig(device, isUSB);
			}, "DeviceReadConfig");
		}

		public OperationResult<string> DeviceReadEventLog(Guid deviceUID, int type)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> DeviceCustomFunctionList(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> DeviceCustomFunctionExecute(Guid deviceUID, string functionName)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> DeviceSetGuardUsersList(Guid deviceUID, string users)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		OperationResult<T> SafeCall<T>(Func<T> func, string methodName)
		{
			var resultData = new OperationResult<T>();
			try
			{
				var result = func();
				resultData.Result = result;
				resultData.HasError = false;
				resultData.Error = null;
				return resultData;
			}
			catch (Exception e)
			{
				string exceptionText = "Exception " + e.Message + " при вызове " + methodName;
				Logger.Error(e, exceptionText);
				resultData.Result = default(T);
				resultData.HasError = true;
				resultData.Error = e.Message;
			}
			return resultData;
		}
	}
}