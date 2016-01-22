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

		public ActionResult Report()
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


		public JsonResult GetFireZonesData()
		{
			//Получили данные с сервера
			var zone = FireZonesDataProvider.Instance.GetFireZones();

			//Создали объект для передачи на клиент и заполняем его данными
			FireZone data = new FireZone();

			//Имя зоны
			data.DescriptorPresentationName = zone.DescriptorPresentationName;

			//Количество датчиков для перевода в состояние Пожар1
			data.Fire1Count = zone.Fire1Count;

			//Количество датчиков для перевода в состояние Пожар2
			data.Fire2Count = zone.Fire2Count;

			//Иконка текущей зоны
			data.ImageSource = InternalConverter.GetImageResource(zone.ImageSource);

			//Изображение, сигнализирующее о состоянии зоны
			data.StateImageSource = InternalConverter.GetImageResource("StateClassIcons/" + Convert.ToString(zone.State.StateClass) + ".png");

			//Переносим устройства для этой зоны
			foreach (var deviceItem in zone.Devices)
			{
				var device = deviceItem;
				do
				{
					data.devicesList.Add(new Device(device.Address, device.ImageSource, device.ShortName, device.State.StateClass));
					device = device.Parent;
				} while (device != null);
			}
			data.devicesList.Reverse();

			//Передаем данные на клиент
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