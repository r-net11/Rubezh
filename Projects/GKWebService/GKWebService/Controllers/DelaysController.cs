using GKWebService.Models;
using RubezhAPI;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
	public class DelaysController : Controller
	{
		public ActionResult Delays()
		{
			return View();
		}
		public ActionResult DelaysDetails()
		{
			return View();
		}
		public JsonResult GetDelays()
		{
			var delays = new List<DelayModel>();
			foreach (var delay in GKManager.Delays)
			{
				var copyDelay = new DelayModel(delay);
				delays.Add(copyDelay);
			}
			return Json(delays, JsonRequestBehavior.AllowGet);
		}

	}
}