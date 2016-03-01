using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GKWebService.Models;

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
			string error = null;

			if (!loginData.userName.Equals("adm", StringComparison.InvariantCultureIgnoreCase))
			{
				error = "Неверный логин или пароль";
			}

			var authTicket = new FormsAuthenticationTicket(
				2,
				loginData.userName,
				DateTime.Now,
				DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
				false,
				"some token that will be used to access the web service and that you have fetched"
			);
			var authCookie = new HttpCookie(
				FormsAuthentication.FormsCookieName,
				FormsAuthentication.Encrypt(authTicket)
			)
			{
				HttpOnly = true
			};
			Response.SuppressFormsAuthenticationRedirect = true;
			Response.AppendCookie(authCookie);

			return Json(new { success = (error == null), message = error });
		}

		public JsonResult TryGetCurrentUserName()
		{
			return Json(new { userName = User.Identity.Name }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult LogOut()
		{
			FormsAuthentication.SignOut();
            return Json("ok", JsonRequestBehavior.AllowGet);
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