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

		public static OperationResult<DeviceConfiguration> AutoDetectDevice(Guid deviceUID, bool fastSearch)
		{
			return FiresecDriver.DeviceAutoDetectChildren(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID, fastSearch);
		}

		public static OperationResult<DeviceConfiguration> DeviceReadConfiguration(Guid deviceUID, bool isUsb)
		{
			return FiresecDriver.DeviceReadConfiguration(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
		}

		public static OperationResult<bool> DeviceWriteConfiguration(Guid deviceUID, bool isUsb)
		{
			return FiresecDriver.DeviceWriteConfiguration(FiresecConfiguration.DeviceConfiguration, deviceUID);
		}

		public static OperationResult<string> ReadDeviceJournal(Guid deviceUID, bool isUsb)
		{
			return FiresecDriver.DeviceReadEventLog(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
		}

		public static OperationResult<bool> SynchronizeDevice(Guid deviceUID, bool isUsb)
		{
			return FiresecDriver.DeviceDatetimeSync(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
		}

		public static OperationResult<string> DeviceUpdateFirmware(Guid deviceUID, bool isUsb, byte[] bytes, string fileName)
		{
			return FiresecDriver.DeviceUpdateFirmware(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID, bytes, fileName);
		}

		public static OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUsb, byte[] bytes, string fileName)
		{
			return FiresecDriver.DeviceVerifyFirmwareVersion(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID, bytes, fileName);
		}

		public static OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUsb)
		{
			return FiresecDriver.DeviceGetInformation(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
		}

		public static OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID)
		{
			return FiresecDriver.DeviceGetSerialList(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID);
		}

		public static OperationResult<bool> SetPassword(Guid deviceUID, bool isUsb, DevicePasswordType devicePasswordType, string password)
		{
			return FiresecDriver.DeviceSetPassword(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID, devicePasswordType, password);
		}

		public static OperationResult<string> DeviceCustomFunctionExecute(Guid deviceUID, string functionName)
		{
			return FiresecDriver.DeviceCustomFunctionExecute(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID, functionName);
		}

		public static OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID)
		{
			return FiresecDriver.DeviceGetGuardUsersList(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID);
		}

		public static OperationResult<bool> DeviceSetGuardUsersList(Guid deviceUID, string users)
		{
			return FiresecDriver.DeviceSetGuardUsersList(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID, users);
		}

		public static OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			return FiresecDriver.DeviceGetMDS5Data(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID);
		}
	}
}