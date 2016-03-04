using GKWebService.Models.Door;
using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
	[Authorize]
	public class DoorsController : Controller
    {
        // GET: Doors
        public ActionResult Door()
        {
            return View();
        }

		public ActionResult DoorDetails()
		{
			return View();
		}

		public JsonResult GetDoors()
		{
			var data = new List<Door>();
			GKManager.Doors.ForEach(x => data.Add(new Door(x)));
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public JsonResult SetAutomaticState(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(door);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetManualState(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(door);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetIgnoreState(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(door);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOn(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.FiresecService.GKTurnOn(door);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOffNow(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.FiresecService.GKTurnOffNow(door);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOff(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.FiresecService.GKTurnOff(door);
			}

			return new JsonResult();
		}

		[HttpPost]
		public JsonResult Reset(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.FiresecService.GKReset(door);
			}

			return new JsonResult();
		}

		public JsonResult SetRegimeNorm(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(door);
				ClientManager.FiresecService.GKTurnOffInAutomatic(door);
			}

			return new JsonResult();

		}

		public JsonResult SetRegimeOpen(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(door);
				ClientManager.FiresecService.GKTurnOn(door);
			}

			return new JsonResult();
		}

		public JsonResult SetRegimeClose(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(door);
				ClientManager.FiresecService.GKTurnOff(door);
			}

			return new JsonResult();
		}
    }
}