﻿#region Usings

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xaml;
using GKWebService.DataProviders;
using ImageMagick;
using Infrustructure.Plans.Elements;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using FontStyle = System.Windows.FontStyle;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;

#endregion

namespace GKWebService.Models
{
	public class PlanElement : Shape
	{
		public static PlanElement FromRectangle(ElementBaseRectangle item) {
			var rect = item.GetRectangle();
			var pt = new PointCollection {
				rect.TopLeft,
				rect.TopRight,
				rect.BottomRight,
				rect.BottomLeft
			};
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(pt, PathKind.ClosedLine),
				Border = InernalConverter.ConvertColor(item.BorderColor),
				Fill = InernalConverter.ConvertColor(item.BackgroundColor),
				BorderMouseOver = InernalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InernalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = GetElementHint(item),
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		public static PlanElement FromEllipse(ElementEllipse item) {
			var rect = item.GetRectangle();
			var pt = new PointCollection {
				rect.TopLeft,
				rect.TopRight,
				rect.BottomRight,
				rect.BottomLeft
			};
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(pt, PathKind.Ellipse),
				Border = InernalConverter.ConvertColor(item.BorderColor),
				Fill = InernalConverter.ConvertColor(item.BackgroundColor),
				BorderMouseOver = InernalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InernalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = GetElementHint(item),
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		public static PlanElement FromPolygon(ElementBasePolygon item) {
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(item.Points, PathKind.ClosedLine),
				Border = InernalConverter.ConvertColor(item.BorderColor),
				Fill = InernalConverter.ConvertColor(item.BackgroundColor),
				BorderMouseOver = InernalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InernalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				BorderThickness = item.BorderThickness,
				Hint = GetElementHint(item),
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		public static PlanElement FromPolyline(ElementBasePolyline item) {
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(item.Points, PathKind.Line),
				Border = InernalConverter.ConvertColor(item.BorderColor),
				Fill = System.Drawing.Color.Transparent,
				BorderMouseOver = InernalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InernalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = GetElementHint(item),
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		public static PlanElement FromTextBlocks(ElementTextBlock item) {
			var fontFamily = new System.Windows.Media.FontFamily(item.FontFamilyName);
			var fontStyle = item.FontItalic ? FontStyles.Italic : FontStyles.Normal;
			var fontWeight = item.FontBold ? FontWeights.Bold : FontWeights.Normal;
            FormattedText text = new FormattedText(item.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(fontFamily, fontStyle, fontWeight, FontStretches.Normal), item.FontSize, new SolidColorBrush(item.ForegroundColor));
			var shape = new PlanElement {
				Path = text.BuildGeometry(new Point(item.Left, item.Top)).GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture).Substring(2),
				Border = InernalConverter.ConvertColor(Colors.Transparent),
				Fill = InernalConverter.ConvertColor(item.ForegroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = GetElementHint(item),
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;

		}

		public static PlanElement FromDevice(ElementGKDevice item) {
			// Находим девайс и набор его состояний
			var device =
				GKManager.Devices.FirstOrDefault(d => d.UID == item.DeviceUID);
			if (device == null) {
				return null;
			}

			// Создаем элемент плана
			// Ширину и высоту задаем 500, т.к. знаем об этом
			var shape = new PlanElement {
				Name = item.PresentationName,
				Id = item.DeviceUID,
				Image = GetDeviceStatePic(device),
				Hint = GetElementHint(item),
				X = item.Left - 7,
				Y = item.Top - 7,
				Height = 14,
				Width = 14,
				Type = ShapeTypes.GkDevice.ToString()
			};
			return shape;
		}

		public static void UpdateDeviceState(GKState state) {
			if (state.BaseObjectType != GKBaseObjectType.Device) {
				return;
			}
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == state.UID);
			// Получаем обновленную картинку устройства
			var getPictureTask = Task.Factory.StartNewSta(() => GetDeviceStatePic(device));
			Task.WaitAll();
			var pic = getPictureTask.Result;
			if (pic == null) {
				return;
			}

            // Получаем названия для состояний
		    string[] stateClasses = new string[state.StateClasses.Count];
		    for (int index = 0; index < state.StateClasses.Count; index++) {
		        XStateClass stateClass = state.StateClasses[index];
		        stateClasses[index] = InernalConverter.GetStateClassName(stateClass);
		    }

            // Собираем обновление для передачи
		    var statusUpdate = new {
				Id = state.UID,
				Picture = pic,
				StateClass = InernalConverter.GetStateClassName(state.StateClass),
                StateClasses = stateClasses,
				state.AdditionalStates
			};
			PlansUpdater.Instance.UpdateDeviceState(statusUpdate);
		}

	    private static string GetDeviceStatePic(GKDevice device) {
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
							Canvas surface;
							var imageBytes = Encoding.Unicode.GetBytes(frame1.Image ?? "");
							using (var stream = new MemoryStream(imageBytes)) {
								surface = (Canvas)XamlServices.Load(stream);
							}
							return surface != null ? InernalConverter.XamlCanvasToPngBitmap(surface) : null;
						});
					Task.WaitAll(getBitmapTask);
					if (getBitmapTask.Result == null) {
						continue;
					}
					var pngBitmap = getBitmapTask.Result;
					var img = new MagickImage(pngBitmap) {
						AnimationDelay = frame.Duration / 10, HasAlpha = true, AlphaColor = MagickColor.Transparent, BackgroundColor = MagickColor.Transparent
					};
					collection.Add(img);
				}
				if (collection.Count == 0) {
					return string.Empty;
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
			return Convert.ToBase64String(bytes);
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

		private static ElementHint GetElementHint(ElementBase element) {
            var hint = new ElementHint();

			var asZone = element as IElementZone;
			if (asZone != null) {
				if (element is ElementRectangleGKZone
				    || element is ElementPolygonGKZone) {
					var zone = GKManager.Zones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null) {
						if (zone.PresentationName != null) {
                            hint.StateHintLines.Add(new HintLine(){Text = zone.PresentationName});
						}
					}
				}
				if (element is ElementRectangleGKGuardZone
				    || element is ElementPolygonGKGuardZone) {
					var zone = GKManager.GuardZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null) {
						if (zone.PresentationName != null) {
							hint.StateHintLines.Add(new HintLine(){Text = zone.PresentationName});
						}
					}
				}
				if (element is ElementRectangleGKSKDZone
				    || element is ElementPolygonGKSKDZone) {
					var zone = GKManager.SKDZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null) {
						if (zone.PresentationName != null) {
							hint.StateHintLines.Add(new HintLine(){Text = zone.PresentationName});
						}
					}
				}
			}
			var asMpt = element as IElementMPT;
			if (asMpt != null) {
				var mpt = GKManager.MPTs.FirstOrDefault(m => m.UID == asMpt.MPTUID);
				if (mpt != null) {
					if (mpt.PresentationName != null) {
						hint.StateHintLines.Add(new HintLine(){Text = mpt.PresentationName});
					}
				}
			}
			var asDelay = element as IElementDelay;
			if (asDelay != null) {
				var delay = GKManager.Delays.FirstOrDefault(m => m.UID == asDelay.DelayUID);
				if (delay != null) {
					if (delay.PresentationName != null) {
						hint.StateHintLines.Add(new HintLine(){Text = delay.PresentationName});
					}
				}
			}
			var asDirection = element as IElementDirection;
			if (asDirection != null) {
				var direction = GKManager.Directions.FirstOrDefault(
					d => d.UID == asDirection.DirectionUID);
				if (direction != null) {
					if (direction.PresentationName != null) {
						hint.StateHintLines.Add(new HintLine(){Text = direction.PresentationName});
					}
				}
			}
			var asDevice = element as ElementGKDevice;
			if (asDevice != null) {
				var device = GKManager.Devices.FirstOrDefault(
					d => d.UID == asDevice.DeviceUID);
				if (device != null
				    && device.PresentationName != null) {
					hint.StateHintLines.Add(new HintLine {Text = device.PresentationName, Icon = GetElementHintIcon(asDevice).Item1});
				}
			}
			return hint;
		}

		private static Tuple<string, Size> GetElementHintIcon(ElementBasePoint item) {
			var imagePath = string.Empty;

		    var gkDevice = item as ElementGKDevice;
		    if (gkDevice != null) {
				var device =
					GKManager.Devices.FirstOrDefault(d => d.UID == gkDevice.DeviceUID);
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
		    var gkDoor = item as ElementGKDoor;
		    if (gkDoor != null) {
				var door =
					GKManager.Doors.FirstOrDefault(d => d.UID == gkDoor.DoorUID);
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
		private static Tuple<string, Size> GetImageResource(string resName) {
			var assembly = Assembly.GetAssembly(typeof (Controls.AlarmButton));
			var name =
				assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(".resources", StringComparison.Ordinal));
			var resourceStream = assembly.GetManifestResourceStream(name);
			if (resourceStream == null) {
				return new Tuple<string, Size>("", new Size());
			}
			byte[] values;
			using (var reader = new ResourceReader(resourceStream)) {
				string type;
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

		
	}
}