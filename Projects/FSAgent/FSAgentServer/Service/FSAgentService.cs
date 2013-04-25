using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;
using System.ServiceModel;
using FiresecAPI.Models;
using System.Threading;
using Common;
using FiresecAPI;
using System.Diagnostics;

namespace FSAgentServer
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class FSAgentContract : IFSAgentContract
	{
		NativeFiresecClient DirectClient
		{
			get { return WatcherManager.Current.DirectClient; }
		}

		#region Main
		public List<FSAgentCallbac> Poll(Guid clientUID)
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
				return new List<FSAgentCallbac>();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FSAgentContract.Poll");
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
			NativeFiresecClient.ContinueProgress = false;
		}
		#endregion

		#region Common
		public OperationResult<string> GetCoreConfig()
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.GetCoreConfig();
			}));
		}
		public OperationResult<string> GetMetadata()
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.GetMetadata();
			}));
		}
		#endregion

		#region Monitor
		public OperationResult<string> GetCoreState()
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.GetCoreState();
			}));
		}
		public OperationResult<string> GetCoreDeviceParams()
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.GetCoreDeviceParams();
			}));
		}
		public OperationResult<string> ReadEvents(int fromId, int limit)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.ReadEvents(fromId, limit);
			}));
		}
		public void AddToIgnoreList(List<string> devicePaths)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				DirectClient.AddToIgnoreList(devicePaths);
			});
		}
		public void RemoveFromIgnoreList(List<string> devicePaths)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				DirectClient.RemoveFromIgnoreList(devicePaths);
			});
		}
		public void ResetStates(string states)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				DirectClient.ResetStates(states);
			});
		}
		public void SetZoneGuard(string placeInTree, string localZoneNo)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				DirectClient.SetZoneGuard(placeInTree, localZoneNo);
			});
		}
		public void UnSetZoneGuard(string placeInTree, string localZoneNo)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				DirectClient.UnSetZoneGuard(placeInTree, localZoneNo);
			});
		}
		public void AddUserMessage(string message)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				DirectClient.AddUserMessage(message);
			});
		}
		public OperationResult<StringRequestIdResult> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters)
		{
			return (OperationResult<StringRequestIdResult>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters);
			}));
		}

		public OperationResult<bool> ExecuteCommand(string devicePath, string methodName)
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.ExecuteCommand(devicePath, methodName);
			}));
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.CheckHaspPresence();
			}));
		}
		#endregion

		#region Administrator
		public OperationResult<bool> SetNewConfig(string coreConfig)
		{
			DBHashHelper.RemoveHash();
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.SetNewConfig(coreConfig);
			}));
		}
		public OperationResult<bool> DeviceWriteConfig(string coreConfig, string devicePath)
		{
			DBHashHelper.RemoveHash();
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceWriteConfig(coreConfig, devicePath);
			}));
		}
		public OperationResult<bool> DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceSetPassword(coreConfig, devicePath, password, deviceUser);
			}));
		}
		public OperationResult<bool> DeviceDatetimeSync(string coreConfig, string devicePath)
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceDatetimeSync(coreConfig, devicePath);
			}));
		}
		public OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceGetInformation(coreConfig, devicePath);
			}));
		}
		public OperationResult<string> DeviceGetSerialList(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceGetSerialList(coreConfig, devicePath);
			}));
		}
		public OperationResult<string> DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceUpdateFirmware(coreConfig, devicePath, fileName);
			}));
		}
		public OperationResult<string> DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName);
			}));
		}
		public OperationResult<string> DeviceReadConfig(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceReadConfig(coreConfig, devicePath);
			}));
		}
		public OperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath, int type)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceReadEventLog(coreConfig, devicePath, type);
			}));
		}
		public OperationResult<string> DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch);
			}));
		}
		public OperationResult<string> DeviceCustomFunctionList(string driverUID)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceCustomFunctionList(driverUID);
			}));
		}
		public OperationResult<string> DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName);
			}));
		}
		public OperationResult<string> DeviceGetGuardUsersList(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceGetGuardUsersList(coreConfig, devicePath);
			}));
		}
		public OperationResult<bool> DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceSetGuardUsersList(coreConfig, devicePath, users);
			}));
		}
		public OperationResult<string> DeviceGetMDS5Data(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.DeviceGetMDS5Data(coreConfig, devicePath);
			}));
		}
		public OperationResult<string> GetPlans()
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return DirectClient.GetPlans();
			}));
		}
		#endregion
	}
}