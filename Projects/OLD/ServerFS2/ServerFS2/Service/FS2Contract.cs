using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
				var hasCancelled = false;
				foreach (var cancellationTokenSource in CancellationTokenSources)
				{
					if (cancellationTokenSource.Token.IsCancellationRequested)
					{
						hasCancelled = true;	
					}
				}
				if (hasCancelled)
				{
					CancellationTokenSources.Clear();
					throw new AggregateException("Операция отменена");
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

		public OperationResult AddToIgnoreList(List<Guid> deviceUIDs, string userName)
		{
			return SafeCall(() =>
			{
				var devices = new List<Device>();
				foreach(var deviceUID in deviceUIDs)
				{
					devices.Add(FindDevice(deviceUID));
				}
				MainManager.AddToIgnoreList(devices, GetUserName(userName));
			}, "AddToIgnoreList");
		}

		public OperationResult RemoveFromIgnoreList(List<Guid> deviceUIDs, string userName)
		{
			return SafeCall(() =>
			{
				var devices = new List<Device>();
				foreach (var deviceUID in deviceUIDs)
				{
					devices.Add(FindDevice(deviceUID));
				}
				MainManager.RemoveFromIgnoreList(devices, GetUserName(userName));
			}, "RemoveFromIgnoreList");
		}

		public OperationResult SetZoneGuard(Guid zoneUID, string userName)
		{
			var zone = FindZone(zoneUID);
			return SafeCall(() =>
			{
				MainManager.SetZoneGuard(zone, GetUserName(userName));
			}, "SetZoneGuard");
		}

		public OperationResult UnSetZoneGuard(Guid zoneUID, string userName)
		{
			var zone = FindZone(zoneUID);
			return SafeCall(() =>
			{
				MainManager.UnSetZoneGuard(zone, GetUserName(userName));
			}, "UnSetZoneGuard");
		}

		public OperationResult SetDeviceGuard(Guid deviceUID, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCall(() =>
			{
				MainManager.SetDeviceGuard(device, GetUserName(userName));
			}, "SetDeviceGuard");
		}

		public OperationResult UnSetDeviceGuard(Guid deviceUID, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCall(() =>
			{
				MainManager.UnSetDeviceGuard(device, GetUserName(userName));
			}, "UnSetDeviceGuard");
		}

		public OperationResult ResetStates(List<PanelResetItem> panelResetItems, string userName)
		{
			return SafeCall(() =>
			{
				MainManager.ResetStates(panelResetItems, GetUserName(userName));
			}, "ResetStates");
		}

		public OperationResult ExecuteCommand(Guid deviceUID, string methodName, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCall(() =>
			{
				MainManager.ExecuteCommand(device, methodName, GetUserName(userName));
			}, "ExecuteCommand");
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Administrator
		public OperationResult SetNewConfiguration(DeviceConfiguration deviceConfiguration, string userName)
		{
			return SafeCall(() =>
			{
				MainManager.SetNewConfiguration(deviceConfiguration, GetUserName(userName));
			}, "SetNewConfiguration");
		}

		public OperationResult<bool> DeviceWriteConfiguration(Guid deviceUID, bool isUSB, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<bool>(() =>
			{
				return MainManager.DeviceWriteConfiguration(device, isUSB, GetUserName(userName));
			}, "DeviceWriteConfiguration", device, true);
		}

		public OperationResult<List<Guid>> DeviceWriteAllConfiguration(List<Guid> deviceUIDs, string userName)
		{
			return SafeCallWithMonitoringSuspending<List<Guid>>(() =>
			{
				return MainManager.DeviceWriteAllConfiguration(deviceUIDs, GetUserName(userName));
			}, "DeviceWriteAllConfiguration", null, true);
		}

		public OperationResult DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceSetPassword(device, isUSB, devicePasswordType, password, userName);
			}, "DeviceSetPassword", device, false);
		}

		public OperationResult DeviceDatetimeSync(Guid deviceUID, bool isUSB, string userName)
		{
			var device = FindDevice(deviceUID);
			return TaskSafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceDatetimeSync(device, isUSB, userName);
			}, "DeviceDatetimeSync", device, false);
		}

		public OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetInformation(device, isUSB, userName);
			}, "DeviceGetInformation", device, false);
		}

		public OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCall<List<string>>(() =>
			{
				return MainManager.DeviceGetSerialList(device, userName);
			}, "DeviceGetSerialList");
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceVerifyFirmwareVersion(device, isUSB, fileName);
			}, "DeviceVerifyFirmwareVersion", device, false);
		}

		public OperationResult DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceUpdateFirmware(device, isUSB, userName);
			}, "DeviceUpdateFirmware", device, true);
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfiguration(Guid deviceUID, bool isUSB, string userName)
		{
			var device = FindDevice(deviceUID);
			return TaskSafeCallWithMonitoringSuspending<DeviceConfiguration>(() =>
			{
				var deviceConfiguration = MainManager.DeviceReadConfiguration(device, isUSB, userName);
				if (deviceConfiguration == null)
				{
					throw new FS2Exception("Ошибка при получении конфигурации");
				}
				return deviceConfiguration;
			}, "DeviceReadConfiguration", device, false);
		}

		public OperationResult<FS2JournalItemsCollection> DeviceReadJournal(Guid deviceUID, bool isUSB, string userName)
		{
			var device = FindDevice(deviceUID);
			return TaskSafeCallWithMonitoringSuspending<FS2JournalItemsCollection>(() =>
			{
				return MainManager.DeviceReadJournal(device, isUSB, userName);
			}, "DeviceReadJournal", device, false);
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<DeviceConfiguration>(() =>
			{
				CustomMessageJournalHelper.Add("Автопоиск устройств", userName, device);
				return MainManager.DeviceAutoDetectChildren(device, fastSearch, userName);
			}, "DeviceAutoDetectChildren", device, false);
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceGetCustomFunctions(DriverType driverType)
		{
			return SafeCall<List<DeviceCustomFunction>>(() =>
			{
				return MainManager.DeviceGetCustomFunctions(driverType);
			}, "DeviceGetCustomFunctions");
		}

		public OperationResult DeviceExecuteCustomFunction(Guid deviceUID, bool isUSB, string functionName, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceExecuteCustomFunction(device, isUSB, functionName, userName);
			}, "DeviceExecuteCustomFunction", device, false);
		}

		public OperationResult<List<GuardUser>> DeviceGetGuardUsers(Guid deviceUID, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<List<GuardUser>>(() =>
			{
				return MainManager.DeviceGetGuardUsers(device, userName);
			}, "DeviceGetGuardUsers", device, false);
		}

		public OperationResult DeviceSetGuardUsers(Guid deviceUID, List<GuardUser> guardUsers, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.DeviceSetGuardUsers(device, USBManager.IsUsbDevice(device), guardUsers, userName);
			}, "DeviceSetGuardUsers", device, false);
		}

		public OperationResult<string> DeviceGetMDS5Data(Guid deviceUID, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetMDS5Data(device, userName);
			}, "DeviceGetMDS5Data", device, false);
		}

		public OperationResult SetAuParameters(Guid deviceUID, List<Property> properties, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending(() =>
			{
				MainManager.SetAuParameters(device, properties, userName);
			}, "SetAuParameters", device, false);
		}

		public OperationResult<List<Property>> GetAuParameters(Guid deviceUID, string userName)
		{
			var device = FindDevice(deviceUID);
			return SafeCallWithMonitoringSuspending<List<Property>>(() =>
			{
				return MainManager.GetAuParameters(device, userName);
			}, "GetAuParameters", device, false);
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

		public OperationResult AddJournalItems(List<FS2JournalItem> journalItems)
		{
			return SafeCall(() =>
			{
				ServerJournalHelper.AddJournalItems(journalItems);
			}, "AddJournalItems");
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

		Zone FindZone(Guid zoneUID)
		{
			var zone = ConfigurationManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone == null)
				throw new FS2Exception("Невозможно выполнить операцию с зоной, отсутствующей в конфигурации");
			return zone;
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

				MainManager.SuspendMonitoring(device);
				//if (stopMonitoring)
				//{
				//    MainManager.StopMonitoring(device);
				//}
				//else
				//{
				//    MainManager.SuspendMonitoring(device);
				//}
				return SafeCall<T>(func, methodName);
			}
			catch(Exception e)
			{
				return new OperationResult<T>(e.Message);
			}
			finally
			{
				Task.Factory.StartNew(() =>
				{
					if (stopMonitoring)
					{
						MainManager.ResumeMonitoring(device);
						MainManager.StopMonitoring(device);
						MainManager.StartMonitoring(device);
					}
					else
					{
						MainManager.ResumeMonitoring(device);
					}
				});
			}
		}

		OperationResult SafeCallWithMonitoringSuspending(Action action, string methodName, Device device, bool stopMonitoring)
		{
			try
			{
				if (device.Driver.DriverType == DriverType.Computer)
					device = null;

				MainManager.SuspendMonitoring(device);
				//if (stopMonitoring)
				//{
				//    MainManager.StopMonitoring(device);
				//}
				//else
				//{
				//    MainManager.SuspendMonitoring(device);
				//}
				return SafeCall(action, methodName);
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			finally
			{
				Task.Factory.StartNew(() =>
				{
					if (stopMonitoring)
					{
						MainManager.ResumeMonitoring(device);
						MainManager.StopMonitoring(device);
						MainManager.StartMonitoring(device);
					}
					else
					{
						MainManager.ResumeMonitoring(device);
					}
				});
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

		public string GetUserName(string userName)
		{
			try
			{
				string userIp = "127.0.0.1";
				try
				{
					if (OperationContext.Current.IncomingMessageProperties.Keys.Contains(RemoteEndpointMessageProperty.Name))
					{
						var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
						userIp = endpoint.Address;
					}
				}
				catch { }

				var addressList = Dns.GetHostEntry("localhost").AddressList;
				if (addressList.Any(ip => ip.ToString() == userIp))
					userIp = "localhost";

				return userName + " (" + userIp + ")";
			}
			catch (Exception e)
			{
				Logger.Error(e, "FSContract.GetUserName");
			}
			return null;
		}
		#endregion
	}
}