using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}