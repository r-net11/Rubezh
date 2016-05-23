using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RubezhService.Models;

namespace RubezhService.Web.Controllers
{
    public class LicenseController : Controller
    {
        // GET: License
        public ActionResult License()
        {
            ViewBag.InitialKey = LicenseModel.InitialKey;
            return View(LicenseModel.LicenseInfo);
        }
    }
}