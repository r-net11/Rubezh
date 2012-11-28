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
        public string GetStatus()
        {
            return null;
        }

        public List<FSAgentCallbac> Poll(Guid clientUID)
        {
            ClientsManager.Add(clientUID);

            var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID);
            if (clientInfo != null)
            {
                var result = CallbackManager.Get(clientInfo);
				if (result.Count == 0)
				{
					if (WatcherManager.Current.LastFSProgressInfo != null)
					{
						var fsProgressInfo = new FSProgressInfo()
						{
							Stage = WatcherManager.Current.LastFSProgressInfo.Stage,
							Comment = WatcherManager.Current.LastFSProgressInfo.Comment,
							PercentComplete = WatcherManager.Current.LastFSProgressInfo.PercentComplete,
							BytesRW = WatcherManager.Current.LastFSProgressInfo.BytesRW
						};
						WatcherManager.Current.LastFSProgressInfo = null;
						result = new List<FSAgentCallbac>();
						var fsAgentCallbac = new FSAgentCallbac()
						{
							FSProgressInfo = fsProgressInfo
						};
						result.Add(fsAgentCallbac);
						return result;
					}

					clientInfo.PollWaitEvent.WaitOne(TimeSpan.FromSeconds(10));
					result = CallbackManager.Get(clientInfo);
				}
                return result;
            }
            return new List<FSAgentCallbac>();
        }

		public FSProgressInfo PollAdministratorProgress()
		{
			if (WatcherManager.Current.LastFSProgressInfo == null)
			{
				return null;
			}
			var fsProgressInfo = new FSProgressInfo()
			{
				Stage = WatcherManager.Current.LastFSProgressInfo.Stage,
				Comment = WatcherManager.Current.LastFSProgressInfo.Comment,
				PercentComplete = WatcherManager.Current.LastFSProgressInfo.PercentComplete,
				BytesRW = WatcherManager.Current.LastFSProgressInfo.BytesRW
			};
			WatcherManager.Current.LastFSProgressInfo = null;
			Trace.WriteLine("fsProgressInfo.Comment = " + fsProgressInfo.Comment);
			return fsProgressInfo;
		}

		public FSProgressInfo PollMonitorProgress()
		{
			return null;
		}

		public void CancelAdministratorProgress()
		{
		}

		public void AddToIgnoreList(List<string> devicePaths)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.MonitorClient.AddToIgnoreList(devicePaths);
			});
		}
		public void RemoveFromIgnoreList(List<string> devicePaths)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.MonitorClient.RemoveFromIgnoreList(devicePaths);
			});
		}
		public void ResetStates(string states)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.MonitorClient.ResetStates(states);
			});
		}
		public void SetZoneGuard(string placeInTree, string localZoneNo)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.MonitorClient.SetZoneGuard(placeInTree, localZoneNo);
			});
		}
		public void UnSetZoneGuard(string placeInTree, string localZoneNo)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.MonitorClient.UnSetZoneGuard(placeInTree, localZoneNo);
			});
		}
		public void AddUserMessage(string message)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.MonitorClient.AddUserMessage(message);
			});
		}

		public OperationResult<string> GetCoreConfig()
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.GetCoreConfig();
			}));
		}
		public OperationResult<string> GetPlans()
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.GetPlans();
			}));
		}
		public OperationResult<string> GetMetadata()
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.GetMetadata();
			}));
		}
		public OperationResult<string> GetCoreState()
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.GetCoreState();
			}));
		}
		public OperationResult<string> GetCoreDeviceParams()
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.GetCoreDeviceParams();
			}));
		}
		public OperationResult<bool> SetNewConfig(string coreConfig)
		{
			return (OperationResult<bool>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.SetNewConfig(coreConfig);
			}));
		}

		public OperationResult<string> ReadEvents(int fromId, int limit)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.ReadEvents(fromId, limit);
			}));
		}

		public OperationResult<StringRequestIdResult> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters)
		{
			return (OperationResult<StringRequestIdResult>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters);
			}));
		}

		public OperationResult<bool> ExecuteCommand(string devicePath, string methodName)
		{
			return (OperationResult<bool>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.ExecuteCommand(devicePath, methodName);
			}));
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			return (OperationResult<bool>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.CheckHaspPresence();
			}));
		}

		public OperationResult<bool> DeviceWriteConfig(string coreConfig, string devicePath)
		{
			return (OperationResult<bool>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceWriteConfig(coreConfig, devicePath);
			}));
		}

		public OperationResult<bool> DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
		{
			return (OperationResult<bool>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceSetPassword(coreConfig, devicePath, password, deviceUser);
			}));
		}

		public OperationResult<bool> DeviceDatetimeSync(string coreConfig, string devicePath)
		{
			return (OperationResult<bool>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceDatetimeSync(coreConfig, devicePath);
			}));
		}

		public OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceGetInformation(coreConfig, devicePath);
			}));
		}

		public OperationResult<string> DeviceGetSerialList(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceGetSerialList(coreConfig, devicePath);
			}));
		}

		public OperationResult<string> DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceUpdateFirmware(coreConfig, devicePath, fileName);
			}));
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName);
			}));
		}

		public OperationResult<string> DeviceReadConfig(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceReadConfig(coreConfig, devicePath);
			}));
		}

		public OperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceReadEventLog(coreConfig, devicePath);
			}));
		}

		public OperationResult<string> DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch);
			}));
		}

		public OperationResult<string> DeviceCustomFunctionList(string driverUID)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceCustomFunctionList(driverUID);
			}));
		}

		public OperationResult<string> DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName);
			}));
		}

		public OperationResult<string> DeviceGetGuardUsersList(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceGetGuardUsersList(coreConfig, devicePath);
			}));
		}

		public OperationResult<bool> DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
		{
			return (OperationResult<bool>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceSetGuardUsersList(coreConfig, devicePath, users);
			}));
		}

		public OperationResult<string> DeviceGetMDS5Data(string coreConfig, string devicePath)
		{
			return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
			{
				return WatcherManager.Current.AdministratorClient.DeviceGetMDS5Data(coreConfig, devicePath);
			}));
		}
    }
}