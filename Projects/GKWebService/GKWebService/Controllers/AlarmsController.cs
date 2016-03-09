using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models.GK.Alarms;
using GKWebService.Utils;

namespace GKWebService.Controllers
{
	[Authorize]
	public class AlarmsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		[AllowAnonymous]
		public ActionResult AlarmGroups()
        {
            return View();
        }

		[ErrorHandler]
		[AllowAnonymous]
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

		[HttpPost]
		[ErrorHandler]
		public JsonResult ResetAll()
		{
			AlarmsViewModel.ResetAll();

			return new JsonResult();
		}

		[HttpPost]
		[ErrorHandler]
		public JsonResult Reset(AlarmViewModel alarm)
		{
			alarm.Reset();

			return new JsonResult();
		}

		[HttpPost]
		[ErrorHandler]
		public JsonResult ResetIgnore(AlarmViewModel alarm)
		{
			alarm.ResetIgnore();

			return new JsonResult();
		}

		[HttpPost]
		[ErrorHandler]
		public JsonResult TurnOnAutomatic(AlarmViewModel alarm)
		{
			alarm.TurnOnAutomatic();

			return new JsonResult();
		}

		[HttpPost]
		[ErrorHandler]
		public JsonResult ResetIgnoreAll()
		{
			AlarmsViewModel.ResetIgnoreAll();

			return new JsonResult();
		}
	}
}