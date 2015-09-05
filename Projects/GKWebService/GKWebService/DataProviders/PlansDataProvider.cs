#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKWebService.Models;
using Infrastructure.Common.Services.Content;
using Infrustructure.Plans.Elements;
using System.Windows.Controls;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Brushes = System.Windows.Media.Brushes;

#endregion

namespace GKWebService.DataProviders
{
    public class PlansDataProvider
    {
	    private ContentService _contentService;
        private PlansDataProvider()
        {
            Plans = new List<PlanSimpl>();
			_contentService = new ContentService("GKOPC");

		}

        private static PlansDataProvider _instance;

        public static PlansDataProvider Instance
        {
            get
            {
                if (_instance != null) return _instance;
                return _instance = new PlansDataProvider();
            }
        }

        public List<PlanSimpl> Plans { get; set; }

        public void LoadPlans()
        {
            var plans = FiresecManager.PlansConfiguration.Plans;
            Plans = new List<PlanSimpl>();
            //FileConfigurationHelper.LoadFromFile(fileName);
			SafeFiresecService.GKCallbackResultEvent += SafeFiresecService_GKCallbackResultEvent;
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
				// Добавляем элемент самого плана

	            var plan1 = plan;
	   //         Task<string> getContenTask = Task.Factory.StartNewSta(() => GetContent(plan1.BackgroundImageSource,
	   //                                                                        Convert.ToInt32(plan1.Width),
	   //                                                                        Convert.ToInt32(plan1.Height)));
				//Task.WaitAll(getContenTask);

				// Все остальное
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
                                             Type = ShapeTypes.Plan.ToString(),
											 Image = GetContent(plan1.BackgroundImageSource,
																			   Convert.ToInt32(plan1.Width),
																			   Convert.ToInt32(plan1.Height)),
											 Width = plan.Width,
											 Height = plan.Height
										 }
                                     };
                var rectangles =
                    (from rect in plan.ElementRectangles
                     select rect as ElementBaseRectangle)
                        .Union
                        (from rect in plan.ElementRectangleGKZones
                         select rect as ElementBaseRectangle)
                        .Union
                        (from rect in plan.ElementRectangleGKDelays
                         select rect as ElementBaseRectangle)
                        .Union
                        (from rect in plan.ElementRectangleGKDirections
                         select rect as ElementBaseRectangle)
                        .Union
                        (from rect in plan.ElementRectangleGKGuardZones
                         select rect as ElementBaseRectangle)
                        .Union
                        (from rect in plan.ElementRectangleGKMPTs
                         select rect as ElementBaseRectangle)
                        .Union
                        (from rect in plan.ElementRectangleGKSKDZones
                         select rect as ElementBaseRectangle);


                // Конвертим зоны-прямоугольники
                foreach (var rectangle in rectangles.ToList())
                {
                    var elemToAdd = RectangleToShape(rectangle);
                    var asDirection = rectangle as IElementDirection;

                    var firstOrDefault = GKManager.Directions.FirstOrDefault(
                        d => asDirection != null && d.UID == asDirection.DirectionUID);
                    if (firstOrDefault != null)
                    {
                        if (firstOrDefault.PresentationName != null)
                        {
                            elemToAdd.Hint =
                                firstOrDefault.PresentationName;
                        }
                    }
                    planToAdd.Elements.Add(elemToAdd); 
                }

                var polygons =
                    (from rect in plan.ElementPolygons
                     select rect as ElementBasePolygon)
                        .Union
                        (from rect in plan.ElementPolygonGKZones
                         select rect as ElementBasePolygon)
                        .Union
                        (from rect in plan.ElementPolygonGKDelays
                         select rect as ElementBasePolygon)
                        .Union
                        (from rect in plan.ElementPolygonGKDirections
                         select rect as ElementBasePolygon)
                        .Union
                        (from rect in plan.ElementPolygonGKGuardZones
                         select rect as ElementBasePolygon)
                        .Union
                        (from rect in plan.ElementPolygonGKMPTs
                         select rect as ElementBasePolygon)
                        .Union
                        (from rect in plan.ElementPolygonGKSKDZones
                         select rect as ElementBasePolygon);

