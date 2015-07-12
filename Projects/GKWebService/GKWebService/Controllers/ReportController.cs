using GKWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
    public class ReportController : Controller
    {
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