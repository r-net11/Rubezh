using GKWebService.DataProviders.FireZones;
using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using GKWebService.Models.FireZone;
using GKWebService.Utils;
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
					State = realDirection.State.StateClass.ToDescription(),
					StateIcon = realDirection.State.StateClass.ToString()
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
            //Получаем данные о зонах
		    var data = FireZonesDataProvider.Instance.GetFireZones();

			//Передаем данные на клиент
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetMPTsData()
		{
			var data = new List<MPTModel>();
			foreach (var mpt in GKManager.MPTs)
			{
				data.Add(new MPTModel {Name = mpt.Name, No = mpt.No, UID = mpt.UID});
			}
			data.Reverse();
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetDelays()
		{
			var delays = new List<DelayModel>();
			foreach (var delay in GKManager.Delays)
			{
				var copyDelay = new DelayModel
				{
					Number = delay.No,
					Name = delay.Name,
					PresentationLogic = GKManager.GetPresentationLogic(delay.Logic),
					OnDelay = delay.State.OnDelay,
					HoldDelay = delay.State.HoldDelay
				};
				delays.Add(copyDelay);
			}
			return Json(delays, JsonRequestBehavior.AllowGet);
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