using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.Journal;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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

		public JsonResult GetJournal()
		{
			var apiItems = JournalHelper.Get(new JournalFilter());
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