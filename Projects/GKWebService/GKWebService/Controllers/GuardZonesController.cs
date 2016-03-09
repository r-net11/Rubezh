using GKWebService.Models;
using GKWebService.Models.GuardZones;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
	[Authorize]
	public class GuardZonesController : Controller
	{
		// GET: GuardZones
		public ActionResult Guard()
		{
			return View();
		}


		public ActionResult GuardZoneDetails()
		{
			return View();
		}

		[HttpGet]
		public JsonResult GetGuardZones()
		{
			List<GuardZone> guardZones = new List<GuardZone>();
			GKManager.GuardZones.ForEach(x => guardZones.Add(new GuardZone(x)));
			return Json(guardZones.OrderBy(x => x.No), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetGuardZoneDevices(Guid id)
		{
			List<Device> data = new List<Device>();
			var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == id);
			if (guardZone != null)
				guardZone.GuardZoneDevices.ForEach(x => data.Add(new Device(x.Device) {ActionType = !x.Device.Driver.IsCardReaderOrCodeReader? x.ActionType.ToDescription(): string.Empty}));
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult SetAutomaticState(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(guardZone);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetManualState(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(guardZone);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetIgnoreState(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(guardZone);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOn(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.FiresecService.GKTurnOn(guardZone);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOnNow(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.FiresecService.GKTurnOnNow(guardZone);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOff(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.FiresecService.GKTurnOff(guardZone);
			}

			return new JsonResult();
		}

		public JsonResult TurnOffNow(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.FiresecService.GKTurnOffNow(guardZone);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult Reset(Guid id)
		{
			var mpt = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKReset(mpt);
			}

			return new JsonResult();
		}
	}
}