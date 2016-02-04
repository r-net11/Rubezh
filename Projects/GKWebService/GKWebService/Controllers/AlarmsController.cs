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
			var alarms = AlarmsViewModel.OnGKObjectsStateChanged(null);
			var alarmGroupsViewModel = new AlarmGroupsViewModel();
			alarmGroupsViewModel.Update(alarms);
			return Json(alarmGroupsViewModel, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Alarms()
		{
			return View();
		}

		[ErrorHandler]
		public JsonResult GetAlarms()
		{
			var alarms = AlarmsViewModel.OnGKObjectsStateChanged(null);
			var alarmsViewModel = new AlarmsViewModel();
			alarmsViewModel.UpdateAlarms(alarms);
			return Json(alarmsViewModel, JsonRequestBehavior.AllowGet);
		}
	}
}