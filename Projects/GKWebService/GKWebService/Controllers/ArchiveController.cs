using GKWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
    public class ArchiveController : Controller
    {
        [HttpPost]
        [HttpGet]
        public JsonResult GetArchive()
        {
            List<JournalModel> list = new List<JournalModel>();

            for (int i = 0; i < 10; i++)
            {
                //list.Add(new JournalModel()
                //{
					//Desc = "Описание" + i.ToString(),
					//DeviceDate = DateTime.Now,
					//Name = "Назваине" + i.ToString(),
					//Object = "Объект" + i.ToString(),
					//SystemDate = DateTime.Now
                //});
            }

            return Json(list);
        }
    }
}