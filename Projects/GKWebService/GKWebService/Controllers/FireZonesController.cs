using GKWebService.DataProviders.FireZones;
using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RubezhAPI.Automation;

namespace GKWebService.Controllers
{
    public class FireZonesController : Controller
    {
        // GET: FireZones
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FireZonesDetails()
        {
            return View();
        }

        public JsonResult GetFireZonesData()
        {
            return Json(FireZonesDataProvider.Instance.GetFireZones(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Метод, предоставляющий данные об устройствах для конкретной зоны
        /// </summary>
        public JsonResult GetDevicesListByZoneNumber(int id)
        {
            return Json(FireZonesDataProvider.Instance.GetDevicesByZone(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TurnOff(Guid id)
        {
            if (GKManager.Zones.FirstOrDefault(z => z.UID == id) != null)
            {
                ClientManager.FiresecService.ControlFireZone(id, ZoneCommandType.Ignore);
            }

            return new JsonResult();
        }


        [HttpPost]
        public JsonResult TurnOn(Guid id)
        {
            if (GKManager.Zones.FirstOrDefault(z => z.UID == id) != null)
            {
                ClientManager.FiresecService.ControlFireZone(id, ZoneCommandType.ResetIgnore);
            }

            return new JsonResult();
        }

        [HttpPost]
        public JsonResult Reset(Guid id)
        {
            if (GKManager.Zones.FirstOrDefault(z => z.UID == id) != null)
            {
                ClientManager.FiresecService.ControlFireZone(id, ZoneCommandType.Reset);
            }

            return new JsonResult();
        }

    }
}