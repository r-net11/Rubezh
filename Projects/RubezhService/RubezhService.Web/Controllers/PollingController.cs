using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RubezhService.Models;

namespace RubezhService.Web.Controllers
{
    public class PollingController : Controller
    {
        // GET: Polling
        public ActionResult Polling()
        {
            return View(PollingModel.PollingItems);
        }
    }
}