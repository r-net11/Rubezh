using Common;
using Infrastructure.Common.License;
using RubezhAPI;
using RubezhAPI.License;
using RubezhAPI.Models;
using System;
using System.Linq;

namespace RubezhClient
{
	public partial class ClientManager
	{
		public static ClientCredentials ClientCredentials { get; private set; }
		public static ISafeFiresecService FiresecService { get; internal set; }

		public static string Connect(ClientType clientType, string serverAddress, string login, string password)
		{
			try
			{
				ClientCredentials = new ClientCredentials()
				{
					Login = login,
					Password = password,
					ClientType = clientType,
					ClientUID = FiresecServiceFactory.UID
				};

				string error = null;
				for (int i = 0; i < 3; i++)
				{
					FiresecService = new SafeFiresecService(serverAddress);
					var operationResult = FiresecService.Connect(ClientCredentials);
					if (!operationResult.HasError)
					{
						error = null;
						break;
					}
					error = operationResult.Error;
				}

				_userLogin = login;
				return error;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.Connect");
				return e.Message;
			}
		}

		public static string GetLicense()
		{
			try
			{
				var operationResult = FiresecService.GetLicenseInfo();
				if (!operationResult.HasError)
				{
					LicenseManager.CurrentLicenseInfo = operationResult.Result;
					return null;
				}
				else
					return operationResult.Error;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.GetLicense");
				return e.Message;
			}
		}

		public static string GetIP()
		{
			return FiresecService.Ping();
		}

		internal static string _userLogin;
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
				Logger.Error(e, "ClientManager.CheckPermission");
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
				Logger.Error(e, "ClientManager.Disconnect");
			}
		}

		public static void StartPoll()
		{
			try
			{
				FiresecService.StartPoll();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.StartPoll");
			}
		}

		static OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> action, string methodName)
		{
			try
			{
				return action();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.SafeOperationCall." + methodName);
				return OperationResult<T>.FromError(e.Message);
			}
		}
	}
}