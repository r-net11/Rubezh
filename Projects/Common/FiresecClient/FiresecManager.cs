using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static ClientCredentials ClientCredentials { get; private set; }
		public static SafeFiresecService FiresecService { get; private set; }

		public static string Connect(ClientType clientType, string serverAddress, string login, string password)
		{
			var clientCallbackAddress = CallbackAddressHelper.GetFreeClientCallbackAddress();
			FiresecCallbackServiceManager.Open(clientCallbackAddress);

			ClientCredentials = new ClientCredentials()
			{
				UserName = login,
				Password = password,
				ClientType = clientType,
				ClientCallbackAddress = clientCallbackAddress,
				ClientUID = Guid.NewGuid()
			};

			FiresecService = new SafeFiresecService(serverAddress);

			var operationResult = FiresecService.Connect(ClientCredentials, true);
			if (operationResult.HasError)
			{
				return operationResult.Error;
			}

			_userLogin = login;
			OnUserChanged();
			return null;
		}

		public static string Reconnect(string login, string password)
		{
			var operationResult = FiresecService.Reconnect(login, password);
			if (operationResult.HasError)
			{
				return operationResult.Error;
			}

			_userLogin = login;
			OnUserChanged();
			return null;
		}

		public static void GetStates()
		{
			DeviceStates = FiresecService.GetStates(false);
			UpdateStates();
			FiresecService.StartPing();
		}

		public static void UpdateStates()
		{
			foreach (var deviceState in DeviceStates.DeviceStates)
			{
				deviceState.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceState.UID);
				if (deviceState.Device == null)
				{
					MessageBox.Show("Ошибка при сопоставлении устройства с его состоянием");
					continue;
				}

				foreach (var state in deviceState.States)
				{
					state.DriverState = deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == state.Code);
				}

				foreach (var parentState in deviceState.ParentStates)
				{
					parentState.ParentDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == parentState.ParentDeviceId);
					if (parentState.ParentDevice != null)
						parentState.DriverState = parentState.ParentDevice.Driver.States.FirstOrDefault(x => x.Code == parentState.Code);
				}

				foreach (var childState in deviceState.ChildStates)
				{
					childState.ChildDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == childState.ChildDeviceId);
					if (childState.ChildDevice != null)
						childState.DriverState = childState.ChildDevice.Driver.States.FirstOrDefault(x => x.Code == childState.Code);
				}
			}
		}

		public static event Action UserChanged;
		static void OnUserChanged()
		{
			if (UserChanged != null)
				UserChanged();
		}
		static string _userLogin;
		public static User CurrentUser
		{
			get { return SecurityConfiguration.Users.FirstOrDefault(x => x.Login == _userLogin); }
		}

		static bool IsDisconnected = false;
		public static void Disconnect()
		{
			if (!IsDisconnected)
			{
				if (FiresecService != null)
				{
					Logger.Info("FiresecManager.Disconnect");
					FiresecService.Dispose();
				}
				FiresecCallbackServiceManager.Close();
			}
			else
			{
				Logger.Info("FiresecManager.Disconnect IsDisconnected=true");
			}
			IsDisconnected = true;
		}

		public static OperationResult<DeviceConfiguration> AutoDetectDevice(Guid deviceUID, bool fastSearch)
		{
			return FiresecService.DeviceAutoDetectChildren(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID, fastSearch);
		}

		public static OperationResult<DeviceConfiguration> DeviceReadConfiguration(Guid deviceUID, bool isUsb)
		{
			return FiresecService.DeviceReadConfiguration(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
		}

		public static OperationResult<bool> DeviceWriteConfiguration(Guid deviceUID, bool isUsb)
		{
			return FiresecService.DeviceWriteConfiguration(DeviceConfiguration, deviceUID);
		}

		public static OperationResult<bool> WriteAllDeviceConfiguration()
		{
			return FiresecService.DeviceWriteAllConfiguration(DeviceConfiguration);
		}

		public static OperationResult<string> ReadDeviceJournal(Guid deviceUID, bool isUsb)
		{
			return FiresecService.DeviceReadEventLog(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
		}

		public static OperationResult<bool> SynchronizeDevice(Guid deviceUID, bool isUsb)
		{
			return FiresecService.DeviceDatetimeSync(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
		}

		public static OperationResult<string> DeviceUpdateFirmware(Guid deviceUID, bool isUsb, byte[] bytes, string fileName)
		{
			return FiresecService.DeviceUpdateFirmware(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID, bytes, fileName);
		}

		public static OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUsb, byte[] bytes, string fileName)
		{
			return FiresecService.DeviceVerifyFirmwareVersion(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID, bytes, fileName);
		}

		public static OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUsb)
		{
			return FiresecService.DeviceGetInformation(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
		}

		public static OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID)
		{
			return FiresecService.DeviceGetSerialList(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID);
		}

		public static OperationResult<bool> SetPassword(Guid deviceUID, bool isUsb, DevicePasswordType devicePasswordType, string password)
		{
			return FiresecService.DeviceSetPassword(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID, devicePasswordType, password);
		}

		public static OperationResult<string> DeviceCustomFunctionExecute(Guid deviceUID, string functionName)
		{
			return FiresecService.DeviceCustomFunctionExecute(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID, functionName);
		}

		public static OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID)
		{
			return FiresecService.DeviceGetGuardUsersList(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID);
		}

		public static OperationResult<bool> DeviceSetGuardUsersList(Guid deviceUID, string users)
		{
			return FiresecService.DeviceSetGuardUsersList(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID, users);
		}

		public static OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			return FiresecService.DeviceGetMDS5Data(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID);
		}
	}
}