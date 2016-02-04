﻿using GKWebService.DataProviders.FireZones;
using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models;
using RubezhAPI.Automation;
using GKWebService.Models.FireZone;

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

		/// <summary>
		/// Метод, предоставляющий данные о зонах
		/// </summary>
		/// <returns></returns>
        public JsonResult GetFireZonesData()
        {
            return Json(GKManager.Zones.Select(
				zone => new FireZone(zone)).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Метод, предоставляющий данные об устройствах для конкретной зоны
        /// </summary>
        public JsonResult GetDevicesListByZoneNumber(int id)
        {
			List<DeviceNode> listTree = new List<DeviceNode>();

			DeviceNode data = new DeviceNode();

			int level = 0;

			if (GKManager.Zones.Count - 1 < id)
			{
				return null;
			}

			foreach (var remoteDevice in GKManager.Zones[id].Devices)
			{
				data.DeviceList.Add(new Device()
				{
					Name = remoteDevice.PresentationName,
					Address = remoteDevice.Address,
					ImageDeviceIcon = "/Content/Image/" + remoteDevice.ImageSource.Replace("/Controls;component/", ""),
					StateIcon = "/Content/Image/Icon/GKStateIcons/" + Convert.ToString(remoteDevice.State.StateClass) + ".png",
					Level = level,
					Note = remoteDevice.Description
				});
			}

			listTree.Add(data);
			var device = GKManager.Zones[id].Devices.FirstOrDefault();

			while (device != null && device.Parent != null)
			{
				level++;
				DeviceNode item = new DeviceNode();
				device = device.Parent;
				item.DeviceList.Add(new Device()
				{
					Name = device.PresentationName,
					Address = device.Address,
					ImageDeviceIcon = "/Content/Image/" + device.ImageSource.Replace("/Controls;component/", ""),
					StateIcon = "/Content/Image/Icon/GKStateIcons/" + Convert.ToString(device.State.StateClass) + ".png",
					Level = level,
					Note = device.Description
				});
				listTree.Add(item);
			}
			listTree.Reverse();

			return Json(listTree, JsonRequestBehavior.AllowGet);
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