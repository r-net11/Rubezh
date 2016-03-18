using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GKWebService.Models;
using GKWebService.Models.GK;
using GKWebService.Utils;
using Infrastructure.Common;
using RubezhAPI.Models;
using RubezhClient;

namespace GKWebService.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		// GET: Home
		[AllowAnonymous]
		public ActionResult Index()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public JsonResult Login(LoginData loginData)
		{
			var error = ClientManager.Connect(loginData.userName, loginData.password);

			if (string.IsNullOrEmpty(error))
			{
				var authTicket = new FormsAuthenticationTicket(
					2,
					loginData.userName,
					DateTime.Now,
					DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
					false,
					string.Empty //clientCredentials.ClientUID.ToString()
					);
				var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket))
				{
					HttpOnly = true
				};
				Response.SuppressFormsAuthenticationRedirect = true;
				Response.AppendCookie(authCookie);
			}

			return Json(new { success = string.IsNullOrEmpty(error), message = error, permissions = GetCurrentUserPermissions() });
		}

		public JsonResult TryGetCurrentUserName()
		{
			return Json(new { userName = User.Identity.Name, permissions = GetCurrentUserPermissions() }, JsonRequestBehavior.AllowGet);
		}

		private List<string> GetCurrentUserPermissions()
		{
			//var permissions = ClientManager.CurrentUser.PermissionStrings;
			// на случай если в ОЗ поменяется наменование конкретного права permission произойдёт ошибка компиляции
			var permissions = new List<string>();
			if (ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
				permissions.Add("Oper_Device_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_Full_Door_Control))
				permissions.Add("Oper_Full_Door_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_Door_Control))
				permissions.Add("Oper_Door_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
				permissions.Add("Oper_Zone_Control");
            return permissions;
		}

		[HttpPost]
		public JsonResult LogOut()
		{
			FormsAuthentication.SignOut();
            return Json("ok", JsonRequestBehavior.AllowGet);
		}

		[AllowAnonymous]
		[ErrorHandler]
		public JsonResult GetNavigationItems()
		{
			var gkModelLoader = new GKModuleLoader();
			var items = gkModelLoader.CreateNavigation();
			gkModelLoader.Initialize();
			return Json(items, JsonRequestBehavior.AllowGet);
		}

		public ActionResult RestartDetails()
		{
			return View();
		}

		public ActionResult MPTs()
		{
			return View();
		}

		[HttpPost]
		public JsonResult Logon(string login, string password)
		{
			string error = null;

			if (!login.Equals("admin") || !password.Equals("admin"))
			{
				error = "Неверный логин или пароль";
			}

			return Json(new { Success = error == null, Message = error });
		}
	}
}