                // Конвертим зоны-полигоны
                foreach (var polygon in polygons)
                {
                    var elemToAdd = PolygonToShape(polygon);
                    var asDirection = polygon as IElementDirection;

                    var firstOrDefault = GKManager.Directions.FirstOrDefault(
                        d => asDirection != null && d.UID == asDirection.DirectionUID);
                    if (firstOrDefault != null)
                    {
                        if (firstOrDefault.PresentationName != null)
                        {
                            elemToAdd.Hint =
                                firstOrDefault.PresentationName;
                        }
                    }
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

	    private string GetContent(Guid? source, int width, int height)
	    {
			Drawing drawing = null;
			if (source.HasValue)
			{
				drawing = _contentService.GetDrawing(source.Value);
			}
			else
			{
				return string.Empty;
			}

			drawing.Freeze();

			var bitmapEncoder = new PngBitmapEncoder();

			// The image parameters...
			double dpiX = 96;
			double dpiY = 96;

			// The Visual to use as the source of the RenderTargetBitmap.
			DrawingVisual drawingVisual = new DrawingVisual();
			using (var drawingContext = drawingVisual.RenderOpen())
			{
				drawingContext.DrawDrawing(drawing);
			}

		    var bounds = drawingVisual.ContentBounds;

			RenderTargetBitmap targetBitmap = new RenderTargetBitmap(width*10, height*10, dpiX, dpiY, PixelFormats.Pbgra32);
			drawingVisual.Transform = new ScaleTransform(width*10/bounds.Width, height*10 / bounds.Height);


			targetBitmap.Render(drawingVisual);

			// Encoding the RenderBitmapTarget as an image file.
			bitmapEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));

		    byte[] values;
			using(var str = new MemoryStream())
		    {
			    bitmapEncoder.Save(str);
				values = str.ToArray();
				//bmp = new Bitmap(stream);
				//using (var str = new MemoryStream())
				//{
				//	bmp.Save(str, ImageFormat.Png);
				//	str.Close();

				//	byteArray = str.ToArray();
				//}
				//stream.Position = 0;
				
				
				
				//stream.Position = 0;
				//byteArray = stream.ToArray();
			}
			//const int offset = 4;
			//var size = BitConverter.ToInt32(values, 0);
			//bmp = new Bitmap(new MemoryStream(values, offset, size));
			//byte[] byteArray;
			//using (var stream = new MemoryStream())
			//{
			//	bmp.Save(stream, ImageFormat.Png);
			//	stream.Close();

			//	byteArray = stream.ToArray();
			//}


			return Convert.ToBase64String(values);

			//   Drawing drawing = null;
			//if (source.HasValue)
			//{
			//	drawing = _contentService.GetDrawing(source.Value);
			//}

			//DrawingImage drawingImg = new DrawingImage(drawing);

			//var image = new Image
			//{
			//	Source = drawingImg
			//};
			//image.Arrange(new Rect(0, 0, width, height));  //Required

			////3. Render the Image control's content to a RenderTargetBitmap
			//RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
			//rtb.Render(image);

			//var bitmap = new WriteableBitmap((BitmapSource)image.Source);
			//   Bitmap bmp = null;
			//using (MemoryStream outStream = new MemoryStream())
			//{
			//	BitmapEncoder enc = new BmpBitmapEncoder();

			//	enc.Frames.Add(BitmapFrame.Create(bitmap));
			//	enc.Save(outStream);
			//	bmp = new Bitmap(outStream);
			//}
			//   byte[] byteArray = null;

			//using (var stream = new MemoryStream())
			//{
			//	bmp.Save(stream, ImageFormat.Png);
			//	stream.Close();

			//	byteArray = stream.ToArray();
			//}
			//   return Convert.ToBase64String(byteArray);
		}

		private void SafeFiresecService_GKCallbackResultEvent(GKCallbackResult obj)
		{
			Debug.WriteLine("GK property changed " + obj.GKStates);
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
            var imagePath = device != null ? device.ImageSource.Replace("/Controls;component/", "") : null;
            if (imagePath == null) return null;
            var imageData = GetImageResource(imagePath);

            var shape = new PlanElement
                        {
                            Name = item.PresentationName,
                            Id = item.UID,
                            Image = imageData.Item1,
                            Hint = device.PresentationName,
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
            var assembly = Assembly.GetAssembly(typeof (Controls.AlarmButton));
            var name =
                assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(".resources"));
            var resourceStream = assembly.GetManifestResourceStream(name);
            if (resourceStream == null)
                return new Tuple<string, Size>("", new Size());
            byte[] values;
            string type;
            using (var reader = new ResourceReader(resourceStream))
                reader.GetResourceData(resName.ToLowerInvariant(), out type, out values);

			// Получили массив байтов ресурса, теперь преобразуем его в png bitmap, а потом снова в массив битов
			// уже корректного формата, после чего преобразуем его в base64string для удобства обратного преобразования
			// на клиенте

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