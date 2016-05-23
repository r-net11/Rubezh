using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RubezhService.Models;

namespace RubezhService.Web.Controllers
{
    public class GKController : Controller
    {
        // GET: GK
        public ActionResult GK()
        {
            return View(GKModel.GetLifecycleItems(20));
        }
    }
}