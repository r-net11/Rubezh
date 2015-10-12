#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.GK;
using FiresecAPI.Models;
using RubezhClient;
using GKWebService.Models;
using Infrastructure.Common.Services.Content;
using Infrustructure.Plans.Elements;
using System.Windows.Controls;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Xaml;
using ImageMagick;

#endregion

namespace GKWebService.DataProviders
{
	public class PlansDataProvider
	{
		#region props

		private readonly ContentService _contentService;
		public List<PlanSimpl> Plans
		{ get; set; }

		#endregion

		#region singleton
		private static PlansDataProvider _instance;

		public static PlansDataProvider Instance
		{
			get
			{
				if (_instance != null)
					return _instance;
				return _instance = new PlansDataProvider();
			}
		}
		#endregion

		#region ctor
		private PlansDataProvider()
		{
			Plans = new List<PlanSimpl>();
			_contentService = new ContentService("GKOPC");

		}

		#endregion

		#region plan loading

		public void LoadPlans()
		{
			var plans = ClientManager.PlansConfiguration.Plans;

			Plans = new List<PlanSimpl>();

			// TODO: вынести динамику плана в отдельный код
			SafeFiresecService.GKCallbackResultEvent += SafeFiresecService_GKCallbackResultEvent;

			foreach (var plan in plans)
			{
				LoadPlan(plan);
			}
		}

		private void LoadPlan(Plan plan)
		{
			// Корень плана
			var planToAdd = new PlanSimpl
			{
				Name = plan.Caption,
				Uid = plan.UID,
				Description = plan.Description,
				Width = plan.Width,
				Height = plan.Height,
				Elements = new List<PlanElement>()
			};

			// Добавляем сам план с фоном
			var planRootElement = new PlanElement
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
				Image = GetDrawingContent(plan.BackgroundImageSource,
														 Convert.ToInt32(plan.Width),
														 Convert.ToInt32(plan.Height)),
				Width = plan.Width,
				Height = plan.Height
			};

			planToAdd.Elements.Add(planRootElement);

			// Конвертим и добавляем прямоугольные элементы
			var rectangles = LoadRectangleElements(plan);
			planToAdd.Elements.AddRange(rectangles);

			// Конвертим и добавляем полигональные элементы
			var polygons = LoadPolygonElements(plan);
			planToAdd.Elements.AddRange(polygons);

			// Конвертим и добавляем линии-элементы
			var polylines = LoadPolyLineElements(plan);
			planToAdd.Elements.AddRange(polylines);

			//var doors = LoadDoorElements(plan);
			//planToAdd.Elements.AddRange(doors);


			// Конвертим и добавляем устройства
			foreach (var planElement in plan.ElementGKDevices)
			{
				var elemToAdd = DeviceToShape(planElement);
				planToAdd.Elements.Add(elemToAdd);
			}

			// TODO: законвертить остальные элементы

			Plans.Add(planToAdd);
		}

		//private IEnumerable<PlanElement> LoadDoorElements(Plan plan)
		//{
		//	var result = new List<PlanElement>();
		//	foreach (var door in plan.ElementGKDoors)
		//	{
		//		var elemToAdd = DoorToShape(door);
		//		elemToAdd.Hint = GetElementHint(door);
		//		result.Add(elemToAdd);
		//	}
		//	return result;
		//}

		//private PlanElement DoorToShape(ElementGKDoor item)
		//{
		//	GKDoor door =
		//		GKManager.Doors.FirstOrDefault(d => d.UID == item.DoorUID);
		//	if (door == null)
		//		return null;

		//	var bytes = GetDeviceStatePic(device);

		//	// Создаем элемент плана
		//	// Ширину и высоту задаем 500, т.к. знаем об этом
		//	var shape = new PlanElement
		//	{
		//		Name = item.PresentationName,
		//		Id = item.DeviceUID,
		//		Image = Convert.ToBase64String(bytes),
		//		Hint = GetElementHint(item),
		//		X = item.Left - 7,
		//		Y = item.Top - 7,
		//		Height = 14,
		//		Width = 14,
		//		Type = ShapeTypes.GkDevice.ToString()
		//	};
		//	return shape;
		//}

