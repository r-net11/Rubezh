using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GKWebService.Controllers
{
	public class JournalController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult JournalFilter()
		{
			return View();
		}

		[HttpPost]
		public JsonResult GetJournal(GKWebService.Models.JournalFilter filter)
		{
			var journalFilter = new RubezhAPI.Journal.JournalFilter();
			if (filter != null)
			{
				if (filter.deviceUids != null)
					journalFilter.ObjectUIDs = filter.deviceUids;
			};
			var apiItems = JournalHelper.Get(journalFilter);
			var list = apiItems.Select(x => new JournalModel(x)).ToList();
			return Json(list, JsonRequestBehavior.AllowGet);
		}
		
		public JsonResult GetFilterDevices()
		{
			var listTree = new List<DeviceNode>();
			var data = new DeviceNode();
			
			var result = new List<Device>();
			BuildDeviceTree(GKManager.DeviceConfiguration.RootDevice, null, result);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		void BuildDeviceTree(RubezhAPI.GK.GKDevice apiDevice, Device parent, List<Device> devices)
		{
			var device = new Device
			{
				Uid = apiDevice.UID,
				Name = apiDevice.PresentationName,
				Address = apiDevice.Address,
                ImageDeviceIcon = "/Content/Image/" + apiDevice.ImageSource.Replace("/Controls;component/", ""),
				Level = parent != null ? parent.Level + 1 : 0
			};
			devices.Add(device);
			foreach (var child in apiDevice.Children)
			{
				BuildDeviceTree(child, device, devices);
			}
		}
	}
}