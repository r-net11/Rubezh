using GKWebService.DataProviders.FireZones;
using GKWebService.Models;
using RubezhAPI.GK;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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

        public ActionResult Archive()
        {
            return View();
        }

        public ActionResult Plan()
        {
            return View();
        }

        public ActionResult FireZones()
        {
            return View();
        }

        public JsonResult GetFireZonesData()
        {
            var obj = FireZonesDataProvider.Instance.GetFireZones().Devices;

            foreach (var item in obj)
            {
                MemoryStream stream1 = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GKDevice));
                ser.WriteObject(stream1, item);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                var res = sr.ReadToEnd();
            }
            


            var result = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            return Json(result, JsonRequestBehavior.AllowGet);
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


        public JsonResult GetReports()
        {
            List<ReportModel> list = new List<ReportModel>();

            for (int i = 0; i < 100; i++)
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

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = list,
            };




            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}