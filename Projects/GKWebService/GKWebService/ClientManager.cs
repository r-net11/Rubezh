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
		public static ISafeRubezhService RubezhService
		{
			get
			{
				return RubezhClient.ClientManager.RubezhService;
			}
		}

		public static string Connect(string userName, string password)
		{
			string error = null;

			if (!CheckPass(userName, password ?? string.Empty))
			{
				error = "Неверный логин или пароль";
			}

			return error;
		}

		public static bool CheckPass(string userName, string password)
		{
			var user = RubezhClient.ClientManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == userName);
			if (user == null)
			{
				return false;
			}
			return HashHelper.CheckPass(password, user.PasswordHash);
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