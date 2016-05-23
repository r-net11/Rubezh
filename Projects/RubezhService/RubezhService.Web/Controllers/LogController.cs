using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RubezhService.Models;

namespace RubezhService.Web.Controllers
{
    public class LogController : Controller
    {
        // GET: Log
        public ActionResult Log()
        {
            return View(LogModel.LogItems);
        }
    }
}