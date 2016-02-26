using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models;
using GKWebService.Models.ViewModels;
using RubezhAPI;

namespace GKWebService.Controllers
{
	public class DeviceParametersController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Метод, предоставляющий данные об устройствах 
		/// </summary>
		public JsonResult GetDeviceParameters()
		{
			List<DeviceParametersViewModel> parameters = GKManager.Devices
				.Where(d => d.Driver.MeasureParameters.Count(p => !p.IsDelay) > 0)
				.Select(x => new DeviceParametersViewModel(x))
				.ToList();

			return Json(parameters, JsonRequestBehavior.AllowGet);
		}
	}
}