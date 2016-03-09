using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GKWebService.Models;
using RubezhAPI.Automation;
using GKWebService.Models.FireZone;
using Microsoft.Ajax.Utilities;
using RubezhAPI.GK;

namespace GKWebService.Controllers
{
	[Authorize]
	public class FireZonesController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Возвращает форму настройки зоны
		/// </summary>
		/// <returns></returns>
		public ActionResult FireZonesDetails()
		{
			return View();
		}

		/// <summary>
		/// Метод, предоставляющий данные о зонах
		/// </summary>
		/// <returns></returns>
		public JsonResult GetFireZonesData()
		{
			return Json(GKManager.Zones.Select(
				zone => new FireZone(zone)).OrderBy(x=> x.No), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Метод, предоставляющий данные об устройствах по UID пожарной зоны
		/// </summary>
		public JsonResult GetDevicesByZoneUid(Guid id)
		{
			var list = new List<Device>();
			var zone = GKManager.Zones.FirstOrDefault(x => x.UID == id);
			if (zone != null)
				list.AddRange(zone.Devices.Select(remoteDevice => new Device(remoteDevice)));
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult SetIgnore(Guid id)
		{
			if (GKManager.Zones.FirstOrDefault(z => z.UID == id) != null)
			{
				ClientManager.FiresecService.ControlFireZone(id, ZoneCommandType.Ignore);
			}

			return new JsonResult();
		}


		[HttpPost]
		public JsonResult ResetIgnore(Guid id)
		{
			if (GKManager.Zones.FirstOrDefault(z => z.UID == id) != null)
			{
				ClientManager.FiresecService.ControlFireZone(id, ZoneCommandType.ResetIgnore);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult ResetFire(Guid id)
		{
			if (GKManager.Zones.FirstOrDefault(z => z.UID == id) != null)
			{
				ClientManager.FiresecService.ControlFireZone(id, ZoneCommandType.Reset);
			}

			return new JsonResult();
		}

	}
}