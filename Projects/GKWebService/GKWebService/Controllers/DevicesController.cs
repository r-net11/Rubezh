using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GKWebService.Models;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;


namespace GKWebService.Controllers
{
	[Authorize]
	public class DevicesController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Метод, предоставляющий данные об устройствах 
		/// </summary>
		public JsonResult GetDevicesList()
		{
			return Json(BuildTreeList(GKManager.DeviceConfiguration.RootDevice), JsonRequestBehavior.AllowGet);
		}

		public ActionResult DeviceDetails()
		{
			return View();
		}

		[HttpPost]
		public JsonResult SetAutomaticState(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetIgnoreState(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult Reset(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKReset(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetManualState(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOn(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKTurnOn(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOnNow(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKTurnOnNow(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult ForbidStart(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKStop(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOff(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKTurnOff(device);
			}
			return new JsonResult();
		}

		private List<Device> BuildTreeList(GKDevice device, int level = 0)
		{
			var list = new List<Device>();
			list.Add(new Device(device)
			{
				Level = level,
			});
			foreach (var child in device.Children)
			{
				list.AddRange(BuildTreeList(child, level + 1));
			}
			return list;
		}
	}
}