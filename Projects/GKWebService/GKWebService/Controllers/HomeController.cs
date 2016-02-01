﻿using GKWebService.DataProviders.Devices;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
	public class HomeController : Controller
	{
		// GET: Home
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult State()
		{
			return View();
		}

		public ActionResult Device()
		{
			return View();
		}

		public ActionResult Archive()
		{
			return View();
		}

		public ActionResult Plan()
		{
			return View();
		}


		public ActionResult MPTs()
		{
			return View();
		}

		/// <summary>
		/// Метод, предоставляющий данные об устройствах 
		/// </summary>
		public JsonResult GetDevicesList()
		{
			return Json(DevicesDataProvider.Instance.GetDevices(), JsonRequestBehavior.AllowGet);
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