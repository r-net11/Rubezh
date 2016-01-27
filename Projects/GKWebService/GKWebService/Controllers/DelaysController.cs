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
		public ActionResult DelayDetails()
		{
			return View();
		}
		public JsonResult GetDelays()
		{
			var delays = new List<DelayViewModel>();
			foreach (var delay in GKManager.Delays)
			{
				var copyDelay = new DelayViewModel(delay);
				delays.Add(copyDelay);
			}
			return Json(delays, JsonRequestBehavior.AllowGet);
		}

	}
}