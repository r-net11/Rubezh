using GKWebService.DataProviders;
using GKWebService.Models;
using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
	[Authorize]
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

			return Json(delays.OrderBy(x=> x.Number), JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetAutomaticState(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.RubezhService.GKSetAutomaticRegime(delay, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}
		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetManualState(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.RubezhService.GKSetManualRegime(delay, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}
		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetIgnoreState(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.RubezhService.GKSetIgnoreRegime(delay, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}
		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOn(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.RubezhService.GKTurnOn(delay, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}
		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOnNow(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.RubezhService.GKTurnOnNow(delay, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}
		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOff(Guid id)
		{
			var delay = GKManager.Delays.FirstOrDefault(d => d.UID == id);
			if (delay != null)
			{
				ClientManager.RubezhService.GKTurnOff(delay, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}

	}
}