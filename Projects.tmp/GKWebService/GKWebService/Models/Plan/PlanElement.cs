#region Usings

using GKWebService.DataProviders.Plan;
using GKWebService.DataProviders.Resources;
using GKWebService.Utils;
using ImageMagick;
//using Infrustructure.Plans.Elements;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Media;
using System.Xaml;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;

#endregion

namespace GKWebService.Models.Plan
{
	public class PlanElement : Shape
	{
		/*public static PlanElement FromRectangle(ElementBaseRectangle item)
		{
			var rect = item.GetRectangle();
			var pt = new PointCollection {
				rect.TopLeft,
				rect.TopRight,
				rect.BottomRight,
				rect.BottomLeft
			};
			var shape = new PlanElement
			{
				Path = InternalConverter.PointsToPath(pt, PathKind.ClosedLine),
				Border = InternalConverter.ConvertColor(item.BorderColor),
				Fill = InternalConverter.ConvertColor(item.BackgroundColor),
				BorderMouseOver = InternalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InternalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = GetElementHint(item),
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		public static PlanElement FromEllipse(ElementEllipse item)
		{
			var rect = item.GetRectangle();
			var pt = new PointCollection {
				rect.TopLeft,
				rect.TopRight,
				rect.BottomRight,
				rect.BottomLeft
			};
			var shape = new PlanElement
			{
				Path = InternalConverter.PointsToPath(pt, PathKind.Ellipse),
				Border = InternalConverter.ConvertColor(item.BorderColor),
				Fill = InternalConverter.ConvertColor(item.BackgroundColor),
				BorderMouseOver = InternalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InternalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = GetElementHint(item),
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		public static PlanElement FromPolygon(ElementBasePolygon item)
		{
			var shape = new PlanElement
			{
				Path = InternalConverter.PointsToPath(item.Points, PathKind.ClosedLine),
				Border = InternalConverter.ConvertColor(item.BorderColor),
				Fill = InternalConverter.ConvertColor(item.BackgroundColor),
				BorderMouseOver = InternalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InternalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				BorderThickness = item.BorderThickness,
				Hint = GetElementHint(item),
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		public static PlanElement FromPolyline(ElementBasePolyline item)
		{
			var shape = new PlanElement
			{
				Path = InternalConverter.PointsToPath(item.Points, PathKind.Line),
				Border = InternalConverter.ConvertColor(item.BorderColor),
				Fill = System.Drawing.Color.Transparent,
				BorderMouseOver = InternalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InternalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = GetElementHint(item),
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		public static PlanElement FromTextBlocks(ElementTextBlock item)
		{
			var fontFamily = new System.Windows.Media.FontFamily(item.FontFamilyName);
			var fontStyle = item.FontItalic ? FontStyles.Italic : FontStyles.Normal;
			var fontWeight = item.FontBold ? FontWeights.Bold : FontWeights.Normal;
			var text = new FormattedText(
				item.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
				new Typeface(fontFamily, fontStyle, fontWeight, FontStretches.Normal), item.FontSize, new SolidColorBrush(item.ForegroundColor))
			{
				Trimming = TextTrimming.WordEllipsis,
				MaxLineCount = item.WordWrap ? int.MaxValue : 1
			};
			// Делаем первый рендер без трансформаций
			var pathGeometry = text.BuildGeometry(new Point(item.Left + item.BorderThickness, item.Top + item.BorderThickness));
			// Добавляем Scale-трансформацию, если включен Stretch, либо Translate-трансформацию
			if (item.Stretch)
			{
				var scaleFactorX = (item.Width - item.BorderThickness * 2) / text.Width;
				var scaleFactorY = (item.Height - item.BorderThickness * 2) / text.Height;
				pathGeometry.Transform = new ScaleTransform(
					scaleFactorX, scaleFactorY, item.Left + item.BorderThickness, item.Top + item.BorderThickness);
			}
			else
			{
				double offsetX = 0;
				double offsetY = 0;
				switch (item.TextAlignment)
				{
					case 1:
						{
							offsetX = item.Width - item.BorderThickness * 2 - text.Width;
							break;
						}
					case 2:
						{
							offsetX = (item.Width - item.BorderThickness * 2 - text.Width) / 2;
							break;
						}
				}
				switch (item.VerticalAlignment)
				{
					case 1:
						{
							offsetY = (item.Height - item.BorderThickness * 2 - text.Height) / 2;
							break;
						}
					case 2:
						{
							offsetY = item.Height - item.BorderThickness * 2 - text.Height;
							break;
						}
				}

				pathGeometry.Transform = new TranslateTransform(offsetX, offsetY);
			}
			// Делаем финальный рендер
			var path = pathGeometry.GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture).Substring(2);

			var shape = new PlanElement
			{
				Path = path,
				Border = InternalConverter.ConvertColor(Colors.Transparent),
				Fill = InternalConverter.ConvertColor(item.ForegroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = GetElementHint(item),
				BorderThickness = 0,
				Type = ShapeTypes.Path.ToString()
			};
			return shape;
		}

		public static List<PlanElement> FromGkDoor(ElementGKDoor item)
		{
			var result = new List<PlanElement>();
			var strings = EmbeddedResourceLoader.LoadResource("GKWebService.Content.SvgIcons.GKDoor.txt")
												.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
			var rect = item.GetRectangle();
			var geometry = Geometry.Parse(strings[0]);
			var flatten = geometry.GetFlattenedPathGeometry();
			flatten.Transform = new TranslateTransform(rect.TopLeft.X, rect.TopRight.Y);
			KnownColor color1;
			Enum.TryParse(strings[1], true, out color1);
			KnownColor color2;
			Enum.TryParse(strings[2], true, out color2);
			var shape1 = new PlanElement
			{
				Path = flatten.GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture),
				Border = System.Drawing.Color.FromKnownColor(color1),
				Fill = System.Drawing.Color.FromKnownColor(color2),
				BorderMouseOver = InternalConverter.ConvertColor(Colors.Transparent),
				FillMouseOver = InternalConverter.ConvertColor(Colors.Transparent),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = GetElementHint(item),
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			result.Add(shape1);
			geometry = Geometry.Parse(strings[3]);
			flatten = geometry.GetFlattenedPathGeometry();
			flatten.Transform = new TranslateTransform(rect.TopLeft.X, rect.TopRight.Y);
			Enum.TryParse(strings[4], true, out color1);
			Enum.TryParse(strings[5], true, out color2);

			var shape2 = new PlanElement
			{
				Path = flatten.GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture),
				Border = System.Drawing.Color.FromKnownColor(color1),
				Fill = System.Drawing.Color.FromKnownColor(color2),
				BorderMouseOver = InternalConverter.ConvertColor(Colors.Transparent),
				FillMouseOver = InternalConverter.ConvertColor(Colors.Transparent),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = GetElementHint(item),
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString()
			};
			result.Add(shape2);
			return result;
		}

		public static PlanElement FromDevice(ElementGKDevice item)
		{
			// Находим девайс и набор его состояний
			var device =
				GKManager.Devices.FirstOrDefault(d => d.UID == item.DeviceUID);
			if (device == null)
			{
				return null;
			}

			// Создаем элемент плана
			// Ширину и высоту задаем 500, т.к. знаем об этом
			var shape = new PlanElement
			{
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

		public static void UpdateDeviceState(GKState state)
		{
			if (state.BaseObjectType != GKBaseObjectType.Device)
			{
				return;
			}
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == state.UID);
			// Получаем обновленную картинку устройства
			var getPictureTask = Task.Factory.StartNewSta(() => GetDeviceStatePic(device));
			Task.WaitAll();
			var pic = getPictureTask.Result;
			if (pic == null)
			{
				return;
			}

			// Получаем названия для состояний
			var stateClasses = new string[state.StateClasses.Count];
			for (var index = 0; index < state.StateClasses.Count; index++)
			{
				var stateClass = state.StateClasses[index];
				stateClasses[index] = InternalConverter.GetStateClassName(stateClass);
			}

			// Собираем обновление для передачи
			var statusUpdate = new
			{
				Id = state.UID,
				Picture = pic,
				StateClass = InternalConverter.GetStateClassName(state.StateClass),
				StateClasses = stateClasses,
				state.AdditionalStates
			};
			PlansUpdater.Instance.UpdateDeviceState(statusUpdate);
		}

		private static string GetDeviceStatePic(GKDevice device)
		{
			if (device == null)
				return null;
			var deviceConfig =
				GKManager.DeviceLibraryConfiguration.GKDevices.FirstOrDefault(d => d.DriverUID == device.DriverUID);
			if (deviceConfig == null)
			{
				return null;
			}
			var stateWithPic =
				deviceConfig.States.FirstOrDefault(s => s.StateClass == device.State.StateClass) ??
				deviceConfig.States.FirstOrDefault(s => s.StateClass == XStateClass.No);
			if (stateWithPic == null)
			{
				return null;
			}

			// Перебираем кадры в состоянии и генерируем gif картинку
			byte[] bytes;
			using (var collection = new MagickImageCollection())
			{
				foreach (var frame in stateWithPic.Frames)
				{
					var frame1 = frame;
					var getBitmapTask = Task.Factory.StartNewSta(
						() =>
						{
							Canvas surface;
							var imageBytes = Encoding.Unicode.GetBytes(frame1.Image ?? "");
							using (var stream = new MemoryStream(imageBytes))
							{
								surface = (Canvas)XamlServices.Load(stream);
							}
							return surface != null ? InternalConverter.XamlCanvasToPngBitmap(surface) : null;
						});
					Task.WaitAll(getBitmapTask);
					if (getBitmapTask.Result == null)
					{
						continue;
					}
					var pngBitmap = getBitmapTask.Result;
					var img = new MagickImage(pngBitmap)
					{
						AnimationDelay = frame.Duration / 10,
						HasAlpha = true,
						AlphaColor = MagickColor.Transparent,
						BackgroundColor = MagickColor.Transparent
					};
					collection.Add(img);
				}
				if (collection.Count == 0)
				{
					return string.Empty;
				}
				var tempFilename = System.IO.Path.GetTempPath();

				var path = string.Format(@"{0}tmpImage{1}.gif", tempFilename, Guid.NewGuid());
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
			return Convert.ToBase64String(bytes);
		}

		private static ElementHint GetElementHint(ElementBase element)
		{
			var hint = new ElementHint();

			var asDoor = element as ElementGKDoor;
			if (asDoor != null)
			{
				var door = GKManager.Doors.FirstOrDefault(d => d.UID == asDoor.DoorUID);
				if (door != null)
				{
					if (door.PresentationName != null)
					{
						hint.StateHintLines.Add(new HintLine { Text = door.PresentationName, Icon = GetElementHintIcon(asDoor).Item1 });
					}
				}
			}

			var asZone = element as IElementZone;
			if (asZone != null)
			{
				if (element is ElementRectangleGKZone
					|| element is ElementPolygonGKZone)
				{
					var zone = GKManager.Zones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null)
					{
						if (zone.PresentationName != null)
						{
							hint.StateHintLines.Add(new HintLine { Text = zone.PresentationName });
						}
					}
				}
				if (element is ElementRectangleGKGuardZone
					|| element is ElementPolygonGKGuardZone)
				{
					var zone = GKManager.GuardZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null)
					{
						if (zone.PresentationName != null)
						{
							hint.StateHintLines.Add(new HintLine { Text = zone.PresentationName });
						}
					}
				}
				if (element is ElementRectangleGKSKDZone
					|| element is ElementPolygonGKSKDZone)
				{
					var zone = GKManager.SKDZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null)
					{
						if (zone.PresentationName != null)
						{
							hint.StateHintLines.Add(new HintLine { Text = zone.PresentationName });
						}
					}
				}
			}
			var asMpt = element as IElementMPT;
			if (asMpt != null)
			{
				var mpt = GKManager.MPTs.FirstOrDefault(m => m.UID == asMpt.MPTUID);
				if (mpt != null)
				{
					if (mpt.PresentationName != null)
					{
						hint.StateHintLines.Add(new HintLine { Text = mpt.PresentationName });
					}
				}
			}
			var asDelay = element as IElementDelay;
			if (asDelay != null)
			{
				var delay = GKManager.Delays.FirstOrDefault(m => m.UID == asDelay.DelayUID);
				if (delay != null)
				{
					if (delay.PresentationName != null)
					{
						hint.StateHintLines.Add(new HintLine { Text = delay.PresentationName });
					}
				}
			}
			var asDirection = element as IElementDirection;
			if (asDirection != null)
			{
				var direction = GKManager.Directions.FirstOrDefault(
					d => d.UID == asDirection.DirectionUID);
				if (direction != null)
				{
					if (direction.PresentationName != null)
					{
						hint.StateHintLines.Add(new HintLine { Text = direction.PresentationName });
					}
				}
			}
			var asDevice = element as ElementGKDevice;
			if (asDevice != null)
			{
				var device = GKManager.Devices.FirstOrDefault(
					d => d.UID == asDevice.DeviceUID);
				if (device != null
					&& device.PresentationName != null)
				{
					hint.StateHintLines.Add(new HintLine { Text = device.PresentationName, Icon = GetElementHintIcon(asDevice).Item1 });
				}
			}
			return hint;
		}

		private static Tuple<string, Size> GetElementHintIcon(ElementBasePoint item)
		{
			var imagePath = string.Empty;

			var gkDevice = item as ElementGKDevice;
			if (gkDevice != null)
			{
				var device =
					GKManager.Devices.FirstOrDefault(d => d.UID == gkDevice.DeviceUID);
				if (device == null)
				{
					return null;
				}

				var deviceConfig =
					GKManager.DeviceLibraryConfiguration.GKDevices.FirstOrDefault(d => d.DriverUID == device.DriverUID);
				if (deviceConfig == null)
				{
					return null;
				}

				var stateWithPic =
					deviceConfig.States.FirstOrDefault(s => s.StateClass == device.State.StateClass) ??
					deviceConfig.States.FirstOrDefault(s => s.StateClass == XStateClass.No);
				imagePath = device.ImageSource;//.Replace("/Controls;component/", "");
			}
			var gkDoor = item as ElementGKDoor;
			if (gkDoor != null)
			{
				var door =
					GKManager.Doors.FirstOrDefault(d => d.UID == gkDoor.DoorUID);
				if (door == null)
				{
					return null;
				}

				imagePath = door.ImageSource;//.Replace("/Controls;component/", "");
			}

			var imageData = InternalConverter.GetImageResource(imagePath);

			return imageData;
		}*/
	}
}
