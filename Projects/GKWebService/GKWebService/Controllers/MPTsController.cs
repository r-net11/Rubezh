using GKWebService.Models;
using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
    public class MPTsController : Controller
    {
        // GET: MPTs
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult MPTDetails()
		{
			return View();
		}

		public JsonResult GetMPTsData()
		{
			var data = new List<MPTModel>();
			foreach (var mpt in GKManager.MPTs)
			{
				var devices = new List<MPTDevice>();
				mpt.MPTDevices.ForEach(x =>
				{
					devices.Add(new MPTDevice { DottedPresentationAddress = x.Device.DottedPresentationAddress, MPTDeviceType = x.MPTDeviceType.ToDescription(), Uid = x.DeviceUID, Description = x.Device.Description });
				});
				data.Add(new MPTModel 
				{ Name = mpt.Name, 
			      No = mpt.No, 
				  UID = mpt.UID,
				  MptLogic = GKManager.GetPresentationLogic(mpt.MptLogic), 
				  MPTDevices = devices, 
				  Delay = mpt.Delay,
				  StateIcon = mpt.State.StateClass.ToString()
				}
				  );
			}
			data.Reverse();
			return Json(data, JsonRequestBehavior.AllowGet);
		}

    }
}