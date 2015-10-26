#region Usings

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKWebService.Models;
using ImageMagick;
using Infrustructure.Plans.Elements;

#endregion

namespace GKWebService.DataProviders
{
	public partial class PlansDataProvider
	{
		#region type converters, resource loaders

		/// <summary>
		///     Получает преобразованное в Base64String png-изображение фона плана
		/// </summary>
		/// <param name="source">GUID плана</param>
		/// <param name="width">Ширина плана</param>
		/// <param name="height">Высота плана</param>
		/// <returns></returns>
		private string GetDrawingContent(Guid? source, int width, int height) {
			Drawing drawing = null;
			if (source.HasValue) {
				drawing = _contentService.GetDrawing(source.Value);
			}
			else {
				return string.Empty;
			}

			drawing.Freeze();

			var pngBytes = InernalConverter.XamlDrawingToPngImageBytes(width, height, drawing);
			return Convert.ToBase64String(pngBytes);
		}

		private PlanElement RectangleToShape(ElementBaseRectangle item) {
			var pt = new PointCollection {
				item.GetRectangle().TopLeft,
				item.GetRectangle().TopRight,
				item.GetRectangle().BottomRight,
				item.GetRectangle().BottomLeft
			};
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(pt, true),
				Border = InernalConverter.ConvertColor(item.BorderColor),
				Fill = InernalConverter.ConvertColor(item.BackgroundColor),
				BorderMouseOver = InernalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InernalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		private PlanElement PolygonToShape(ElementBasePolygon item) {
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(item.Points, true),
				Border = InernalConverter.ConvertColor(item.BorderColor),
				Fill = InernalConverter.ConvertColor(item.BackgroundColor),
				BorderMouseOver = InernalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InernalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		private PlanElement PolylineToShape(ElementBasePolyline item) {
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(item.Points, false),
				Border = InernalConverter.ConvertColor(item.BorderColor),
				Fill = System.Drawing.Color.Transparent,
				BorderMouseOver = InernalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InernalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		private PlanElement DeviceToShape(ElementGKDevice item) {
			// Находим девайс и набор его состояний
			var device =
				GKManager.Devices.FirstOrDefault(d => d.UID == item.DeviceUID);
			if (device == null) {
				return null;
			}

			var bytes = GetDeviceStatePic(device);

			// Создаем элемент плана
			// Ширину и высоту задаем 500, т.к. знаем об этом
			var shape = new PlanElement {
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

		private byte[] GetDeviceStatePic(GKDevice device) {
			var deviceConfig =
				GKManager.DeviceLibraryConfiguration.GKDevices.FirstOrDefault(d => d.DriverUID == device.DriverUID);
			if (deviceConfig == null) {
				return null;
			}
			var stateWithPic =
				deviceConfig.States.FirstOrDefault(s => s.StateClass == device.State.StateClass) ??
				deviceConfig.States.FirstOrDefault(s => s.StateClass == XStateClass.No);
			if (stateWithPic == null) {
				return null;
			}

			// Перебираем кадры в состоянии и генерируем gif картинку
			byte[] bytes;
			using (var collection = new MagickImageCollection()) {
				foreach (var frame in stateWithPic.Frames) {
					var frame1 = frame;
					var getBitmapTask = Task.Factory.StartNewSta(
						() => {
							var surface =
								(Canvas)InernalConverter.XamlFromString(frame1.Image);
							return InernalConverter.XamlCanvasToPngBitmap(surface);
						});
					Task.WaitAll(getBitmapTask);
					var pngBitmap = getBitmapTask.Result;
					var img = new MagickImage(pngBitmap) {
						AnimationDelay = frame.Duration / 10
					};
					collection.Add(img);
				}
				var path = string.Format(@"C:\tmpImage{0}.gif", Guid.NewGuid());
				collection.Write(path);
				using (var fstream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					using (var stream = new MemoryStream()) {
						fstream.CopyTo(stream);
						bytes = stream.ToArray();
					}
				}
				File.Delete(path);
			}
			return bytes;
		}

		private Tuple<string, Size> GetElementHintIcon(ElementBasePoint item) {
			var imagePath = string.Empty;

			if (item is ElementGKDevice) {
				var device =
					GKManager.Devices.FirstOrDefault(d => d.UID == (item as ElementGKDevice).DeviceUID);
				if (device == null) {
					return null;
				}

				var deviceConfig =
					GKManager.DeviceLibraryConfiguration.GKDevices.FirstOrDefault(d => d.DriverUID == device.DriverUID);
				if (deviceConfig == null) {
					return null;
				}

				var stateWithPic =
					deviceConfig.States.FirstOrDefault(s => s.StateClass == device.State.StateClass) ??
					deviceConfig.States.FirstOrDefault(s => s.StateClass == XStateClass.No);
				imagePath = device.ImageSource.Replace("/Controls;component/", "");
			}
			if (item is ElementGKDoor) {
				var door =
					GKManager.Doors.FirstOrDefault(d => d.UID == (item as ElementGKDoor).DoorUID);
				if (door == null) {
					return null;
				}

				imagePath = door.ImageSource.Replace("/Controls;component/", "");
			}

			var imageData = GetImageResource(imagePath);

			return imageData;
		}

		/// <summary>
		///     Получение иконок для хинтов из ресурсов проекта Controls
		/// </summary>
		/// <param name="resName">Путь к ресурсу формата GKIcons/RSR2_Bush_Fire.png</param>
		/// <returns></returns>
		private Tuple<string, Size> GetImageResource(string resName) {
			var assembly = Assembly.GetAssembly(typeof (Controls.AlarmButton));
			var name =
				assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(".resources"));
			var resourceStream = assembly.GetManifestResourceStream(name);
			if (resourceStream == null) {
				return new Tuple<string, Size>("", new Size());
			}
			byte[] values;
			string type;
			using (var reader = new ResourceReader(resourceStream)) {
				reader.GetResourceData(resName.ToLowerInvariant(), out type, out values);
			}

			// Получили массив байтов ресурса, теперь преобразуем его в png bitmap, а потом снова в массив битов
			// уже корректного формата, после чего преобразуем его в base64string для удобства обратного преобразования
			// на клиенте

			const int offset = 4;
			var size = BitConverter.ToInt32(values, 0);
			var value1 = new Bitmap(new MemoryStream(values, offset, size));
			byte[] byteArray;
			using (var stream = new MemoryStream()) {
				value1.Save(stream, ImageFormat.Png);
				stream.Close();

				byteArray = stream.ToArray();
			}

			return new Tuple<string, Size>(Convert.ToBase64String(byteArray), value1.Size);
		}

		#endregion
	}
}
