#region Usings

using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Web.Mvc;
using GKWebService.DataProviders.Plan;

#endregion

namespace GKWebService.Controllers
{
	public class PlansController : Controller
	{
		public ActionResult Index() {
			return View();
		}

		/// <summary>
		///     Получение полного списка планов
		/// </summary>
		/// <returns>Список планов, где могут быть планы-папки, у которых могут быть планы-потомки.</returns>
		[HttpGet]
		public ActionResult List() {
			var result = PlansDataProvider.Instance.GetPlansList();
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		///     Получение объекта плана по UID
		/// </summary>
		/// <param name="planGuid">UID плана</param>
		/// <returns>План с UID = <paramref name="planGuid" /></returns>
		[HttpGet]
		public ActionResult GetPlan(Guid planGuid) {
			var task = Task.Factory.StartNewSta(() => PlansDataProvider.Instance.GetPlan(planGuid));
			Task.WaitAll();

			var plan = task.Result;

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
	}
}
