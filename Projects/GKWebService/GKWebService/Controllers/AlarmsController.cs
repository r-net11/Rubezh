using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models.GK.Alarms;
using GKWebService.Utils;

namespace GKWebService.Controllers
{
    public class AlarmsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AlarmGroups()
        {
            return View();
        }

		[ErrorHandler]
		public JsonResult GetAlarmGroups()
		{
			return Json(new AlarmGroupsViewModel(), JsonRequestBehavior.AllowGet);
		}
	}
}