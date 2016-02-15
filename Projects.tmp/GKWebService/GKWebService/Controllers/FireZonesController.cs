using GKWebService.DataProviders.FireZones;
using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models;
using RubezhAPI.Automation;
using GKWebService.Models.FireZone;

namespace GKWebService.Controllers
{
	public class FireZonesController : Controller
	{
		// GET: FireZones
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
				zone => new FireZone(zone)).ToList(), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Метод, предоставляющий данные об устройствах для конкретной зоны
		/// </summary>
		public JsonResult GetDevicesByZoneUID(Guid id)
		{
			var listTree = new List<DeviceNode>();
			var data = new DeviceNode();
			int level = 0;

			var firstZone = GKManager.Zones.FirstOrDefault(zone => zone.UID == id);
			if (firstZone != null)
			{
				var devices = firstZone.Devices;

				foreach (var remoteDevice in devices)
				{
					data.DeviceList.Add(new Device(remoteDevice)
					{
						Level = level
					});
				}

				listTree.Add(data);
				var device = devices.FirstOrDefault();
				while (device != null && device.Parent != null)
				{
					level++;
					var item = new DeviceNode();
					device = device.Parent;
					item.DeviceList.Add(new Device(device)
					{
						Level = level
					});
					listTree.Add(item);
				}
			}
			listTree.Reverse();
			return Json(listTree, JsonRequestBehavior.AllowGet);
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