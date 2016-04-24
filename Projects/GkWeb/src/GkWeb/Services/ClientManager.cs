using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Infrastructure.Common;
using Microsoft.AspNet.Http;
using RubezhAPI.Models;
using RubezhClient;

namespace GkWeb.Services
{
	public class ClientManager
	{
		public ClientManager() {
			
		}

		public string Connect(string userName, string password) {
			string error = null;

			if (!CheckPass(userName, password ?? string.Empty)) {
				error = "Неверный логин или пароль";
			}

			return error;
		}

		public bool CheckPass(string userName, string password) {
			var user = RubezhClient.ClientManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == userName);
			return HashHelper.CheckPass(password, user.PasswordHash);
		}

		//public User CurrentUser
		//{
		//	get { return RubezhClient.ClientManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == HttpContext.User.Identity.Name); }
		//}

		//public bool CheckPermission(PermissionType permissionType) {
		//	try {
		//		if (CurrentUser == null)
		//			return false;
		//		return CurrentUser.HasPermission(permissionType);
		//	}
		//	catch (Exception e) {
		//		Logger.Error(e, "ClientManager.CheckPermission");
		//		return false;
		//	}
		//}
	}

}