		private IEnumerable<PlanElement> LoadPolygonElements(Plan plan)
		{
			var result = new List<PlanElement>();
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
				elemToAdd.Hint = GetElementHint(polygon);
				result.Add(elemToAdd);
			}
			return result;
		}

		private IEnumerable<PlanElement> LoadRectangleElements(Plan plan)
		{
			var result = new List<PlanElement>();
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

				elemToAdd.Hint = GetElementHint(rectangle);


				result.Add(elemToAdd);
			}
			return result;
		}

		private string GetElementHint(ElementBase element)
		{
			if (element is IElementZone)
			{
				var asZone = element as IElementZone;
				if (element is ElementRectangleGKZone || element is ElementPolygonGKZone)
				{
					var zone = GKManager.Zones.FirstOrDefault(z => asZone != null && z.UID == asZone.ZoneUID);
					if (zone != null)
					{
						if (zone.PresentationName != null)
						{
							return zone.PresentationName;
						}
					}
				}
				if (element is ElementRectangleGKGuardZone || element is ElementPolygonGKGuardZone)
				{
					var zone = GKManager.GuardZones.FirstOrDefault(z => asZone != null && z.UID == asZone.ZoneUID);
					if (zone != null)
					{
						if (zone.PresentationName != null)
						{
							return zone.PresentationName;
						}
					}
				}
				if (element is ElementRectangleGKSKDZone || element is ElementPolygonGKSKDZone)
				{
					var zone = GKManager.SKDZones.FirstOrDefault(z => asZone != null && z.UID == asZone.ZoneUID);
					if (zone != null)
					{
						if (zone.PresentationName != null)
						{
							return zone.PresentationName;
						}
					}
				}
			}
			if (element is IElementMPT)
			{
				var asMPT = element as IElementMPT;
				var MPT = GKManager.MPTs.FirstOrDefault(m => asMPT != null && m.UID == asMPT.MPTUID);
				if (MPT != null)
				{
					if (MPT.PresentationName != null)
					{
						return MPT.PresentationName;
					}
				}
			}
			if (element is IElementDelay)
			{
				var asDelay = element as IElementDelay;
				var delay = GKManager.Delays.FirstOrDefault(m => asDelay != null && m.UID == asDelay.DelayUID);
				if (delay != null)
				{
					if (delay.PresentationName != null)
					{
						return delay.PresentationName;
					}
				}
			}
			if (element is IElementDirection)
			{
				var asDirection = element as IElementDirection;
				var direction = GKManager.Directions.FirstOrDefault(
					d => asDirection != null && d.UID == asDirection.DirectionUID);
				if (direction != null)
				{
					if (direction.PresentationName != null)
					{
						return direction.PresentationName;
					}
				}
			}
			if (element is ElementGKDevice)
			{
				var device = GKManager.Devices.FirstOrDefault(
					d => element != null && d.UID == (element as ElementGKDevice).DeviceUID);
				if (device != null)
				{
					if (device.PresentationName != null)
					{
						return device.PresentationName;
					}
				}
			}
			return string.Empty;
		}

		private IEnumerable<PlanElement> LoadPolyLineElements(Plan plan)
		{
			var result = new List<PlanElement>();
			var polylines = (from line in plan.ElementPolylines
							 select line as ElementBasePolyline);

			// Конвертим зоны-полигоны
			foreach (var polyline in polylines)
			{
				var elemToAdd = PolylineToShape(polyline);
				elemToAdd.Hint = GetElementHint(polyline);
				result.Add(elemToAdd);
			}
			return result;
		}

		#endregion

		#region type converters, resource loaders

		/// <summary>
		/// Получает преобразованное в Base64String png-изображение фона плана
		/// </summary>
		/// <param name="source">GUID плана</param>
		/// <param name="width">Ширина плана</param>
		/// <param name="height">Высота плана</param>
		/// <returns></returns>
		private string GetDrawingContent(Guid? source, int width, int height)
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

			var pngBytes = XamlDrawingToPngImageBytes(width, height, drawing);
			return Convert.ToBase64String(pngBytes);
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
				Path = PointsToPath(pt, true),
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
		private PlanElement PolygonToShape(ElementBasePolygon item)
		{
			var shape = new PlanElement
			{
				Path = PointsToPath(item.Points, true),
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

		private PlanElement PolylineToShape(ElementBasePolyline item)
		{
			var shape = new PlanElement
			{
				Path = PointsToPath(item.Points, false),
				Border = ConvertColor(item.BorderColor),
				Fill = System.Drawing.Color.Transparent,
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
			// Находим девайс и набор его состояний
			var device =
				GKManager.Devices.FirstOrDefault(d => d.UID == item.DeviceUID);
			if (device == null)
				return null;

			var bytes = GetDeviceStatePic(device);

			// Создаем элемент плана
			// Ширину и высоту задаем 500, т.к. знаем об этом
			var shape = new PlanElement
			{
				Name = item.PresentationName,
				Id = item.DeviceUID,
				Image = Convert.ToBase64String(bytes),
				Hint = GetElementHint(item),
				X = item.Left - 7,
				Y = item.Top - 7,
				Height = 14,
				Width = 14,
				Type = ShapeTypes.GkDevice.ToString()
			};
			return shape;
		}

		//private byte[] GetDoorPic(GKDoor device)
		//{
		//	var stateWithPic =
		//		device.States.FirstOrDefault(s => s.StateClass == device.State.StateClass) ??
		//		deviceConfig.States.FirstOrDefault(s => s.StateClass == XStateClass.No);
		//	if (stateWithPic == null)
		//		return null;

		//	// Перебираем кадры в состоянии и генерируем gif картинку
		//	byte[] bytes;
		//	using (MagickImageCollection collection = new MagickImageCollection())
		//	{
		//		foreach (var frame in stateWithPic.Frames)
		//		{
		//			var frame1 = frame;
		//			Task<Bitmap> getBitmapTask = Task.Factory.StartNewSta(() =>
		//			{
		//				Canvas surface =
		//					(Canvas)XamlFromString(frame1.Image);
		//				return XamlCanvasToPngBitmap(surface);
		//			});
		//			Task.WaitAll(getBitmapTask);
		//			var pngBitmap = getBitmapTask.Result;
		//			MagickImage img = new MagickImage(pngBitmap)
		//			{
		//				AnimationDelay = frame.Duration / 10,
		//			};
		//			collection.Add(img);
		//		}
		//		var path = string.Format(@"C:\tmpImage{0}.gif", Guid.NewGuid());
		//		collection.Write(path);
		//		using (var fstream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
		//		{
		//			using (var stream = new MemoryStream())
		//			{
		//				fstream.CopyTo(stream);
		//				bytes = stream.ToArray();
		//			}
		//		}
		//		File.Delete(path);
		//	}
		//	return bytes;

		//}

		private byte[] GetDeviceStatePic(GKDevice device)
		{
			var deviceConfig =
				GKManager.DeviceLibraryConfiguration.GKDevices.FirstOrDefault(d => d.DriverUID == device.DriverUID);
			if (deviceConfig == null)
				return null;
			var stateWithPic =
				deviceConfig.States.FirstOrDefault(s => s.StateClass == device.State.StateClass) ??
				deviceConfig.States.FirstOrDefault(s => s.StateClass == XStateClass.No);
			if (stateWithPic == null)
				return null;

			// Перебираем кадры в состоянии и генерируем gif картинку
			byte[] bytes;
			using (MagickImageCollection collection = new MagickImageCollection())
			{
				foreach (var frame in stateWithPic.Frames)
				{
					var frame1 = frame;
					Task<Bitmap> getBitmapTask = Task.Factory.StartNewSta(() =>
																		  {
																			  Canvas surface =
																				  (Canvas)XamlFromString(frame1.Image);
																			  return XamlCanvasToPngBitmap(surface);
																		  });
					Task.WaitAll(getBitmapTask);
					var pngBitmap = getBitmapTask.Result;
					MagickImage img = new MagickImage(pngBitmap)
					{
						AnimationDelay = frame.Duration / 10,
					};
					collection.Add(img);
				}
				var path = string.Format(@"C:\tmpImage{0}.gif", Guid.NewGuid());
				collection.Write(path);
				using (var fstream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (var stream = new MemoryStream())
					{
						fstream.CopyTo(stream);
						bytes = stream.ToArray();
					}
				}
				File.Delete(path);
			}
			return bytes;
		}

		private Tuple<string, Size> GetElementHintIcon(ElementBasePoint item)
		{
			string imagePath = string.Empty;

			if (item is ElementGKDevice)
			{
				var device =
				 GKManager.Devices.FirstOrDefault(d => d.UID == (item as ElementGKDevice).DeviceUID);
				if (device == null)
					return null;

				var deviceConfig =
					GKManager.DeviceLibraryConfiguration.GKDevices.FirstOrDefault(d => d.DriverUID == device.DriverUID);
				if (deviceConfig == null)
					return null;

				var stateWithPic =
					deviceConfig.States.FirstOrDefault(s => s.StateClass == device.State.StateClass) ??
					deviceConfig.States.FirstOrDefault(s => s.StateClass == XStateClass.No);
				imagePath = device.ImageSource.Replace("/Controls;component/", "");
			}
			if (item is ElementGKDoor)
			{
				var door =
				 GKManager.Doors.FirstOrDefault(d => d.UID == (item as ElementGKDoor).DoorUID);
				if (door == null)
					return null;

				imagePath = door.ImageSource.Replace("/Controls;component/", "");
			}
			
			var imageData = GetImageResource(imagePath);

			return imageData;
		}

		/// <summary>
		/// Получение иконок для хинтов из ресурсов проекта Controls
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

		#endregion

		#region dynamic behaviour

		private void SafeFiresecService_GKCallbackResultEvent(GKCallbackResult obj)
		{
			var states = obj.GKStates;
			foreach (var state in states.DelayStates)
			{

			}
			foreach (var state in states.DeviceStates)
			{
				var updated = UpdateDeviceState(state);
			}
			foreach (var state in states.DirectionStates)
			{

			}
			foreach (var state in states.DoorStates)
			{

			}
			foreach (var state in states.GuardZoneStates)
			{

			}
			foreach (var state in states.MPTStates)
			{

			}
			foreach (var state in states.PumpStationStates)
			{

			}
			foreach (var state in states.SKDZoneStates)
			{

			}
			foreach (var state in states.ZoneStates)
			{

			}
		}

		private bool UpdateDeviceState(GKState state)
		{
			if (state.BaseObjectType != GKBaseObjectType.Device)
			{
				return false;
			}
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == state.UID);
			// Получаем обновленную картинку устройства
			var pictureTask = Task.Factory.StartNewSta<byte[]>(() => GetDeviceStatePic(device));
			Task.WaitAll();
			var pic = pictureTask.Result;
			if (pic == null)
			{
				return false;
			}
			var statusUpdate = new
			{
				Id = state.UID,
				Picture = Convert.ToBase64String(pic),
				StateClass = state.StateClass.ToString(),
				StateClasses = state.StateClasses,
				AdditionalStates = state.AdditionalStates
			};
			PlansUpdater.Instance.UpdateDeviceState(statusUpdate);
			return true;
		}

		#endregion

		#region Utils

		/// <summary>
		/// Преобразует XAML Drawing/DrawingGroup в png
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="drawing"></param>
		/// <returns></returns>
		private static byte[] XamlDrawingToPngImageBytes(int width, int height, Drawing drawing)
		{
			var bitmapEncoder = new PngBitmapEncoder();

			// The image parameters...
			double dpiX = 96;
			double dpiY = 96;

			// The Visual to use as the source of the RenderTargetBitmap.
			DrawingVisual drawingVisual = new DrawingVisual();
			using (var drawingContext = drawingVisual.RenderOpen())
				drawingContext.DrawDrawing(drawing);

			var bounds = drawingVisual.ContentBounds;

			RenderTargetBitmap targetBitmap = new RenderTargetBitmap(width * 10, height * 10, dpiX, dpiY,
																	 PixelFormats.Pbgra32);
			drawingVisual.Transform = new ScaleTransform(width * 10 / bounds.Width, height * 10 / bounds.Height);


			targetBitmap.Render(drawingVisual);

			// Encoding the RenderBitmapTarget as an image.
			bitmapEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));

			byte[] values;
			using (var str = new MemoryStream())
			{
				bitmapEncoder.Save(str);
				values = str.ToArray();
			}
			return values;
		}

		/// <summary>
		/// Преобразует Xaml-canvas в png изображение
		/// </summary>
		/// <param name="surface">XAML Canvas</param>
		/// <returns>Bitmap, rendered from XAML</returns>
		private Bitmap XamlCanvasToPngBitmap(Canvas surface)
		{

			// Save current canvas transform
			Transform transform = surface.LayoutTransform;
			// reset current transform (in case it is scaled or rotated)
			surface.LayoutTransform = null;

			// Get the size of canvas
			var size = new System.Windows.Size(surface.Width, surface.Height);
			// Measure and arrange the surface
			// VERY IMPORTANT
			surface.Measure(size);
			surface.Arrange(new Rect(size));

			// Create a render bitmap and push the surface to it
			RenderTargetBitmap renderBitmap =
				new RenderTargetBitmap(
					(int)size.Width,
					(int)size.Height,
					96d,
					96d,
					PixelFormats.Pbgra32);
			renderBitmap.Render(surface);

			Bitmap bmp;
			// Create a file stream for saving image
			using (var stream = new MemoryStream())
			{
				// Use png encoder for our data
				PngBitmapEncoder encoder = new PngBitmapEncoder();
				// push the rendered bitmap to it
				encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
				// save the data to the stream
				encoder.Save(stream);

				bmp = new Bitmap(stream);
			}
			return bmp;
		}

		private string PointsToPath(PointCollection points, bool isClosed)
		{
			var enumerable = points.ToArray();
			if (!enumerable.Any())
				return string.Empty;

			var start = enumerable[0];
			var segments = new List<LineSegment>();
			for (var i = 1; i < enumerable.Length; i++)
				segments.Add(new LineSegment(new Point(enumerable[i].X, enumerable[i].Y), true));
			var figure = new PathFigure(new Point(start.X, start.Y), segments, isClosed);
			var geometry = new PathGeometry();
			geometry.Figures.Add(figure);
			return geometry.ToString();
		}

		private MemoryStream GenerateStreamFromString(string value)
		{
			return new MemoryStream(Encoding.Unicode.GetBytes(value ?? ""));
		}

		private object XamlFromString(string source)
		{
			return XamlServices.Load(GenerateStreamFromString(source));
		}

		private System.Drawing.Color ConvertColor(Color source)
		{
			return System.Drawing.Color.FromArgb(source.A, source.R, source.G, source.B);
		}

		#endregion
	}
}