using RubezhService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RubezhService.Web.Controllers
{
    public class ConnectionsController : Controller
    {
        // GET: Connections
        public ActionResult Connections()
        {
            return View(ConnectionsModel.Connections);
        }
    }
}