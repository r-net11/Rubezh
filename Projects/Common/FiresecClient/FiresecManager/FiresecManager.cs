using System;
using System.Collections.Generic;
using System.Linq;
using Common;
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
			try
			{
				ClientCredentials = new ClientCredentials()
				{
					UserName = login,
					Password = password,
					ClientType = clientType,
					ClientUID = FiresecServiceFactory.UID
				};

				var operationResult = new OperationResult<bool>();
				for (int i = 0; i < 3; i++)
				{
					FiresecService = new SafeFiresecService(serverAddress);
					operationResult = FiresecService.Connect(FiresecServiceFactory.UID, ClientCredentials, true);
					if (!operationResult.HasError)
						break;
				}
				if (operationResult.HasError)
				{
					return operationResult.Error;
				}

				_userLogin = login;
				return null;
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.Connect");
				return e.Message;
			}
		}

		public static string Reconnect(string login, string password)
		{
			try
			{
				var operationResult = FiresecService.Reconnect(FiresecServiceFactory.UID, login, password);
				if (operationResult.HasError)
				{
					return operationResult.Error;
				}

				var securityConfiguration = FiresecManager.FiresecService.GetSecurityConfiguration();
				if (securityConfiguration != null)
				{
					SecurityConfiguration = securityConfiguration;
				}

				_userLogin = login;
				return null;
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.Reconnect");
				return e.Message;
			}
		}

		public static string GetIP()
		{
			return FiresecService.Ping();
		}

		static string _userLogin;
		public static User CurrentUser
		{
			get { return SecurityConfiguration.Users.FirstOrDefault(x => x.Login == _userLogin); }
		}
		public static bool CheckPermission(PermissionType permissionType)
		{
			try
			{
				if (CurrentUser == null)
					return false;
				return CurrentUser.HasPermission(permissionType);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.CheckPermission");
				return false;
			}
		}

        static object locker = new object();
		public static bool IsDisconnected { get; private set; }
		public static void Disconnect()
		{
			try
			{
                lock (locker)
                {
                    if (!IsDisconnected)
                    {
						if (FSAgent != null)
							FSAgent.Stop();

						if (FS2ClientContract != null)
							FS2ClientContract.Stop();

                        if (FiresecService != null)
                        {
                            FiresecService.Dispose();
                        }
                    }
                    IsDisconnected = true;
                }
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.Disconnect");
			}
		}

		public static void StartPoll(bool mustReactOnCallback)
		{
			try
			{
				FiresecService.StartPoll(mustReactOnCallback);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.StartPoll");
			}
		}

		public static OperationResult<DeviceConfiguration> AutoDetectDevice(Device device, bool fastSearch)
		{
			return SafeOperationCall<DeviceConfiguration>(() =>
				{
					return FiresecDriver.DeviceAutoDetectChildren(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID, fastSearch);
				}, "AutoDetectDevice");
		}

		public static OperationResult<DeviceConfiguration> DeviceReadConfiguration(Device device, bool isUsb)
		{
			return SafeOperationCall<DeviceConfiguration>(() =>
				{
					return FiresecDriver.DeviceReadConfiguration(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID);
				}, "DeviceReadConfiguration");
		}

		public static OperationResult<bool> DeviceWriteConfiguration(Device device, bool isUsb)
		{
			return SafeOperationCall<bool>(() =>
				{
					if (isUsb)
					{
						try
						{
							device.IsAltInterface = true;
							return FiresecDriver.DeviceWriteConfiguration(FiresecConfiguration.DeviceConfiguration, device.UID);
						}
						finally
						{
							device.IsAltInterface = false;
						}
					}
					else
					{
						return FiresecDriver.DeviceWriteConfiguration(FiresecConfiguration.DeviceConfiguration, device.UID);
					}
				}, "DeviceWriteConfiguration");
		}

		public static OperationResult<string> ReadDeviceJournal(Device device, bool isUsb)
		{
			return SafeOperationCall<string>(() =>
				{
					var journalType = 0;
					if (device.Driver.DriverType == DriverType.Rubezh_2OP || device.Driver.DriverType == DriverType.USB_Rubezh_2OP)
						journalType = 2;
					return FiresecDriver.DeviceReadEventLog(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID, journalType);
				}, "ReadDeviceJournal");
		}

		public static OperationResult<bool> SynchronizeDevice(Device device, bool isUsb)
		{
			return SafeOperationCall<bool>(() =>
				{
					return FiresecDriver.DeviceDatetimeSync(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID);
				}, "SynchronizeDevice");
		}

		public static OperationResult<string> DeviceUpdateFirmware(Device device, bool isUsb, string fileName)
		{
			return SafeOperationCall<string>(() =>
				{
					return FiresecDriver.DeviceUpdateFirmware(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID, fileName);
				}, "DeviceUpdateFirmware");
		}

		public static OperationResult<string> DeviceVerifyFirmwareVersion(Device device, bool isUsb, string fileName)
		{
			return SafeOperationCall<string>(() =>
				{
					return FiresecDriver.DeviceVerifyFirmwareVersion(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID, fileName);
				}, "DeviceVerifyFirmwareVersion");
		}

		public static OperationResult<string> DeviceGetInformation(Device device, bool isUsb)
		{
			return SafeOperationCall<string>(() =>
				{
					return FiresecDriver.DeviceGetInformation(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID);
				}, "DeviceGetInformation");
		}

		public static OperationResult<List<string>> DeviceGetSerialList(Device device)
		{
			return SafeOperationCall<List<string>>(() =>
				{
					return FiresecDriver.DeviceGetSerialList(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID);
				}, "DeviceGetSerialList");
		}

		public static OperationResult<bool> SetPassword(Device device, bool isUsb, DevicePasswordType devicePasswordType, string password)
		{
			return SafeOperationCall<bool>(() =>
				{
					return FiresecDriver.DeviceSetPassword(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID, devicePasswordType, password);
				}, "SetPassword");
		}

		public static OperationResult<string> DeviceCustomFunctionExecute(Device device, bool isUsb, string functionName)
		{
			return SafeOperationCall<string>(() =>
				{
					return FiresecDriver.DeviceCustomFunctionExecute(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, isUsb), device.UID, functionName);
				}, "DeviceCustomFunctionExecute");
		}

		public static OperationResult<string> DeviceGetGuardUsersList(Device device)
		{
			return SafeOperationCall<string>(() =>
				{
					return FiresecDriver.DeviceGetGuardUsersList(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID);
				}, "DeviceGetGuardUsersList");
		}

		public static OperationResult<bool> DeviceSetGuardUsersList(Device device, string users)
		{
			return SafeOperationCall<bool>(() =>
				{
					return FiresecDriver.DeviceSetGuardUsersList(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID, users);
				}, "DeviceSetGuardUsersList");
		}

		public static OperationResult<string> DeviceGetMDS5Data(Device device)
		{
			return SafeOperationCall<string>(() =>
				{
					return FiresecDriver.DeviceGetMDS5Data(FiresecConfiguration.DeviceConfiguration.CopyOneBranch(device, false), device.UID);
				}, "DeviceGetMDS5Data");
		}

		static OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> action, string methodName)
		{
			try
			{
				return action();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.SafeOperationCall." + methodName);
				return new OperationResult<T>(e.Message);
			}
		}
	}
}