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
        public List<FSAgentCallbac> Poll(Guid clientUID)
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

		public void CanceProgress()
		{
			NativeFiresecClient.ContinueProgress = false;
		}

		public void AddToIgnoreList(List<string> devicePaths)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				WatcherManager.Current.DirectClient.AddToIgnoreList(devicePaths);
			});
		}
		public void RemoveFromIgnoreList(List<string> devicePaths)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				WatcherManager.Current.DirectClient.RemoveFromIgnoreList(devicePaths);
			});
		}
		public void ResetStates(string states)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				WatcherManager.Current.DirectClient.ResetStates(states);
			});
		}
		public void SetZoneGuard(string placeInTree, string localZoneNo)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				WatcherManager.Current.DirectClient.SetZoneGuard(placeInTree, localZoneNo);
			});
		}
		public void UnSetZoneGuard(string placeInTree, string localZoneNo)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				WatcherManager.Current.DirectClient.UnSetZoneGuard(placeInTree, localZoneNo);
			});
		}
		public void AddUserMessage(string message)
		{
			WatcherManager.Current.AddNonBlockingTask(() =>
			{
				WatcherManager.Current.DirectClient.AddUserMessage(message);
			});
		}

		public OperationResult<string> GetCoreConfig()
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.GetCoreConfig();
			}));
		}
		public OperationResult<string> GetPlans()
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.GetPlans();
			}));
		}
		public OperationResult<string> GetMetadata()
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.GetMetadata();
			}));
		}
		public OperationResult<string> GetCoreState()
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.GetCoreState();
			}));
		}
		public OperationResult<string> GetCoreDeviceParams()
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.GetCoreDeviceParams();
			}));
		}
		public OperationResult<bool> SetNewConfig(string coreConfig)
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.SetNewConfig(coreConfig);
			}));
		}

		public OperationResult<string> ReadEvents(int fromId, int limit)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.ReadEvents(fromId, limit);
			}));
		}

		public OperationResult<StringRequestIdResult> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters)
		{
			return (OperationResult<StringRequestIdResult>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters);
			}));
		}

		public OperationResult<bool> ExecuteCommand(string devicePath, string methodName)
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.ExecuteCommand(devicePath, methodName);
			}));
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.CheckHaspPresence();
			}));
		}

		public OperationResult<bool> DeviceWriteConfig(string coreConfig, string devicePath)
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceWriteConfig(coreConfig, devicePath);
			}));
		}

		public OperationResult<bool> DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceSetPassword(coreConfig, devicePath, password, deviceUser);
			}));
		}

		public OperationResult<bool> DeviceDatetimeSync(string coreConfig, string devicePath)
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceDatetimeSync(coreConfig, devicePath);
			}));
		}

		public OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceGetInformation(coreConfig, devicePath);
			}));
		}

		public OperationResult<string> DeviceGetSerialList(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceGetSerialList(coreConfig, devicePath);
			}));
		}

		public OperationResult<string> DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceUpdateFirmware(coreConfig, devicePath, fileName);
			}));
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName);
			}));
		}

		public OperationResult<string> DeviceReadConfig(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceReadConfig(coreConfig, devicePath);
			}));
		}

		public OperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceReadEventLog(coreConfig, devicePath);
			}));
		}

		public OperationResult<string> DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch);
			}));
		}

		public OperationResult<string> DeviceCustomFunctionList(string driverUID)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceCustomFunctionList(driverUID);
			}));
		}

		public OperationResult<string> DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName);
			}));
		}

		public OperationResult<string> DeviceGetGuardUsersList(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceGetGuardUsersList(coreConfig, devicePath);
			}));
		}

		public OperationResult<bool> DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
		{
			return (OperationResult<bool>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceSetGuardUsersList(coreConfig, devicePath, users);
			}));
		}

		public OperationResult<string> DeviceGetMDS5Data(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.AddBlockingTask(new Func<object>(() =>
			{
				return WatcherManager.Current.DirectClient.DeviceGetMDS5Data(coreConfig, devicePath);
			}));
		}
	}
}