using GKWebService.Models;
using GKWebService.Models.GuardZones;
using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
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
			List<GuardZone> data = new List<GuardZone>();
			GKManager.GuardZones.ForEach(x => data.Add(new GuardZone(x)));
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetGuardZoneDevices(Guid id)
		{
			List<Device> data = new List<Device>();
			var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == id);
			if (guardZone != null)
				guardZone.GuardZoneDevices.ForEach(x => data.Add(new Device(x.Device) {ActionType = x.ActionType.ToDescription()}));
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult SetAutomaticState(Guid id)
		{
			var mpt = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetManualState(Guid id)
		{
			var mpt = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetIgnoreState(Guid id)
		{
			var mpt = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOn(Guid id)
		{
			var mpt = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKTurnOn(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOnNow(Guid id)
		{
			var mpt = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKTurnOnNow(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOff(Guid id)
		{
			var mpt = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKTurnOff(mpt);
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