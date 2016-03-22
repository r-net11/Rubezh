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
			get
			{
				var userName = HttpContext.Current.User.Identity.Name;
				return users[userName];
			}
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

				if (users.ContainsKey(clientCredentials.Login))
				{
					//Если пользователь уже аудентифицирован и законнектен то надо только аудентифицировать, но не коннектить
					if (!CheckPass(clientCredentials.Login, clientCredentials.Password))
					{
						error = "Неверный логин или пароль";
					}
				}
				else
				{
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
				}

				return error;
			}
		}

		public static bool CheckPass(string userName, string password)
		{
			var user = RubezhClient.ClientManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == userName);
			return HashHelper.CheckPass(password, user.PasswordHash);
		}

		public static void AddAdminUser(string login, ISafeFiresecService firesecService)
		{
			users.AddOrUpdate(login, firesecService, (key, oldValue) => firesecService);
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