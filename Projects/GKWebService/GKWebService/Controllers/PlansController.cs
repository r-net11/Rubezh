using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKWebService.Models;
using Color = System.Drawing.Color;

namespace GKWebService.Controllers
{
    public class PlansController : Controller
    {
        //public JsonResult GetShapes()
        //{
        //    //FiresecManager.PlansConfiguration.Plans[0].Children
        //    //GKManager.Zones.FirstOrDefault(x => x.UID)

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


        public ActionResult GetPlans()
        {
            var plans = FiresecManager.PlansConfiguration.Plans;
            List<object> lightPlans = new List<object>();
            foreach (var plan in plans)
            {
                lightPlans.Add(new {name = plan.Caption, id = plan.UID, description = plan.Description});
            }
            var result = Json(lightPlans, JsonRequestBehavior.AllowGet);
            return result;
        }

        public JsonResult GetPlan(Guid planGuid)
        {
            var plan =
                FiresecManager.PlansConfiguration.Plans
                              .FirstOrDefault(p => p.UID == planGuid);
            var width = plan.Width;
            var shapes = new List<Shape>();
            shapes.Add(new Shape()
                       {
                           Border = Colors.Black,
                           Fill = plan.BackgroundColor,
                           Id = plan.UID,
                           Name = plan.Caption,
                           Path =
                               "M 0 0 L " + plan.Width + " 0 L " + plan.Width + " " + plan.Height +
                               " L 0 " + plan.Height + " L 0 0 z"
                       });
            // Конвертим полигоны
            foreach (var planElement in plan.ElementPolygonGKDirections)
            {
                var pnt = new PointCollection() {new Point(1,1), new Point(1,1), new Point(2,2)};

                //ManualResetEvent manEvent = new ManualResetEvent(false);
                //ThreadPool.QueueUserWorkItem((o) =>
                //{

                List<Tuple<double, double>> points =
                                pnt.Select(
                                    point =>
                                    new Tuple<double, double>(point.X, point.Y))
                                           .ToList();

                shapes.Add(PolygonToShape(points, planElement));
				IEnumerable<Point> arrPoints = null;
				planElement.Points.Dispatcher.Invoke(() =>
					{
						arrPoints = planElement.Points.ToArray();
					});
				//var temp = planElement.Points.Select(p => new Tuple<double, double>(p.X, p.Y));
                //planElement.Points.Dispatcher.Invoke(() =>
                //        {
                //            List<Tuple<double, double>> points =
                //                pnt.Select(
                //                    point =>
                //                    new Tuple<double, double>(point.X, point.Y))
                //                           .ToList();

                //            shapes.Add(PolygonToShape(points, planElement));

                //        });

                //    manEvent.Set();
                //});
                //manEvent.WaitOne();
            }
            

            var result = Json(shapes, JsonRequestBehavior.AllowGet);
            return result;
        }

        private Shape PolygonToShape(List<Tuple<double, double>> points, ElementPolygonGKDirection item)
        {
            var shape = new Shape
                        {
                            Path = PointsToPath(points),
                            Border = item.BorderColor,
                            Fill = item.BackgroundColor,
                            BorderMouseOver = item.BorderColor,
                            FillMouseOver = item.BackgroundColor,
                            Name = item.PresentationName,
                            Id = item.UID
                        };
            return shape;
        }

        private string PointsToPath(IEnumerable<Tuple<double, double>> points)
        {
            var enumerable = points as Tuple<double, double>[] ?? points.ToArray();
            if (enumerable.Any())
            {
                Tuple<double, double> start = enumerable[0];
                List<LineSegment> segments = new List<LineSegment>();
                for (int i = 1; i < enumerable.Length; i++)
                {
                    segments.Add(new LineSegment(new Point(enumerable[i].Item1, enumerable[i].Item2), true));
                }
                PathFigure figure = new PathFigure(new Point(start.Item1, start.Item2), segments, false); //true if closed
                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);
                return geometry.ToString();
            }
            return string.Empty;
        }
    }
}