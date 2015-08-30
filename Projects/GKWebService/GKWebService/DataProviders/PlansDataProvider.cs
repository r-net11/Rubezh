#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using GKWebService.Models;
using Infrustructure.Plans.Elements;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

#endregion

namespace GKWebService.DataProviders
{
    public class PlansDataProvider
    {
        private PlansDataProvider()
        {
            Plans = new List<PlanSimpl>();
        }

        private static PlansDataProvider _instance;

        public static PlansDataProvider Instance
            => _instance ?? (_instance = new PlansDataProvider());

        public List<PlanSimpl> Plans { get; set; }

        public void LoadPlans()
        {
            var plans = FiresecManager.PlansConfiguration.Plans;
            Plans = new List<PlanSimpl>();
            foreach (var plan in plans)
            {
                //var z = GKManager.Zones;
                //var dc = GKManager.DeviceConfiguration;
                //var lc = GKManager.DeviceLibraryConfiguration;
                //var dir = GKManager.Directions;
                //var dev = GKManager.Devices;
                //var drv = GKManager.Drivers;
                //var door = GKManager.Doors;
                //var gz = GKManager.GuardZones;
                //var drvc = GKManager.DriversConfiguration;
                //var mpt = GKManager.MPTs;
                //var ps = GKManager.PumpStations;
                //var skdz = GKManager.SKDZones;
                //var pt = GKManager.ParameterTemplates;
                var planToAdd = new PlanSimpl
                                {
                                    Name = plan.Caption,
                                    Uid = plan.UID,
                                    Description = plan.Description,
                                    Width = plan.Width,
                                    Height = plan.Height
                                };
                planToAdd.Elements = new List<PlanElement>
                                     {
                                         new PlanElement
                                         {
                                             Border = ConvertColor(Colors.Black),
                                             BorderThickness = 0,
                                             Fill = ConvertColor(plan.BackgroundColor),
                                             Id = plan.UID,
                                             Name = plan.Caption,
                                             Hint = plan.Description,
                                             Path =
                                                 "M 0 0 L " + plan.Width + " 0 L " + plan.Width +
                                                 " " + plan.Height +
                                                 " L 0 " + plan.Height + " L 0 0 z",
                                             Type = ShapeTypes.Plan.ToString()
                                         }
                                     };
                // Конвертим зоны-полигоны
                foreach (var planElement in plan.ElementPolygonGKDirections)
                {
                    var elemToAdd = PolygonToShape(planElement);
                    elemToAdd.Hint =
                        GKManager.Directions.FirstOrDefault(d => d.UID == planElement.DirectionUID)?
                                 .PresentationName;
                    planToAdd.Elements.Add(elemToAdd);
                }

                // Конвертим зоны-прямоугольники
                foreach (var planElement in plan.ElementRectangleGKDirections)
                {
                    var elemToAdd = RectangleToShape(planElement);
                    elemToAdd.Hint =
                        GKManager.Directions.FirstOrDefault(d => d.UID == planElement.DirectionUID)?
                                 .PresentationName;
                    planToAdd.Elements.Add(elemToAdd);
                }

                // Конвертим устройства
                foreach (var planElement in plan.ElementGKDevices)
                {
                    var elemToAdd = DeviceToShape(planElement);
                    planToAdd.Elements.Add(elemToAdd);
                }

                // TODO: законвертить остальные элементы

                Plans.Add(planToAdd);
            }
        }

        private PlanElement PolygonToShape(ElementBasePolygon item)
        {
            var shape = new PlanElement
                        {
                            Path = PointsToPath(item.Points),
                            Border = ConvertColor(item.BorderColor),
                            Fill = ConvertColor(item.BackgroundColor),
                            BorderMouseOver = ConvertColor(item.BorderColor),
                            FillMouseOver = ConvertColor(item.BackgroundColor),
                            Name = item.PresentationName,
                            Id = item.UID,
                            BorderThickness = item.BorderThickness,
                            Type = ShapeTypes.Path.ToString()
                        };
            return shape;
        }

        private PlanElement DeviceToShape(ElementGKDevice item)
        {
            var device =
                GKManager.Devices.FirstOrDefault(d => d.UID == item.DeviceUID);
            var imagePath = device?.ImageSource.Replace("/Controls;component/", "");
            var imageData = GetImageResource(imagePath);

            var shape = new PlanElement
                        {
                            Name = item.PresentationName,
                            Id = item.UID,
                            Image = imageData.Item1,
                            Hint = device?.PresentationName,
                            X = item.Left,
                            Y = item.Top,
                            Height = imageData.Item2.Height,
                            Width = imageData.Item2.Width,
                            Type = ShapeTypes.GkDevice.ToString()
                        };
            return shape;
        }

        private PlanElement RectangleToShape(ElementBaseRectangle item)
        {
            var pt = new PointCollection
                     {
                         item.GetRectangle().TopLeft,
                         item.GetRectangle().TopRight,
                         item.GetRectangle().BottomRight,
                         item.GetRectangle().BottomLeft
                     };
            var shape = new PlanElement
            {
                Path = PointsToPath(pt),
                Border = ConvertColor(item.BorderColor),
                Fill = ConvertColor(item.BackgroundColor),
                BorderMouseOver = ConvertColor(item.BorderColor),
                FillMouseOver = ConvertColor(item.BackgroundColor),
                Name = item.PresentationName,
                Id = item.UID,
                BorderThickness = item.BorderThickness,
                Type = ShapeTypes.Path.ToString()
            };
            return shape;
        }


        #region Utils
        /// <summary>
        /// Получение иконок для устройств из ресурсов проекта Controls
        /// </summary>
        /// <param name="resName">Путь к ресурсу формата GKIcons/RSR2_Bush_Fire.png</param>
        /// <returns></returns>
        private Tuple<string, Size> GetImageResource(string resName)
        {
            var assembly = Assembly.GetAssembly(typeof(Controls.AlarmButton));
            var name =
                assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(".resources"));
            var resourceStream = assembly.GetManifestResourceStream(name);
            if (resourceStream == null)
                return new Tuple<string, Size>("", new Size());
            byte[] values;
            string type;
            using (var reader = new ResourceReader(resourceStream))
                reader.GetResourceData(resName.ToLowerInvariant(), out type, out values);

            const int offset = 4;
            var size = BitConverter.ToInt32(values, 0);
            var value1 = new Bitmap(new MemoryStream(values, offset, size));
            byte[] byteArray;
            using (var stream = new MemoryStream())
            {
                value1.Save(stream, ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }

            return new Tuple<string, Size>(Convert.ToBase64String(byteArray), value1.Size);
        }

        private string PointsToPath(PointCollection points)
        {
            var enumerable = points.ToArray();
            if (!enumerable.Any())
                return string.Empty;

            var start = enumerable[0];
            var segments = new List<LineSegment>();
            for (var i = 1; i < enumerable.Length; i++)
                segments.Add(new LineSegment(new Point(enumerable[i].X, enumerable[i].Y), true));
            var figure = new PathFigure(new Point(start.X, start.Y), segments, true);
            //true if closed
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry.ToString();
        }


        private System.Drawing.Color ConvertColor(Color source)
        {
            return System.Drawing.Color.FromArgb(source.A, source.R, source.G, source.B);
        } 
        #endregion
    }
}