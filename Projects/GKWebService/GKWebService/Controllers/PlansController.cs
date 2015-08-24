using System;
using System.Linq;
using System.Web.Mvc;
using GKWebService.DataProviders;
using Microsoft.Practices.Unity;

namespace GKWebService.Controllers
{
    public class PlansController : Controller
    {
        //public JsonResult GetShapes()
        //{
        //    //FiresecManager.PlansConfiguration.Plans[0].Children
        //GKManager.Zones.FirstOrDefault(x => x.UID)

        //    var shape = new Shape
        //    {
        //        Id = 1,
        //        Name = "Sample Triangle 1",
        //        Fill = Color.Blue,
        //        Border = Color.Red,
        //        FillMouseOver = Color.Red,
        //        BorderMouseOver = Color.Blue,
        //        Path = "M 0 0 L 80 0 L 40 80 L 0 0 z"
        //    };
        //    var shape1 = new Shape
        //    {
        //        Id = 2,
        //        Name = "Sample Triangle 2",
        //        Fill = Color.Red,
        //        Border = Color.Blue,
        //        FillMouseOver = Color.Blue,
        //        BorderMouseOver = Color.Red,
        //        Path = "M 0 90 L 80 130 L 0 170 L 0 90 z"
        //    };
        //    var result = new List<Shape>
        //                 {
        //                     shape,
        //                     shape1
        //                 };
        //    return Json(result);
        //}

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GetPlans()
        {
            
            var result = Json(PlansDataProvider.Instance.Plans, JsonRequestBehavior.AllowGet);
            
            return result;
        }

        public ActionResult GetPlan(Guid planGuid)
        {
            var plan =
                PlansDataProvider.Instance.Plans.FirstOrDefault(p => p.Uid == planGuid);


            if (plan != null)
            {
                var result = Json(plan.Elements, JsonRequestBehavior.AllowGet);
                return result;
            }
            else return HttpNotFound($"План с ID {planGuid} не найден");
        } 
    }
}