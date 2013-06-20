using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Journal;
using ServerFS2.Processor;

namespace ServerFS2.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class FS2Contract : IFS2Contract
	{
		#region Cancelisation
		bool IsFirstCall = true;
		public static List<CancellationTokenSource> CancellationTokenSources { get; private set; }

		public static void CheckCancellationAndNotify(string message)
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo(message) });
			CheckCancellationRequested();
		}

		public static void CheckCancellationRequested()
		{
			if (CancellationTokenSources != null)
			{
				foreach (var cancellationTokenSource in CancellationTokenSources)
				{
					if (cancellationTokenSource.Token.IsCancellationRequested)
					{
						throw new AggregateException("Операция отменена");
					}
					cancellationTokenSource.Token.ThrowIfCancellationRequested();
				}
			}
		}

		public FS2Contract()
		{
			CancellationTokenSources = new List<CancellationTokenSource>();
		}
		#endregion

		#region Main
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
						if (IsFirstCall)
							clientInfo.PollWaitEvent.WaitOne(TimeSpan.FromSeconds(1));
						//else
						//    clientInfo.PollWaitEvent.WaitOne(TimeSpan.FromMinutes(1));
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
			try
			{
				foreach (var cancellationTokenSource in CancellationTokenSources)
				{
					cancellationTokenSource.Cancel();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FS2Contract.CancelProgress");
			}
		}
		#endregion

		#region Common
		public OperationResult<DeviceConfiguration> GetDeviceConfiguration()
		{
			return SafeCallWithMonitoringSuspending<DeviceConfiguration>(() =>
			{
				return ConfigurationCash.DeviceConfiguration;
			}, "GetDeviceConfiguration");
		}

		public OperationResult<DriversConfiguration> GetDriversConfiguration()
		{
			return SafeCallWithMonitoringSuspending<DriversConfiguration>(() =>
			{
				return ConfigurationCash.DriversConfiguration;
			}, "GetDriversConfiguration");
		}
		#endregion

		#region Monitor
		public OperationResult<List<DeviceState>> GetDeviceStates()
		{
			return SafeCall<List<DeviceState>>(() =>
			{
				return MainManager.GetDeviceStates();
			}, "GetDeviceStates");
		}

		public OperationResult<List<DeviceState>> GetDeviceParameters()
		{
			return SafeCall<List<DeviceState>>(() =>
			{
				return MainManager.GetDeviceParameters();
			}, "GetDeviceParameters");
		}

		public OperationResult<List<ZoneState>> GetZoneStates()
		{
			return SafeCall<List<ZoneState>>(() =>
			{
				return MainManager.GetZoneStates();
			}, "GetZoneStates");
		}

		public OperationResult AddToIgnoreList(List<Guid> deviceUIDs)
		{
			return SafeCall(() =>
			{
				var devices = new List<Device>();
				foreach(var deviceUID in deviceUIDs)
				{
					devices.Add(FindDevice(deviceUID));
				}
				MainManager.AddToIgnoreList(devices);
			}, "AddToIgnoreList");
		}

		public OperationResult RemoveFromIgnoreList(List<Guid> deviceUIDs)
		{
			return SafeCall(() =>
			{
				var devices = new List<Device>();
				foreach (var deviceUID in deviceUIDs)
				{
					devices.Add(FindDevice(deviceUID));
				}
				MainManager.RemoveFromIgnoreList(devices);
			}, "RemoveFromIgnoreList");
		}

		public OperationResult SetZoneGuard(Guid zoneUID)
		{
			return SafeCall(() =>
			{
				MainManager.SetZoneGuard(zoneUID);
			}, "SetZoneGuard");
		}

		public OperationResult UnSetZoneGuard(Guid zoneUID)
		{
			return SafeCall(() =>
			{
				MainManager.UnSetZoneGuard(zoneUID);
			}, "UnSetZoneGuard");
		}

		public OperationResult SetDeviceGuard(Guid deviceUID)
		{
			return SafeCall(() =>
			{
				MainManager.SetDeviceGuard(deviceUID);
			}, "SetDeviceGuard");
		}

		public OperationResult UnSetDeviceGuard(Guid deviceUID)
		{
			return SafeCall(() =>
			{
				MainManager.UnSetDeviceGuard(deviceUID);
			}, "UnSetDeviceGuard");
		}

		public OperationResult ResetStates(List<PanelResetItem> panelResetItems)
		{
			return SafeCall(() =>
			{
				MainManager.ResetStates(panelResetItems);
			}, "ResetStates");
		}

		public OperationResult ExecuteCommand(Guid deviceUID, string methodName)
		{
			return SafeCall(() =>
			{
				MainManager.ExecuteCommand(deviceUID, methodName);
			}, "ExecuteCommand");
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Administrator
		public OperationResult SetNewConfig(DeviceConfiguration deviceConfiguration)
		{
			return SafeCall(() =>
			{
				MainManager.SetNewConfig(deviceConfiguration);
			}, "SetNewConfig");
		}

		public OperationResult DeviceWriteConfig(Guid deviceUID, bool isUSB)
		{
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceWriteConfig(FindDevice(deviceUID), isUSB);
			}, "DeviceWriteConfig");
		}

		public OperationResult DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceSetPassword(FindDevice(deviceUID), isUSB, devicePasswordType, password);
			}, "DeviceSetPassword");
		}

		public OperationResult DeviceDatetimeSync(Guid deviceUID, bool isUSB)
		{
			return TaskSafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceDatetimeSync(FindDevice(deviceUID), isUSB);
			}, "DeviceDatetimeSync");
		}

		public OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB)
		{
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetInformation(FindDevice(deviceUID), isUSB);
			}, "DeviceGetInformation");
		}

		public OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID)
		{
			return SafeCallWithMonitoringSuspending<List<string>>(() =>
			{
				return MainManager.DeviceGetSerialList(FindDevice(deviceUID));
			}, "DeviceGetSerialList", true);
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceVerifyFirmwareVersion(FindDevice(deviceUID), isUSB, fileName);
			}, "DeviceVerifyFirmwareVersion");
		}

		public OperationResult DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceUpdateFirmware(FindDevice(deviceUID), isUSB, fileName);
			}, "DeviceUpdateFirmware", true);
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfig(Guid deviceUID, bool isUSB)
		{
			return TaskSafeCallWithMonitoringSuspending<DeviceConfiguration>(() =>
			{
				return MainManager.DeviceReadConfig(FindDevice(deviceUID), isUSB);
			}, "DeviceReadConfig");
		}

		public OperationResult<List<FS2JournalItem>> DeviceReadEventLog(Guid deviceUID, bool isUSB)
		{
			return TaskSafeCallWithMonitoringSuspending<List<FS2JournalItem>>(() =>
			{
				return MainManager.DeviceReadEventLog(FindDevice(deviceUID), isUSB);
			}, "DeviceReadEventLog");
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch)
		{
			return SafeCallWithMonitoringSuspending<DeviceConfiguration>(() =>
			{
				return MainManager.DeviceAutoDetectChildren(FindDevice(deviceUID), fastSearch);
			}, "DeviceAutoDetectChildren");
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(DriverType driverType)
		{
			return SafeCallWithMonitoringSuspending<List<DeviceCustomFunction>>(() =>
			{
				return MainManager.DeviceCustomFunctionList(driverType);
			}, "DeviceCustomFunctionList");
		}

		public OperationResult DeviceCustomFunctionExecute(Guid deviceUID, bool isUSB, string functionName)
		{
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceCustomFunctionExecute(FindDevice(deviceUID), isUSB, functionName);
			}, "DeviceCustomFunctionExecute");
		}

		public OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID)
		{
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetGuardUsersList(FindDevice(deviceUID));
			}, "DeviceGetGuardUsersList");
		}

		public OperationResult DeviceSetGuardUsersList(Guid deviceUID, string users)
		{
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceSetGuardUsersList(FindDevice(deviceUID), users);
			}, "DeviceSetGuardUsersList");
		}

		public OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetMDS5Data(FindDevice(deviceUID));
			}, "DeviceGetMDS5Data");
		}

		public OperationResult SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			return SafeCall(() =>
			{
				MainManager.SetConfigurationParameters(FindDevice(deviceUID), properties);
			}, "SetConfigurationParameters");
		}

		public OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID)
		{
			return SafeCall<List<Property>>(() =>
			{
				return MainManager.GetConfigurationParameters(FindDevice(deviceUID));
			}, "GetConfigurationParameters");
		}
		#endregion

		#region Journal
		public OperationResult<List<FS2JournalItem>> GetFilteredJournal(JournalFilter journalFilter)
		{
			return SafeCall<List<FS2JournalItem>>(() =>
			{
				return ServerJournalHelper.GetFilteredJournal(journalFilter);
			}, "GetFilteredJournal");
		}

		public OperationResult<List<FS2JournalItem>> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return SafeCall<List<FS2JournalItem>>(() =>
			{
				return ServerJournalHelper.GetFilteredArchive(archiveFilter);
			}, "GetFilteredArchive");
		}

		public OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return SafeCall(() =>
			{
				ServerJournalHelper.BeginGetFilteredArchive(archiveFilter);
			}, "BeginGetFilteredArchive");
		}

		public OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
		{
			return SafeCall<List<JournalDescriptionItem>>(() =>
			{
				return ServerJournalHelper.GetDistinctDescriptions();
			}, "GetDistinctDescriptions");
		}

		public OperationResult<DateTime> GetArchiveStartDate()
		{
			return SafeCall<DateTime>(() =>
			{
				return ServerJournalHelper.GetArchiveStartDate();
			}, "GetArchiveStartDate");
		}

		public OperationResult AddJournalRecords(List<FS2JournalItem> journalItems)
		{
			return SafeCall(() =>
			{
				//return ServerJournalHelper.AddJournalRecords(journalItems);
			}, "AddJournalRecords");
		}
		#endregion

		#region Helpers
		Device FindDevice(Guid deviceUID)
		{
			var device = ConfigurationManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device == null)
				throw new FS2Exception("Невозможно выполнить операцию с устройством, отсутствующим в конфигурации");
			return device;
		}

		OperationResult<T> TaskSafeCallWithMonitoringSuspending<T>(Func<T> func, string methodName)
		{
			try
			{
				var cancellationTokenSource = new CancellationTokenSource();
				CancellationTokenSources.Add(cancellationTokenSource);
				var cancellationToken = cancellationTokenSource.Token;
				var task = Task.Factory.StartNew(
					() =>
						{
						return SafeCallWithMonitoringSuspending<T>(func, methodName);
						},
					cancellationToken);
				try
				{
					task.Wait();
				}
				catch (AggregateException ae)
				{
					return new OperationResult<T>("Операция отменена");
				}
				catch (Exception ex)
				{
					return new OperationResult<T>(ex.Message);
				}
				var result = task.Result;
				CancellationTokenSources.Remove(cancellationTokenSource);
				task.Dispose();
				cancellationTokenSource.Dispose();
				return result;
			}
			catch (Exception e)
			{
				return new OperationResult<T>(e.Message);
			}
		}

		OperationResult TaskSafeCallWithMonitoringSuspending(Action action, string methodName)
		{
			try
			{
				Trace.WriteLine("TaskSafeCallWithMonitoringSuspending");
				var cancellationTokenSource = new CancellationTokenSource();
				CancellationTokenSources.Add(cancellationTokenSource);
				var cancellationToken = cancellationTokenSource.Token;
				var task = Task.Factory.StartNew(
					() =>
					{
						return SafeCallWithMonitoringSuspending(action, methodName);
					},
					cancellationToken);
				try
				{
					task.Wait();
				}
				catch (AggregateException ae)
				{
					return new OperationResult("Операция отменена");
				}
				catch (Exception ex)
				{
					return new OperationResult(ex.Message);
				}
				var result = task.Result;
				CancellationTokenSources.Remove(cancellationTokenSource);
				task.Dispose();
				cancellationTokenSource.Dispose();
				return result;
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		OperationResult<T> SafeCallWithMonitoringSuspending<T>(Func<T> func, string methodName, bool stopMonitoring = false)
		{
			try
			{
				if (stopMonitoring)
				{
					MainManager.StopMonitoring();
				}
				else
				{
					MainManager.SuspendMonitoring();
				}
				return SafeCall<T>(func, methodName);
			}
			catch(Exception e)
			{
				return new OperationResult<T>(e.Message);
			}
			finally
			{
				if (stopMonitoring)
				{
					MainManager.StartMonitoring();
				}
				else
				{
					MainManager.ResumeMonitoring();
				}
			}
		}

		OperationResult SafeCallWithMonitoringSuspending(Action action, string methodName, bool stopMonitoring = false)
		{
			try
			{
				if (stopMonitoring)
				{
					MainManager.StopMonitoring();
				}
				else
				{
					MainManager.SuspendMonitoring();
				}
				return SafeCall(action, methodName);
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			finally
			{
				if (stopMonitoring)
				{
					MainManager.StartMonitoring();
				}
				else
				{
					MainManager.ResumeMonitoring();
				}
			}
		}

		OperationResult<T> SafeCall<T>(Func<T> func, string methodName)
		{
			IsFirstCall = false;
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

		OperationResult SafeCall(Action action, string methodName)
		{
			IsFirstCall = false;
			var resultData = new OperationResult();
			try
			{
				action();
				resultData.HasError = false;
				resultData.Error = null;
			}
			catch (Exception e)
			{
				string exceptionText = "Exception " + e.Message + " при вызове " + methodName;
				Logger.Error(e, exceptionText);
				resultData.HasError = true;
				resultData.Error = e.Message;
			}
			return resultData;
		}
		#endregion
	}
}