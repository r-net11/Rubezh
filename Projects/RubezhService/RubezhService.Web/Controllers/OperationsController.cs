using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RubezhService.Models;

namespace RubezhService.Web.Controllers
{
    public class OperationsController : Controller
    {
        // GET: Operations
        public ActionResult Operations()
        {
            return View(OperationsModel.Operations);
        }
    }
}