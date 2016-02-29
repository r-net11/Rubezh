using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;

namespace GKWebService.Controllers
{
	[Authorize]
	public class DirectionsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult DirectionDetails()
        {
            return View();
        }

		[ErrorHandler]
		public JsonResult GetDirections()
		{
			var directions = new List<Direction>();
			foreach (var gkDirection in GKManager.Directions)
			{
				var direction = new Direction(gkDirection);
				directions.Add(direction);
			}

			return Json(directions, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ErrorHandler]
	    public JsonResult SetAutomaticState(Guid id)
		{
			var direction = GKManager.Directions.FirstOrDefault(d => d.UID == id);
			if (direction != null)
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(direction);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ErrorHandler]
		public JsonResult SetManualState(Guid id)
		{
			var direction = GKManager.Directions.FirstOrDefault(d => d.UID == id);
			if (direction != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(direction);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ErrorHandler]
		public JsonResult SetIgnoreState(Guid id)
		{
			var direction = GKManager.Directions.FirstOrDefault(d => d.UID == id);
			if (direction != null)
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(direction);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ErrorHandler]
		public JsonResult TurnOn(Guid id)
		{
			var direction = GKManager.Directions.FirstOrDefault(d => d.UID == id);
			if (direction != null)
			{
				ClientManager.FiresecService.GKTurnOn(direction);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ErrorHandler]
		public JsonResult TurnOnNow(Guid id)
		{
			var direction = GKManager.Directions.FirstOrDefault(d => d.UID == id);
			if (direction != null)
			{
				ClientManager.FiresecService.GKTurnOnNow(direction);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ErrorHandler]
		public JsonResult TurnOff(Guid id)
		{
			var direction = GKManager.Directions.FirstOrDefault(d => d.UID == id);
			if (direction != null)
			{
				ClientManager.FiresecService.GKTurnOff(direction);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ErrorHandler]
		public JsonResult ForbidStart(Guid id)
		{
			var direction = GKManager.Directions.FirstOrDefault(d => d.UID == id);
			if (direction != null)
			{
				ClientManager.FiresecService.GKStop(direction);
			}

			return new JsonResult();
		}
    }
}