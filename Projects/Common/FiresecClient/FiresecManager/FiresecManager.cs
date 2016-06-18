using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Data;

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

				string error = null;
				for (var i = 0; i < 3; i++)
				{
					FiresecService = new SafeFiresecService(serverAddress);
					var operationResult = FiresecService.Connect(FiresecServiceFactory.UID, ClientCredentials, true);
					if (!operationResult.HasError)
					{
						error = null;
						break;
					}
					error = operationResult.Error;
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));
				}

				_userLogin = login;
				return error;
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

		public static void StartPoll()
		{
			try
			{
				FiresecService.StartPoll();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.StartPoll");
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
				Logger.Error(e, "FiresecManager.SafeOperationCall." + methodName);
				return OperationResult<T>.FromError(e.Message);
			}
		}
	}
}