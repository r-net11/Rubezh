using System;
using System.Linq;
using System.Web.Mvc;
using GKWebService.DataProviders.Devices;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhClient;


namespace GKWebService.Controllers
{
	public class DevicesController : Controller
	{

		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Метод, предоставляющий данные об устройствах 
		/// </summary>
		public JsonResult GetDevicesList()
		{
			return Json(DevicesDataProvider.Instance.GetDevices(), JsonRequestBehavior.AllowGet);
		}

		public ActionResult DeviceDetails()
		{
			return View();
		}

		/// <summary>
		/// Метод, предоставляющий данные об устройствах 
		/// </summary>
		public JsonResult GetDeviceParameters()
		{
			return Json(DevicesDataProvider.Instance.GetDevices(), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult SetAutomaticState(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null){
				ClientManager.FiresecService.GKSetAutomaticRegime(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetIgnoreState(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null){
				ClientManager.FiresecService.GKSetIgnoreRegime(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult Reset(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null){
				ClientManager.FiresecService.GKReset(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult SetManualState(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOn(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKTurnOn(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOnNow(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKTurnOnNow(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult ForbidStart(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKStop(device);
			}
			return new JsonResult();
		}

		[HttpPost]
		public JsonResult TurnOff(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKTurnOff(device);
			}
			return new JsonResult();
		}
	}
}