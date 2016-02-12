#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Web.Mvc;
using GKWebService.DataProviders;
using GKWebService.DataProviders.Plan;
using GKWebService.Models.Plan;
using RubezhClient;

#endregion

namespace GKWebService.Controllers
{
	public class PlansController : Controller
	{
		[HttpGet]
		public ActionResult List()
		{
			var result = PlansDataProvider.Instance.GetPlansList();
			return Json(result, JsonRequestBehavior.AllowGet);
		} 

		[HttpGet]
		public ActionResult GetPlan(Guid planGuid)
		{

			var thread = Task.Factory.StartNewSta<PlanSimpl>(() =>
			{
				var result = System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
					() =>
					{
						Debug.WriteLine(string.Format("GetPlan thread is {0}, with appartment = {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.GetApartmentState().ToString()));
						return PlansDataProvider.Instance.GetPlan(planGuid);
					});
				return result;

			});
			Task.WaitAll();

			var plan = thread.Result;

			if (plan == null) {
				return HttpNotFound(string.Format("План с ID {0} не найден", planGuid));
			}
			return Json(plan, JsonRequestBehavior.AllowGet);
		}

		protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior) {
			return new JsonResult {
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding,
				JsonRequestBehavior = behavior,
				MaxJsonLength = int.MaxValue
			};
		}

		//public ActionResult GetPlans() {
		//	var result = Json(PlansDataProvider.Instance.Plans, JsonRequestBehavior.AllowGet);
		//	return result;
		//}

		//public ActionResult GetPlan(Guid planGuid) {
		//	var plan =
		//		PlansDataProvider.Instance.Plans.FirstOrDefault(p => p.Uid == planGuid);

		//	if (plan == null) {
		//		return HttpNotFound(string.Format("План с ID {0} не найден", planGuid));
		//	}
		//	return Json(plan, JsonRequestBehavior.AllowGet);
		//}
	}
}
