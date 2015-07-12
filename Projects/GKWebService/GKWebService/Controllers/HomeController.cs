using GKWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult State()
        {
            return View();
        }

        public ActionResult Device()
        {
            return View();
        }

        public ActionResult Report()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Logon(string login, string password)
        {
            string error = null;

            if(!login.Equals("admin") || !password.Equals("admin"))
            {
                error = "Неверный логин или пароль";
            }

            return Json(new { Success = error == null, Message = error });
        }

        [HttpPost]
        [HttpGet]
        public JsonResult GetReports()
        {
            List<ReportModel> list = new List<ReportModel>();

            for (int i = 0; i < 10; i++)
            {
                list.Add(new ReportModel()
                {
                    Desc = "Описание" + i.ToString(),
                    DeviceDate = DateTime.Now,
                    Name = "Назваине" + i.ToString(),
                    Object = "Объект" + i.ToString(),
                    SystemDate = DateTime.Now
                });
            }

            return Json(list);
        }
    }
}