using GKWebService.Models;
using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
	public class DelaysController : Controller
	{
		public ActionResult Delays()
		{
			return View();
		}
		public ActionResult DelayDetails()
		{
			return View();
		}
		public JsonResult GetDelays()
		{
			var delays = new List<Delay>();
			foreach (var delay in GKManager.Delays)
			{
				var copyDelay = new Delay(delay);
				delays.Add(copyDelay);
			}
			return Json(delays, JsonRequestBehavior.AllowGet);
		}
		public JsonResult SetAutomaticState(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(delay);
			}
			return new JsonResult();
		}
		[HttpPost]
		public JsonResult SetManualState(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(delay);
			}
			return new JsonResult();
		}
		[HttpPost]
		public JsonResult SetIgnoreState(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(delay);
			}
			return new JsonResult();
		}
		[HttpPost]
		public JsonResult TurnOn(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.FiresecService.GKTurnOn(delay);
			}
			return new JsonResult();
		}
		[HttpPost]
		public JsonResult TurnOnNow(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.FiresecService.GKTurnOnNow(delay);
			}
			return new JsonResult();
		}
		[HttpPost]
		public JsonResult TurnOff(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.FiresecService.GKTurnOff(delay);
			}
			return new JsonResult();
		}

	}
}