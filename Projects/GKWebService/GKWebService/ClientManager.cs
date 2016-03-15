using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using Common;
using Infrastructure.Common;
using RubezhAPI.Models;
using RubezhClient;

namespace GKWebService
{
	public class ClientManager
	{
		private static ConcurrentDictionary<string, ISafeFiresecService> users;

		private static object sync = new object();

		public static ISafeFiresecService FiresecService
		{
			get { return users[HttpContext.Current.User.Identity.Name]; }
		}

		static ClientManager()
		{
			users = new ConcurrentDictionary<string, ISafeFiresecService>();
		}

		public static string Connect(string userName, string password)
		{
			lock (sync)
			{
				var clientCredentials = new ClientCredentials()
				{
					Login = userName,
					Password = password ?? string.Empty,
					ClientType = ClientType.WebService
				};

				string error = null;

				for (int i = 0; i < 3; i++)
				{
					var firesecService = new SafeFiresecService(ConnectionSettingsManager.ServerAddress);
					var operationResult = firesecService.Connect(clientCredentials);
					if (!operationResult.HasError)
					{
						users.AddOrUpdate(clientCredentials.Login, firesecService, (key, oldValue) => firesecService);
						error = null;
						break;
					}
					error = operationResult.Error;
				}

				return error;
			}
		}

		public static User CurrentUser
		{
			get { return RubezhClient.ClientManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == HttpContext.Current.User.Identity.Name); }
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
	}
}