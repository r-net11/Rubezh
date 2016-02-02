using System.Web.Mvc;
using GKWebService.DataProviders.Devices;


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
	}
}