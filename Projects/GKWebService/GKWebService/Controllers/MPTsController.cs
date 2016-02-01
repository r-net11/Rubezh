﻿using GKWebService.Models;
using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
    public class MPTsController : Controller
    {
        // GET: MPTs
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult MPTDetails()
		{
			return View();
		}

		public JsonResult GetMPTsData()
		{
			var data = new List<MPTModel>();
			foreach (var mpt in GKManager.MPTs)
			{
				var devices = new List<MPTDevice>();
				mpt.MPTDevices.ForEach(x =>
				{
					devices.Add(new MPTDevice { DottedPresentationAddress = x.Device.DottedPresentationAddress, MPTDeviceType = x.MPTDeviceType.ToDescription(), Uid = x.DeviceUID, Description = x.Device.Description });
				});
				data.Add(new MPTModel(mpt));
			}
			data.Reverse();
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public JsonResult SetAutomaticState(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetManualState(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetIgnoreState(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOn(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKTurnOn(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOnNow(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKTurnOnNow(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOff(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKTurnOff(mpt);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult ForbidStart(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.FiresecService.GKStop(mpt);
			}

			return new JsonResult();
		}

    }
}