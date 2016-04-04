using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GKWebService.Models;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using GKWebService.DataProviders;

namespace GKWebService.Controllers
{
	[Authorize]
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
			return Json(BuildTreeList(GKManager.DeviceConfiguration.RootDevice), JsonRequestBehavior.AllowGet);
		}

		public ActionResult DeviceDetails()
		{
			return View();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetAutomaticState(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(device, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetIgnoreState(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(device, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult Reset(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKReset(device, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetManualState(Guid id)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == id);
			if (device != null)
			{
				ClientManager.FiresecService.GKSetManualRegime(device, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult OnExecuteCommand(GKStateBit commandName, Guid UID)
		{
			var device = GKManager.Devices.FirstOrDefault(dev => dev.UID == UID);
			if (device != null)
			{
				ClientManager.FiresecService.GKExecuteDeviceCommand(device, commandName, ClientManager.CurrentUser.Name);
			}
			return new JsonResult();
		}

		private List<Device> BuildTreeList(GKDevice device, int level = 0)
		{
			var list = new List<Device>();
			list.Add(new Device(device)
			{
				Level = level,
			});
			foreach (var child in device.Children)
			{
				list.AddRange(BuildTreeList(child, level + 1));
			}
			return list;
		}
	}
}