#region Usings

using System;
using System.Linq;
using System.Web.Mvc;
using GKWebService.DataProviders;
using GKWebService.DataProviders.Plan;

#endregion

namespace GKWebService.Controllers
{
	public class PlansController : Controller
	{
		protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior) {
			return new JsonResult {
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding,
				JsonRequestBehavior = behavior,
				MaxJsonLength = int.MaxValue
			};
		}

		/*public ActionResult GetPlans() {
			var result = Json(PlansDataProvider.Instance.Plans, JsonRequestBehavior.AllowGet);
			return result;
		}

		public ActionResult GetPlan(Guid planGuid) {
			var plan =
				PlansDataProvider.Instance.Plans.FirstOrDefault(p => p.Uid == planGuid);

			if (plan == null) {
				return HttpNotFound(string.Format("План с ID {0} не найден", planGuid));
			}
			return Json(plan, JsonRequestBehavior.AllowGet);
		}*/
	}
}
