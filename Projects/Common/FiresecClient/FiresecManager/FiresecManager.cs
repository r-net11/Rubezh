using System;
using System.Collections.Generic;
using System.Linq;
using Firesec;
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
			string clientCallbackAddress;
			if (serverAddress.StartsWith("net.pipe:"))
			{
				clientCallbackAddress = "net.pipe://127.0.0.1/FiresecCallbackService_" + clientType.ToString() + "/";
			}
			else
			{
				clientCallbackAddress = CallbackAddressHelper.GetFreeClientCallbackAddress();
			}
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
					FiresecService.Dispose();
				}
				FiresecCallbackServiceManager.Close();
			}
			else
			{
				//Logger.Info("FiresecManager.Disconnect IsDisconnected=true");
			}
			IsDisconnected = true;
		}

		public static OperationResult<DeviceConfiguration> AutoDetectDevice(Device device, bool fastSearch)
		{
			return FiresecDriver.DeviceAutoDetectChildren(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID, fastSearch);
		}

		public static OperationResult<DeviceConfiguration> DeviceReadConfiguration(Device device, bool isUsb)
		{
			return FiresecDriver.DeviceReadConfiguration(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID);
		}

		public static OperationResult<bool> DeviceWriteConfiguration(Device device, bool isUsb)
		{
			return FiresecDriver.DeviceWriteConfiguration(FiresecConfiguration.DeviceConfiguration, device.UID);
		}

		public static OperationResult<string> ReadDeviceJournal(Device device, bool isUsb)
		{
			return FiresecDriver.DeviceReadEventLog(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID);
		}

		public static OperationResult<bool> SynchronizeDevice(Device device, bool isUsb)
		{
			return FiresecDriver.DeviceDatetimeSync(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID);
		}

		public static OperationResult<string> DeviceUpdateFirmware(Device device, bool isUsb, string fileName)
		{
			return FiresecDriver.DeviceUpdateFirmware(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID, fileName);
		}

		public static OperationResult<string> DeviceVerifyFirmwareVersion(Device device, bool isUsb, string fileName)
		{
			return FiresecDriver.DeviceVerifyFirmwareVersion(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID, fileName);
		}

		public static OperationResult<string> DeviceGetInformation(Device device, bool isUsb)
		{
			return FiresecDriver.DeviceGetInformation(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID);
		}

		public static OperationResult<List<string>> DeviceGetSerialList(Device device)
		{
			return FiresecDriver.DeviceGetSerialList(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID);
		}

		public static OperationResult<bool> SetPassword(Device device, bool isUsb, DevicePasswordType devicePasswordType, string password)
		{
			return FiresecDriver.DeviceSetPassword(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID, devicePasswordType, password);
		}

		public static OperationResult<string> DeviceCustomFunctionExecute(Device device, string functionName)
		{
			return FiresecDriver.DeviceCustomFunctionExecute(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID, functionName);
		}

		public static OperationResult<string> DeviceGetGuardUsersList(Device device)
		{
			return FiresecDriver.DeviceGetGuardUsersList(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID);
		}

		public static OperationResult<bool> DeviceSetGuardUsersList(Device device, string users)
		{
			return FiresecDriver.DeviceSetGuardUsersList(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID, users);
		}

		public static OperationResult<string> DeviceGetMDS5Data(Device device)
		{
			return FiresecDriver.DeviceGetMDS5Data(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID);
		}
	}
}