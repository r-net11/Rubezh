using GKWebService.DataProviders.FireZones;
using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using RubezhAPI;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public ActionResult Journal()
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

		public ActionResult FireZones()
		{
			return View();
		}

		public ActionResult Directions()
		{
			return View();
		}

		public ActionResult Delays()
		{
			return View();
		}

		public JsonResult GetDirections()
		{
			var directions = new List<Direction>();
			foreach (var realDirection in GKManager.Directions)
			{
				var direction = new Direction
				{
					UID = realDirection.UID,
					No = realDirection.No,
					Name = realDirection.Name,
					Delay = realDirection.Delay,
					Hold = realDirection.Hold,
					DelayRegime = realDirection.DelayRegime.ToDescription(),
					Logic = GKManager.GetPresentationLogic(realDirection.Logic),
					State = realDirection.State.StateClass.ToDescription(),
					StateIcon = realDirection.State.StateClass.ToString(),
					OnDelay = realDirection.State.OnDelay,
					HoldDelay = realDirection.State.HoldDelay,
					GKDescriptorNo = realDirection.GKDescriptorNo,
				};
				directions.Add(direction);
			}

			dynamic result = new
			{
				page = 1,
				total = 100,
				records = 100,
				rows = directions
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}


		public ActionResult MPTs()
		{
			return View();
		}

		public JsonResult GetFireZonesData()
		{
			return Json(FireZonesDataProvider.Instance.GetFireZones(), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Метод, предоставляющий данные об устройствах для конкретной зоны
		/// </summary>
		public JsonResult GetDevicesListByZoneNumber(int id)
		{
			return Json(FireZonesDataProvider.Instance.GetDevicesByZone(id), JsonRequestBehavior.AllowGet);
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


		public JsonResult GetJournal()
		{
			var apiItems = JournalHelper.Get(new JournalFilter());
			var list = apiItems.Select(x => new ReportModel()
			{
				Desc = x.JournalEventDescriptionType.ToDescription(),
				DeviceDate = DateTime.Now,
				Name = x.JournalEventNameType.ToDescription(),
				Object = x.JournalObjectType.ToDescription(),
				SystemDate = DateTime.Now
			}).ToList();
			dynamic result = new
			{
				page = 1,
				total = list.Count(),
				records = list.Count(),
				rows = list,
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}
	}
}