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
			CallbackManager.AddProgress(new FS2ProgressInfo(message));
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
			return SafeCall<DeviceConfiguration>(() =>
			{
				return ConfigurationCash.DeviceConfiguration;
			}, "GetDeviceConfiguration");
		}

		public OperationResult<DriversConfiguration> GetDriversConfiguration()
		{
			return SafeCall<DriversConfiguration>(() =>
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
		public OperationResult SetNewConfiguration(DeviceConfiguration deviceConfiguration)
		{
			return SafeCall(() =>
			{
				MainManager.SetNewConfiguration(deviceConfiguration);
			}, "SetNewConfiguration");
		}

		public OperationResult DeviceWriteConfiguration(Guid deviceUID, bool isUSB)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceWriteConfiguration(device, isUSB);
			}, "DeviceWriteConfiguration", device, true);
		}

		public OperationResult DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceSetPassword(device, isUSB, devicePasswordType, password);
			}, "DeviceSetPassword", device, false);
		}

		public OperationResult DeviceDatetimeSync(Guid deviceUID, bool isUSB)
		{
			var device = FindDevice(deviceUID);
			return TaskSafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceDatetimeSync(device, isUSB);
			}, "DeviceDatetimeSync", device, false);
		}

		public OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetInformation(device, isUSB);
			}, "DeviceGetInformation", device, false);
		}

		public OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<List<string>>(() =>
			{
				return MainManager.DeviceGetSerialList(device);
			}, "DeviceGetSerialList", device, true);
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceVerifyFirmwareVersion(device, isUSB, fileName);
			}, "DeviceVerifyFirmwareVersion", device, false);
		}

		public OperationResult DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceUpdateFirmware(device, isUSB, fileName);
			}, "DeviceUpdateFirmware", device, true);
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfiguration(Guid deviceUID, bool isUSB)
		{
			var device = FindDevice(deviceUID);
			return TaskSafeCallWithMonitoringSuspending<DeviceConfiguration>(() =>
			{
				return MainManager.DeviceReadConfiguration(device, isUSB);
			}, "DeviceReadConfiguration", device, false);
		}

		public OperationResult<List<FS2JournalItem>> DeviceReadJournal(Guid deviceUID, bool isUSB)
		{
			var device = FindDevice(deviceUID);
			return TaskSafeCallWithMonitoringSuspending<List<FS2JournalItem>>(() =>
			{
				return MainManager.DeviceReadJournal(device, isUSB);
			}, "DeviceReadJournal", device, false);
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<DeviceConfiguration>(() =>
			{
				return MainManager.DeviceAutoDetectChildren(device, fastSearch);
			}, "DeviceAutoDetectChildren", device, false);
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceGetCustomFunctions(DriverType driverType)
		{
			return SafeCall<List<DeviceCustomFunction>>(() =>
			{
				return MainManager.DeviceGetCustomFunctions(driverType);
			}, "DeviceGetCustomFunctions");
		}

		public OperationResult DeviceExecuteCustomFunction(Guid deviceUID, bool isUSB, string functionName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceExecuteCustomFunction(device, isUSB, functionName);
			}, "DeviceExecuteCustomFunction", device, false);
		}

		public OperationResult<string> DeviceGetGuardUsers(Guid deviceUID)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetGuardUsers(device);
			}, "DeviceGetGuardUsers", device, false);
		}

		public OperationResult DeviceSetGuardUsers(Guid deviceUID, string users)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceSetGuardUsers(device, users);
			}, "DeviceSetGuardUsers", device, false);
		}

		public OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetMDS5Data(device);
			}, "DeviceGetMDS5Data", device, false);
		}

		public OperationResult SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.SetConfigurationParameters(device, properties);
			}, "SetConfigurationParameters", device, false);
		}

		public OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<List<Property>>(() =>
			{
				return MainManager.GetConfigurationParameters(device);
			}, "GetConfigurationParameters", device, false);
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

		OperationResult<T> TaskSafeCallWithMonitoringSuspending<T>(Func<T> func, string methodName, Device device, bool stopMonitoring)
		{
			try
			{
				var cancellationTokenSource = new CancellationTokenSource();
				CancellationTokenSources.Add(cancellationTokenSource);
				var cancellationToken = cancellationTokenSource.Token;
				var task = Task.Factory.StartNew(
					() =>
						{
						return SafeCallWithMonitoringSuspending<T>(func, methodName, device, stopMonitoring);
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

		OperationResult TaskSafeCallWithMonitoringSuspending(Action action, string methodName, Device device, bool stopMonitoring)
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
						return SafeCallWithMonitoringSuspending(action, methodName, device, stopMonitoring);
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

		OperationResult<T> SafeCallWithMonitoringSuspending<T>(Func<T> func, string methodName, Device device, bool stopMonitoring)
		{
			try
			{
				if (device.Driver.DriverType == DriverType.Computer)
					device = null;

				if (stopMonitoring)
				{
					MainManager.StopMonitoring(device);
				}
				else
				{
					MainManager.SuspendMonitoring(device);
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
					MainManager.StartMonitoring(device);
				}
				else
				{
					MainManager.ResumeMonitoring(device);
				}
			}
		}

		OperationResult SafeCallWithMonitoringSuspending(Action action, string methodName, Device device, bool stopMonitoring)
		{
			try
			{
				if (device.Driver.DriverType == DriverType.Computer)
					device = null;

				if (stopMonitoring)
				{
					MainManager.StopMonitoring(device);
				}
				else
				{
					MainManager.SuspendMonitoring(device);
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
					MainManager.StartMonitoring(device);
				}
				else
				{
					MainManager.ResumeMonitoring(device);
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