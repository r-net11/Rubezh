#region Usings

using Common;
using Controls;
using Controls.Converters;
using GKWebService.DataProviders.Plan;
using GKWebService.DataProviders.Resources;
using GKWebService.Models.Plan.PlanElement.Hint;
using GKWebService.Utils;
using ImageMagick;
using Infrastructure.Common.Services.Content;
using Infrastructure.Plans;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xaml;
using Colors = System.Windows.Media.Colors;
using Point = System.Windows.Point;
using PointCollection = Common.PointCollection;
using Size = System.Drawing.Size;

#endregion

namespace GKWebService.Models.Plan.PlanElement
{
	public class PlanElement : Shape
	{
		#region Properties

		public string Name { get; set; }

		public ElementHint Hint { get; set; }

		public IEnumerable<Shape> ChildElements { get; set; }

		public Device Device { get; set; }

		public GKBaseModel GkObject { get; set; }
		#endregion

		#region Methods

		public static bool HasProperty(object objectToCheck, string properyName)
		{
			var type = objectToCheck.GetType();
			return type.GetProperty(properyName) != null;
		}

		public static object GetProperty(object objectToCheck, string properyName)
		{
			var type = objectToCheck.GetType();
			return type.GetProperty(properyName).GetValue(objectToCheck);
		}

		public static PlanElement FromRectangle(ElementBaseRectangle elem)
		{
			if (elem is ElementEllipse)
			{
				return null;
			}
			var showState = false;
			if (HasProperty(elem, "ShowState"))
			{
				showState = (bool)GetProperty(elem, "ShowState");
			}
			if (elem is ElementRectangleGKMPT)
			{
				showState = true;
			}
			Guid zoneUID = Guid.Empty;
			if (HasProperty(elem, "ZoneUID"))
			{
				zoneUID = (Guid)GetProperty(elem, "ZoneUID");
			}
			if (!showState)
			{
				if (elem is ElementRectangle)
				{
					return FromRectangleSimple(elem, false);
				}
				return FromRectangleSimple(elem, true);
			}

			// Получаем прямоугольник, в который вписан текст
			// Получаем элемент текста
			var textElement = new ElementTextBlock
			{
				FontBold = true,
				FontFamilyName = "Arial",
				FontItalic = true,
				Text = "Неизвестно",
				FontSize = 18,
				ForegroundColor = Common.Colors.Black,
				WordWrap = false,
				BorderThickness = 1,
				Stretch = true,
				TextAlignment = 1,
				VerticalAlignment = 1,
				PresentationName = elem.PresentationName,
				UID = zoneUID == Guid.Empty ? elem.UID : zoneUID,
				Height = elem.Height,
				Width = elem.Width
			};
			var planElementText = FromTextElement(
				textElement, new System.Windows.Size(elem.Width, elem.Height), elem.Left, elem.Top, false);
			// Получаем элемент прямоугольника, в который вписан текст
			var planElementRect = FromRectangleSimple(elem, false);
			// Очищаем элементы от групповой информации
			planElementText.Hint = null;
			planElementText.HasOverlay = false;
			planElementText.Id = Guid.Empty.ToString();
			planElementRect.Hint = null;
			planElementRect.HasOverlay = false;
			planElementRect.Id = Guid.Empty.ToString();
			// Задаем групповой элемент
			var planElement = new PlanElement
			{
				ChildElements = new[] { planElementRect, planElementText },
				Id = zoneUID == Guid.Empty ? "pe" + elem.UID : "pe" + zoneUID,
				Hint = GetElementHint(elem),
				GkObject = GetGkObject(elem),
				Type = ShapeTypes.Group.ToString(),
				Name = elem.PresentationName,
				Width = elem.Width,
				Height = elem.Height,
				HasOverlay = true,
				BorderMouseOver = InternalConverter.ConvertColor(Colors.Orange),
				X = elem.Left,
				Y = elem.Top
			};


			return planElement;
		}

		public static Rect GetRectangle(PointCollection points)
		{
			double minLeft = double.MaxValue;
			double minTop = double.MaxValue;
			double maxLeft = 0;
			double maxTop = 0;
			if (points == null)
				points = new PointCollection();

			foreach (var point in points)
			{
				if (point.X < minLeft)
					minLeft = point.X;
				if (point.Y < minTop)
					minTop = point.Y;
				if (point.X > maxLeft)
					maxLeft = point.X;
				if (point.Y > maxTop)
					maxTop = point.Y;
			}
			if (maxTop < minTop)
				minTop = maxTop;
			if (maxLeft < minLeft)
				minLeft = maxLeft;
			return new Rect(minLeft, minTop, maxLeft - minLeft, maxTop - minTop);
		}

		public static PlanElement FromPolygon(ElementBasePolygon elem)
		{
			var rect = GetRectangle(elem.Points);
			var showState = false;
			if (HasProperty(elem, "ShowState"))
			{
				showState = (bool)GetProperty(elem, "ShowState");
			}
			if (elem is ElementPolygonGKMPT)
			{
				showState = true;
			}
			if (!showState)
			{
				return FromPolygonSimple(elem, !(elem is ElementPolygon));
			}
			Guid zoneUID = Guid.Empty;
			if (HasProperty(elem, "ZoneUID"))
			{
				zoneUID = (Guid)GetProperty(elem, "ZoneUID");
			}
			// Получаем прямоугольник, в который вписан текст
			// Получаем элемент текста
			var textElement = new ElementTextBlock
			{
				FontBold = true,
				FontFamilyName = "Arial",
				FontItalic = true,
				Text = "Неизвестно",
				FontSize = 18,
				ForegroundColor = Common.Colors.Black,
				WordWrap = false,
				BorderThickness = 1,
				Stretch = true,
				TextAlignment = 1,
				VerticalAlignment = 1,
				PresentationName = elem.PresentationName,
				UID = zoneUID == Guid.Empty ? elem.UID : zoneUID,
				Height = rect.Height,
				Width = rect.Width
			};
			var planElementText = FromTextElement(
				textElement, new System.Windows.Size(rect.Width, rect.Height), rect.Left, rect.Top, false);
			// Получаем элемент прямоугольника, в который вписан текст
			var planElementRect = FromPolygonSimple(elem, false);
			// Очищаем элементы от групповой информации
			planElementText.Hint = null;
			planElementText.HasOverlay = false;
			planElementText.Id = Guid.Empty.ToString();
			planElementRect.Hint = null;
			planElementRect.HasOverlay = false;
			planElementRect.Id = Guid.Empty.ToString();
			// Задаем групповой элемент
			var planElement = new PlanElement
			{
				ChildElements = new[] { planElementRect, planElementText },
				Id = zoneUID == Guid.Empty ? "pe" + elem.UID : "pe" + zoneUID,
				Hint = GetElementHint(elem),
				GkObject = GetGkObject(elem),
				Type = ShapeTypes.Group.ToString(),
				Width = rect.Width,
				Height = rect.Height,
				HasOverlay = true,
				BorderMouseOver = InternalConverter.ConvertColor(Colors.Orange),
				X = rect.Left,
				Y = rect.Top,
				Name = elem.PresentationName
			};

			return planElement;
		}

		static System.Windows.Media.Color GetGKZoneStateColor(XStateClass stateClass)
		{
			switch (stateClass)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.TechnologicalRegime:
				case XStateClass.ConnectionLost:
				case XStateClass.HasNoLicense:
					return Colors.DarkGray;

				case XStateClass.Fire1:
				case XStateClass.Fire2:
					return Colors.Red;

				case XStateClass.Attention:
					return Colors.Yellow;

				case XStateClass.Ignore:
					return Colors.Yellow;

				case XStateClass.Norm:
					return Colors.Green;

				default:
					return Colors.White;
			}
		}

		static System.Windows.Media.Color GetGKGuardZoneStateColor(XStateClass stateClass)
		{
			switch (stateClass)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.TechnologicalRegime:
				case XStateClass.ConnectionLost:
				case XStateClass.HasNoLicense:
					return Colors.DarkGray;
				case XStateClass.On:
					return Colors.Green;
				case XStateClass.TurningOn:
					return Colors.LightGreen;
				case XStateClass.AutoOff:
					return Colors.Gray;
				case XStateClass.Ignore:
					return Colors.Yellow;
				case XStateClass.Norm:
				case XStateClass.Off:
					return Colors.Blue;
				case XStateClass.Fire1:
				case XStateClass.Fire2:
				case XStateClass.Attention:
					return Colors.Red;
				default:
					return Colors.White;
			}
		}

		static System.Windows.Media.Color GetGKSKDZoneStateColor(XStateClass stateClass)
		{
			switch (stateClass)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.TechnologicalRegime:
				case XStateClass.ConnectionLost:
				case XStateClass.HasNoLicense:
					return Colors.DarkGray;

				case XStateClass.Off:
					return Colors.Green;
				case XStateClass.TurningOff:
					return Colors.LightGreen;
				case XStateClass.Norm:
				case XStateClass.On:
					return Colors.Blue;

				case XStateClass.AutoOff:
					return Colors.Gray;
				case XStateClass.Ignore:
					return Colors.Yellow;
				case XStateClass.Fire1:
				case XStateClass.Fire2:
				case XStateClass.Attention:
					return Colors.Red;
				default:
					return Colors.White;
			}
		}

		private static PlanElement FromRectangleSimple(ElementBaseRectangle elem, bool mouseOver)
		{
			var rect = elem.GetRectangle();
			var pt = new PointCollection {
				rect.TopLeft,
				rect.TopRight,
				rect.BottomRight,
				rect.BottomLeft
			};

			var showHint = true;

			if (HasProperty(elem, "ShowTooltip"))
			{
				showHint = (bool)GetProperty(elem, "ShowTooltip");
			}

			Guid zoneUID = Guid.Empty;
			if (HasProperty(elem, "ZoneUID"))
			{
				zoneUID = (Guid)GetProperty(elem, "ZoneUID");
			}

			var backgroundImage = GetBackgroundContent(elem.BackgroundImageSource, elem.ImageType, elem.Width, elem.Height);
			var shape = new PlanElement
			{
				Path = InternalConverter.PointsToPath(pt.ToWindowsPointCollection(), PathKind.ClosedLine),
				Border = InternalConverter.ConvertColor(elem.BorderColor.ToWindowsColor()),
				BorderMouseOver = InternalConverter.ConvertColor(Colors.Orange),
				Name = elem.PresentationName,
				Id = zoneUID == Guid.Empty ? "pe" + elem.UID : "pe" + zoneUID,
				Image = backgroundImage,
				X = elem.Left,
				Y = elem.Top,
				Hint = showHint ? GetElementHint(elem) : null,
				GkObject = GetGkObject(elem),
				BorderThickness = elem.BorderThickness,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = mouseOver,
				Width = elem.Width,
				Height = elem.Height
			};
			var asZone = elem as IElementZone;
			if (asZone == null)
				return shape;
			var zone = GKManager.Zones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
			if (zone != null)
			{
				var background = GetGKZoneStateColor(zone.State.StateClass);
				shape.Fill = InternalConverter.ConvertColor(background);
				return shape;
			}
			var zoneSkd = GKManager.SKDZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
			if (zoneSkd != null)
			{
				var background = GetGKSKDZoneStateColor(zoneSkd.State.StateClass);
				shape.Fill = InternalConverter.ConvertColor(background);
				return shape;
			}
			var zoneSec = GKManager.GuardZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
			if (zoneSec != null)
			{
				var background = GetGKGuardZoneStateColor(zoneSec.State.StateClass);
				shape.Fill = InternalConverter.ConvertColor(background);
			}
			return shape;
		}

		public static PlanElement FromEllipse(ElementEllipse item)
		{
			var result = Dispatcher.CurrentDispatcher.Invoke(
				() =>
				{
					Debug.WriteLine(
						"App thread is {0}, with appartment = {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.GetApartmentState());
					return item.GetRectangle();
				});
			var rect = result;
			var pt = new PointCollection {
				rect.TopLeft,
				rect.TopRight,
				rect.BottomRight,
				rect.BottomLeft
			};
			var shape = new PlanElement
			{
				Path = InternalConverter.PointsToPath(pt.ToWindowsPointCollection(), PathKind.Ellipse),
				Border = InternalConverter.ConvertColor(item.BorderColor.ToWindowsColor()),
				Fill = InternalConverter.ConvertColor(item.BackgroundColor.ToWindowsColor()),
				BorderMouseOver = InternalConverter.ConvertColor(item.BorderColor.ToWindowsColor()),
				FillMouseOver = InternalConverter.ConvertColor(item.BackgroundColor.ToWindowsColor()),
				Name = item.PresentationName,
				Id = "pe" + item.UID,
				Hint = item.ShowTooltip ? GetElementHint(item) : null,
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = false
			};
			return shape;
		}

		public static PlanElement FromPolygonSimple(ElementBasePolygon item, bool mouseOver)
		{

			var showHint = true;

			if (HasProperty(item, "ShowTooltip"))
			{
				showHint = (bool)GetProperty(item, "ShowTooltip");
			}
			Guid zoneUID = Guid.Empty;
			if (HasProperty(item, "ZoneUID"))
			{
				zoneUID = (Guid)GetProperty(item, "ZoneUID");
			}
			var shape = new PlanElement
			{
				Path = InternalConverter.PointsToPath(item.Points.ToWindowsPointCollection(), PathKind.ClosedLine),
				Border = InternalConverter.ConvertColor(item.BorderColor.ToWindowsColor()),
				BorderMouseOver = InternalConverter.ConvertColor(Colors.Orange),
				Name = item.PresentationName,
				Id = zoneUID == Guid.Empty ? "pe" + item.UID : "pe" + zoneUID,
				GkObject = GetGkObject(item),
				BorderThickness = item.BorderThickness,
				Hint = showHint ? GetElementHint(item) : null,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = mouseOver
			};
			var asZone = item as IElementZone;
			if (asZone == null)
				return shape;
			var zone = GKManager.Zones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
			if (zone != null)
			{
				var converter = new XStateClassToColorConverter2();
				var background = ((SolidColorBrush)converter.Convert(zone.State.StateClass, typeof(SolidColorBrush), null, CultureInfo.InvariantCulture)).Color;
				shape.Fill = InternalConverter.ConvertColor(background);
				return shape;
			}
			var zoneSkd = GKManager.SKDZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
			if (zoneSkd != null)
			{
				var converter = new XStateClassToColorConverter2();
				var background = ((SolidColorBrush)converter.Convert(zoneSkd.State.StateClass, typeof(SolidColorBrush), null, CultureInfo.InvariantCulture)).Color;
				shape.Fill = InternalConverter.ConvertColor(background);
				return shape;
			}
			var zoneSec = GKManager.GuardZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
			if (zoneSec != null)
			{
				var converter = new XStateClassToColorConverter2();
				var background = ((SolidColorBrush)converter.Convert(zoneSec.State.StateClass, typeof(SolidColorBrush), null, CultureInfo.InvariantCulture)).Color;
				shape.Fill = InternalConverter.ConvertColor(background);
			}
			return shape;
		}

		public static PlanElement FromPolyline(ElementPolyline elem)
		{
			var shape = new PlanElement
			{
				Path = InternalConverter.PointsToPath(elem.Points.ToWindowsPointCollection(), PathKind.Line),
				Border = InternalConverter.ConvertColor(elem.BorderColor.ToWindowsColor()),
				Fill = System.Drawing.Color.Transparent,
				BorderMouseOver = InternalConverter.ConvertColor(elem.BorderColor.ToWindowsColor()),
				FillMouseOver = InternalConverter.ConvertColor(elem.BackgroundColor.ToWindowsColor()),
				Name = elem.PresentationName,
				Id = "pe" + elem.UID,
				Hint = elem.ShowTooltip ? GetElementHint(elem) : null,
				BorderThickness = elem.BorderThickness,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = false
			};
			return shape;
		}

		/// <summary>
		///     Создает SVG-группу из ElementTextBlock
		/// </summary>
		/// <param name="elem">ElementTextBlock</param>
		/// <returns>групповой PlanElement</returns>
		public static PlanElement FromTextBlock(ElementTextBlock elem)
		{
			// Получаем прямоугольник, в который вписан текст
			// Получаем элемент текста
			var planElementText = FromTextElement(
				elem, new System.Windows.Size(elem.Width, elem.Height), elem.Left, elem.Top, elem.ShowTooltip);
			// Получаем элемент прямоугольника, в который вписан текст
			var planElementRect = FromRectangleSimple(elem, false);
			// Очищаем элементы от групповой информации
			planElementText.Hint = null;
			planElementText.HasOverlay = false;
			planElementText.Id = Guid.Empty.ToString();
			planElementRect.Hint = null;
			planElementRect.HasOverlay = false;
			planElementRect.Id = Guid.Empty.ToString();
			// Задаем групповой элемент
			var planElement = new PlanElement
			{
				ChildElements = new[] { planElementRect, planElementText },
				Id = planElementText.Id,
				Hint = GetElementHint(elem),
				Type = ShapeTypes.Group.ToString(),
				Width = elem.Width,
				Height = elem.Height,
				HasOverlay = false,
				X = elem.Left,
				Y = elem.Top
			};

			return planElement;
		}

		/// <summary>
		///     Создает SVG-группу из ElementProcedure
		/// </summary>
		/// <param name="elem">ElementProcedure</param>
		/// <returns></returns>
		public static PlanElement FromProcedure(ElementProcedure elem)
		{
			// Получаем элемент текста
			var planElementText = FromTextElement(
				elem, new System.Windows.Size(elem.Width, elem.Height), elem.Left, elem.Top, true);
			// Получаем элемент прямоугольника, в который вписан текст

			var planElementRect = FromRectangleSimple(elem, false);
			// Очищаем элементы от групповой информации
			planElementText.Hint = null;
			planElementText.HasOverlay = false;
			planElementRect.Hint = null;
			planElementRect.HasOverlay = false;
			// Задаем групповой элемент
			var planElement = new PlanElement
			{
				ChildElements = new[] { planElementRect, planElementText },
				Id = planElementText.Id,
				Hint = GetElementHint(elem),
				Type = ShapeTypes.Group.ToString(),
				Width = elem.Width,
				Height = elem.Height,
				Name = elem.PresentationName,
				HasOverlay = true,
				BorderMouseOver = InternalConverter.ConvertColor(Colors.Orange),
				X = elem.Left,
				Y = elem.Top
			};

			return planElement;
		}

		/// <summary>
		///     Рендерит IElementTextBlock
		/// </summary>
		/// <param name="item">Элемент</param>
		/// <param name="size">Размеры контейнера</param>
		/// <param name="left">Координата X</param>
		/// <param name="top">Координата Y</param>
		/// <param name="showHint">Показывать всплывающую подсказку.</param>
		/// <returns></returns>
		private static PlanElement FromTextElement(IElementTextBlock item, System.Windows.Size size, double left, double top, bool showHint)
		{
			if (string.IsNullOrWhiteSpace(item.Text))
			{
				return null;
			}
			var fontFamily = new System.Windows.Media.FontFamily(item.FontFamilyName);
			var fontStyle = item.FontItalic ? FontStyles.Italic : FontStyles.Normal;
			var fontWeight = item.FontBold ? FontWeights.Bold : FontWeights.Normal;
			var text = new FormattedText(
				item.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
				new Typeface(fontFamily, fontStyle, fontWeight, FontStretches.Normal), item.FontSize, new SolidColorBrush(item.ForegroundColor.ToWindowsColor()))
				{
					Trimming = TextTrimming.WordEllipsis,
					MaxLineCount = item.WordWrap ? int.MaxValue : 1
				};
			// Делаем первый рендер без трансформаций
			var pathGeometry = text.BuildGeometry(new Point(left + item.BorderThickness, top + item.BorderThickness));
			// Добавляем Scale-трансформацию, если включен Stretch, либо Translate-трансформацию
			if (item.Stretch)
			{
				var scaleFactorX = (size.Width - item.BorderThickness * 2) / text.Width;
				var scaleFactorY = (size.Height - item.BorderThickness * 2) / text.Height;
				pathGeometry = Geometry.Combine(Geometry.Empty, pathGeometry, GeometryCombineMode.Union, new ScaleTransform(
					scaleFactorX, scaleFactorY, left + item.BorderThickness, top + item.BorderThickness));
			}
			else
			{
				double offsetX = 0;
				double offsetY = 0;
				switch (item.TextAlignment)
				{
					case 1:
						{
							offsetX = size.Width - item.BorderThickness * 2 - text.Width;
							break;
						}
					case 2:
						{
							offsetX = (size.Width - item.BorderThickness * 2 - text.Width) / 2;
							break;
						}
				}
				switch (item.VerticalAlignment)
				{
					case 1:
						{
							offsetY = (size.Height - item.BorderThickness * 2 - text.Height) / 2;
							break;
						}
					case 2:
						{
							offsetY = size.Height - item.BorderThickness * 2 - text.Height;
							break;
						}
				}
				pathGeometry = Geometry.Combine(Geometry.Empty, pathGeometry, GeometryCombineMode.Union, new TranslateTransform(offsetX, offsetY));
			}
			// Делаем финальный рендер
			var path = pathGeometry.GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture).Substring(2);

			var shape = new PlanElement
			{
				Path = path,
				Border = InternalConverter.ConvertColor(Colors.Transparent),
				Fill = InternalConverter.ConvertColor(item.ForegroundColor.ToWindowsColor()),
				Name = item.PresentationName,
				Id = "pe" + item.UID,
				Hint = (item as ElementBase) != null && showHint ? GetElementHint((ElementBase)item) : null,
				BorderThickness = 0,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = false
			};
			return shape;
		}

		public static PlanElement FromGkDoor(ElementGKDoor item)
		{
			var strings = EmbeddedResourceLoader.LoadResource("GKWebService.Content.SvgIcons.GKDoor.txt")
												.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
			var result = Dispatcher.CurrentDispatcher.Invoke(
				() =>
				{
					Debug.WriteLine(
						"App thread is {0}, with appartment = {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.GetApartmentState());
					return item.GetRectangle();
				});
			var rect = result;
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
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = false
			};
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
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = false
			};

			var planElement = new PlanElement
			{
				ChildElements = new[] { shape1, shape2 },
				Id = "pe" + item.UID,
				Hint = GetElementHint(item),
				Type = ShapeTypes.Group.ToString(),
				Width = 30,
				Height = 30,
				Name = item.PresentationName,
				HasOverlay = true,
				BorderMouseOver = InternalConverter.ConvertColor(Colors.Orange),
				X = rect.Left,
				Y = rect.Top
			};
			return planElement;
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
			var shape = new PlanElement
			{
				Id = "pe" + device.UID,
				SubElementId = device.UID.ToString(),
				Name = device.PresentationName,
				Image = GetDeviceStatePic(device, device.State),
				X = item.Left - 7,
				Y = item.Top - 7,
				Height = 30,
				Width = 30,
				Type = ShapeTypes.GkDevice.ToString(),
				HasOverlay = false
			};
			// Добавляем рамку хинта
			var planElement = new PlanElement
			{
				ChildElements = new[] { shape },
				Id = "pe" + device.UID,
				SubElementId = device.UID + "GroupElement",
				Hint = GetElementHint(item),
				Type = ShapeTypes.Group.ToString(),
				Width = 30,
				Height = 30,
				HasOverlay = true,
				Name = device.PresentationName,
				Device = new Device(device),
				GkObject = new Device(device),
				BorderMouseOver = InternalConverter.ConvertColor(Colors.Orange),
				X = item.Left - 7,
				Y = item.Top - 7
			};
			return planElement;
		}

		public static void UpdateZoneState(GKState state)
		{
			var zone = GKManager.Zones.Union<GKBase>(GKManager.GuardZones).Union(GKManager.SKDZones).FirstOrDefault(z => state.UID == z.UID);
			if (zone == null)
			{
				throw new Exception(string.Format("Зона {0} не найдена.", state.UID));
			}
			var hint = new ElementHint();

			var hintImageSource = zone.ImageSource.Replace("/Controls;component/", "");
			hint.StateHintLines.Add(new HintLine { Text = zone.PresentationName, Icon = (hintImageSource.Trim() != string.Empty) ? GetImageResource(hintImageSource).Item1 : null });

			// Добавляем состояния
			foreach (var stateClass in zone.State.StateClasses)
			{
				//Получаем источник иконки для основного класса
				var iconSourceForStateClasses = stateClass.ToIconSource();
				hint.StateHintLines.Add(
					new HintLine
					{
						Text = stateClass.ToDescription(),
						Icon = iconSourceForStateClasses != null ? GetImageResource(iconSourceForStateClasses.Replace("/Controls;component/", "")).Item1 : null
					});
			}
			// Добавляем доп. состояния
			foreach (var stateClass in zone.State.AdditionalStates)
			{
				//Получаем источник иконки для основного класса
				var iconSourceForAdditionalStateClasses = stateClass.StateClass.ToIconSource();
				hint.StateHintLines.Add(
					new HintLine
					{
						Text = stateClass.Name,
						Icon = iconSourceForAdditionalStateClasses != null ? GetImageResource(iconSourceForAdditionalStateClasses.Replace("/Controls;component/", "")).Item1 : null
					});
			}

			System.Windows.Media.Color background;
			switch (zone.GetType().ToString())
			{
				case "RubezhAPI.GK.GKZone":
					{
						background = GetGKZoneStateColor(state.StateClass);
						break;
					}
				case "RubezhAPI.GK.GKSKDZone":
					{
						background = GetGKSKDZoneStateColor(state.StateClass);
						break;
					}
				case "RubezhAPI.GK.GKGuardZone":
					{
						background = GetGKGuardZoneStateColor(state.StateClass);
						break;
					}
				default:
					{
						background = System.Windows.Media.Colors.Transparent;
						break;
					}
			}

			// Собираем обновление для передачи
			var statusUpdate = new
			{
				Id = "pe" + state.UID,
				Background = new { R = background.R, G = background.G, B = background.B, A = background.A },
				Hint = hint
			};
			PlansUpdater.Instance.UpdateZoneState(statusUpdate);
		}

		public static void UpdateDeviceState(GKState state)
		{
			if (state.BaseObjectType != GKBaseObjectType.Device)
			{
				throw new ArgumentException(@"BaseObjectType должен быть GKBaseObjectType.Device", "state");
			}
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == state.UID);
			if (device == null)
			{
				throw new Exception(string.Format("Устройство {0} не найдено.", state.UID));
			}

			// Получаем обновленную картинку устройства
			var getPictureTask = Task.Factory.StartNewSta(() => GetDeviceStatePic(device, state));
			Task.WaitAll(getPictureTask);
			var pic = getPictureTask.Result;

			var hint = new ElementHint();

			var hintImageSource = device.ImageSource.Replace("/Controls;component/", "");
			hint.StateHintLines.Add(new HintLine { Text = device.PresentationName, Icon = (hintImageSource.Trim() != string.Empty) ? GetImageResource(hintImageSource).Item1 : null });

			// Добавляем состояния
			foreach (var stateClass in device.State.StateClasses)
			{
				//Получаем источник иконки для основного класса
				var iconSourceForStateClasses = stateClass.ToIconSource();
				hint.StateHintLines.Add(
					new HintLine
					{
						Text = stateClass.ToDescription(),
						Icon = iconSourceForStateClasses != null ? GetImageResource(iconSourceForStateClasses.Replace("/Controls;component/", "")).Item1 : null
					});
			}
			// Добавляем доп. состояния
			foreach (var stateClass in device.State.AdditionalStates)
			{
				//Получаем источник иконки для основного класса
				var iconSourceForAdditionalStateClasses = stateClass.StateClass.ToIconSource();
				hint.StateHintLines.Add(
					new HintLine
					{
						Text = stateClass.Name,
						Icon = iconSourceForAdditionalStateClasses != null ? GetImageResource(iconSourceForAdditionalStateClasses.Replace("/Controls;component/", "")).Item1 : null
					});
			}
			// Собираем обновление для передачи
			var statusUpdate = new
			{
				Id = "pe" + state.UID,
				Picture = pic,
				Hint = hint
			};
			PlansUpdater.Instance.UpdateDeviceState(statusUpdate);
		}

		private static string GetDeviceStatePic(GKDevice device, GKState state)
		{
			var deviceConfig =
				GKManager.DeviceLibraryConfiguration.GKDevices.FirstOrDefault(d => d.DriverUID == device.DriverUID);
			if (deviceConfig == null)
			{
				return null;
			}
			var stateWithPic =
				deviceConfig.States.FirstOrDefault(s => s.StateClass == state.StateClass) ??
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
					frame1.Image = frame1.Image.Replace("#000000", "#FF0F0F0F");
					Canvas surface;
					var imageBytes = Encoding.Unicode.GetBytes(frame1.Image ?? "");
					using (var stream = new MemoryStream(imageBytes))
					{
						surface = (Canvas)XamlServices.Load(stream);
					}
					var pngBitmap = surface != null ? InternalConverter.XamlCanvasToPngBitmap(surface) : null;
					if (pngBitmap == null)
					{
						continue;
					}
					var img = new MagickImage(pngBitmap)
					{
						AnimationDelay = frame.Duration / 10,
						HasAlpha = true
					};
					collection.Add(img);
				}
				if (collection.Count == 0)
				{
					return string.Empty;
				}
				//Optionally reduce colors
				QuantizeSettings settings = new QuantizeSettings { Colors = 256 };
				collection.Quantize(settings);

				// Optionally optimize the images (images should have the same size).
				collection.Optimize();

				using (var str = new MemoryStream())
				{
					collection.Write(str, MagickFormat.Gif);
					bytes = str.ToArray();
				}


			}
			return Convert.ToBase64String(bytes);
		}

		private static GKBaseModel GetGkObject(ElementBase elem)
		{
			var asZone = elem as IElementZone;
			if (asZone != null)
			{
				if (elem is ElementRectangleGKZone || elem is ElementPolygonGKZone)
				{
					var zone = GKManager.Zones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					return new FireZone.FireZone(zone);
				}
				if (elem is ElementRectangleGKGuardZone || elem is ElementPolygonGKGuardZone)
				{
					var zone = GKManager.GuardZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					return new GuardZones.GuardZone(zone);
				}
			}

			return null;
		}

		private static ElementHint GetElementHint(ElementBase element)
		{
			var hint = new ElementHint();

			//var asDoor = element as ElementGKDoor;
			//if (asDoor != null) {
			//	var door = GKManager.Doors.FirstOrDefault(d => d.UID == asDoor.DoorUID);
			//	if (door != null) {
			//		if (door.PresentationName != null) {
			//			hint.StateHintLines.Add(new HintLine { Text = door.PresentationName, Icon = GetElementHintIcon(asDoor).Item1 });
			//		}
			//	}
			//}

			var asZone = element as IElementZone;
			if (asZone != null)
			{
				if (element is ElementRectangleGKZone || element is ElementPolygonGKZone)
				{
					var zone = GKManager.Zones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null && zone.PresentationName != null)
					{
						var imagePath = "Images/Zone.png";
						var imageData = GetImageResource(imagePath);
						hint.StateHintLines.Add(new HintLine { Text = zone.PresentationName, Icon = imageData.Item1 });

						// Добавляем состояния
						foreach (var stateClass in zone.State.StateClasses)
						{
							//Получаем источник иконки для основного класса
							var iconSourceForStateClasses = stateClass.ToIconSource();
							hint.StateHintLines.Add(
								new HintLine
								{
									Text = stateClass.ToDescription(),
									Icon = iconSourceForStateClasses != null ? GetImageResource(iconSourceForStateClasses.Replace("/Controls;component/", "")).Item1 : null
								});
						}
						// Добавляем доп. состояния
						foreach (var stateClass in zone.State.AdditionalStates)
						{
							//Получаем источник иконки для основного класса
							var iconSourceForAdditionalStateClassses = stateClass.StateClass.ToIconSource();
							hint.StateHintLines.Add(
								new HintLine
								{
									Text = stateClass.Name,
									Icon = iconSourceForAdditionalStateClassses != null ? GetImageResource(iconSourceForAdditionalStateClassses.Replace("/Controls;component/", "")).Item1 : null
								});
						}

					}
				}
				if (element is ElementRectangleGKGuardZone
					|| element is ElementPolygonGKGuardZone)
				{
					var zone = GKManager.GuardZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null && zone.PresentationName != null)
					{
						var imagePath = "Images/GuardZone.png";
						var imageData = GetImageResource(imagePath);
						hint.StateHintLines.Add(new HintLine { Text = zone.PresentationName, Icon = imageData.Item1 });

						// Добавляем состояния
						foreach (var stateClass in zone.State.StateClasses)
						{
							//Получаем источник иконки для основного класса
							var iconSourceForStateClasses = stateClass.ToIconSource();
							hint.StateHintLines.Add(
								new HintLine
								{
									Text = stateClass.ToDescription(),
									Icon = iconSourceForStateClasses != null ? GetImageResource(iconSourceForStateClasses.Replace("/Controls;component/", "")).Item1 : null
								});
						}
						// Добавляем доп. состояния
						foreach (var stateClass in zone.State.AdditionalStates)
						{
							//Получаем источник иконки для основного класса
							var iconSourceForAdditionalStateClassses = stateClass.StateClass.ToIconSource();
							hint.StateHintLines.Add(
								new HintLine
								{
									Text = stateClass.Name,
									Icon = iconSourceForAdditionalStateClassses != null ? GetImageResource(iconSourceForAdditionalStateClassses.Replace("/Controls;component/", "")).Item1 : null
								});
						}

					}

				}
				if (element is ElementRectangleGKSKDZone
					|| element is ElementPolygonGKSKDZone)
				{
					var zone = GKManager.SKDZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null && zone.PresentationName != null)
					{
						var imagePath = "Images/Zone.png";
						var imageData = GetImageResource(imagePath);
						hint.StateHintLines.Add(new HintLine { Text = zone.PresentationName, Icon = imageData.Item1 });

						// Добавляем состояния
						foreach (var stateClass in zone.State.StateClasses)
						{
							//Получаем источник иконки для основного класса
							var iconSourceForStateClasses = stateClass.ToIconSource();
							hint.StateHintLines.Add(
								new HintLine
								{
									Text = stateClass.ToDescription(),
									Icon = iconSourceForStateClasses != null ? GetImageResource(iconSourceForStateClasses.Replace("/Controls;component/", "")).Item1 : null
								});
						}
						// Добавляем доп. состояния
						foreach (var stateClass in zone.State.AdditionalStates)
						{
							//Получаем источник иконки для основного класса
							var iconSourceForAdditionalStateClassses = stateClass.StateClass.ToIconSource();
							hint.StateHintLines.Add(
								new HintLine
								{
									Text = stateClass.Name,
									Icon = iconSourceForAdditionalStateClassses != null ? GetImageResource(iconSourceForAdditionalStateClassses.Replace("/Controls;component/", "")).Item1 : null
								});
						}

					}

				}
			}
			var asMpt = element as IElementMPT;
			if (asMpt != null)
			{
				var mpt = GKManager.MPTs.FirstOrDefault(m => m.UID == asMpt.MPTUID);
				if (mpt != null && mpt.PresentationName != null)
				{
					var imagePath = "Images/BMPT.png";
					var imageData = GetImageResource(imagePath);
					hint.StateHintLines.Add(new HintLine { Text = mpt.PresentationName, Icon = imageData.Item1 });

					// Добавляем состояния
					foreach (var stateClass in mpt.State.StateClasses)
					{
						//Получаем источник иконки для основного класса
						var iconSourceForStateClasses = stateClass.ToIconSource();
						hint.StateHintLines.Add(
							new HintLine
							{
								Text = stateClass.ToDescription(),
								Icon = iconSourceForStateClasses != null ? GetImageResource(iconSourceForStateClasses.Replace("/Controls;component/", "")).Item1 : null
							});
					}
					// Добавляем доп. состояния
					foreach (var stateClass in mpt.State.AdditionalStates)
					{
						//Получаем источник иконки для основного класса
						var iconSourceForAdditionalStateClassses = stateClass.StateClass.ToIconSource();
						hint.StateHintLines.Add(
							new HintLine
							{
								Text = stateClass.Name,
								Icon = iconSourceForAdditionalStateClassses != null ? GetImageResource(iconSourceForAdditionalStateClassses.Replace("/Controls;component/", "")).Item1 : null
							});
					}

				}
			}
			//var asDelay = element as IElementDelay;
			//if (asDelay != null) {
			//	var delay = GKManager.Delays.FirstOrDefault(m => m.UID == asDelay.DelayUID);
			//	if (delay != null) {
			//		if (delay.PresentationName != null) {
			//			hint.StateHintLines.Add(new HintLine {
			//				Text = delay.PresentationName,
			//				Icon = new Func<string>(
			//						() => {
			//							var imagePath = delay.ImageSource.Replace("/Controls;component/", "");
			//							var imageData = GetImageResource(imagePath);
			//							return imageData.Item1;
			//						}).Invoke()
			//			});
			//		}
			//	}
			//}
			var asDirection = element as IElementDirection;
			if (asDirection != null)
			{
				var direction = GKManager.Directions.FirstOrDefault(
					d => d.UID == asDirection.DirectionUID);
				if (direction != null && direction.PresentationName != null)
				{
					var imagePath = "Images/Blue_Direction.png";
					var imageData = GetImageResource(imagePath);
					hint.StateHintLines.Add(new HintLine { Text = direction.PresentationName, Icon = imageData.Item1 });

					// Добавляем состояния
					foreach (var stateClass in direction.State.StateClasses)
					{
						//Получаем источник иконки для основного класса
						var iconSourceForStateClasses = stateClass.ToIconSource();
						hint.StateHintLines.Add(
							new HintLine
							{
								Text = stateClass.ToDescription(),
								Icon = iconSourceForStateClasses != null ? GetImageResource(iconSourceForStateClasses.Replace("/Controls;component/", "")).Item1 : null
							});
					}
					// Добавляем доп. состояния
					foreach (var stateClass in direction.State.AdditionalStates)
					{
						//Получаем источник иконки для основного класса
						var iconSourceForAdditionalStateClassses = stateClass.StateClass.ToIconSource();
						hint.StateHintLines.Add(
							new HintLine
							{
								Text = stateClass.Name,
								Icon = iconSourceForAdditionalStateClassses != null ? GetImageResource(iconSourceForAdditionalStateClassses.Replace("/Controls;component/", "")).Item1 : null
							});
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

					// Добавляем состояния
					foreach (var stateClass in device.State.StateClasses)
					{
						//Получаем источник иконки для основного класса
						var iconSourceForStateClasses = stateClass.ToIconSource();
						hint.StateHintLines.Add(
							new HintLine
							{
								Text = stateClass.ToDescription(),
								Icon = iconSourceForStateClasses != null ? GetImageResource(iconSourceForStateClasses.Replace("/Controls;component/", "")).Item1 : null
							});
					}
					// Добавляем доп. состояния
					foreach (var stateClass in device.State.AdditionalStates)
					{
						//Получаем источник иконки для основного класса
						var iconSourceForAdditionalStateClassses = stateClass.StateClass.ToIconSource();
						hint.StateHintLines.Add(
							new HintLine
							{
								Text = stateClass.Name,
								Icon = iconSourceForAdditionalStateClassses != null ? GetImageResource(iconSourceForAdditionalStateClassses.Replace("/Controls;component/", "")).Item1 : null
							});
					}
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

				imagePath = device.ImageSource.Replace("/Controls;component/", "");
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
		private static Tuple<string, Size> GetImageResource(string resName)
		{
			var assembly = Assembly.GetAssembly(typeof(AlarmButton));
			var name =
				assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(".resources", StringComparison.Ordinal));
			var resourceStream = assembly.GetManifestResourceStream(name);
			if (resourceStream == null)
			{
				return new Tuple<string, Size>("", new Size());
			}
			byte[] values;
			using (var reader = new ResourceReader(resourceStream))
			{
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
			using (var stream = new MemoryStream())
			{
				value1.Save(stream, ImageFormat.Png);
				stream.Close();

				byteArray = stream.ToArray();
			}

			return new Tuple<string, Size>(Convert.ToBase64String(byteArray), value1.Size);
		}

		private static readonly ContentService _contentService;

		static PlanElement()
		{
			_contentService = new ContentService("GKWEB");
		}

		public static string GetBackgroundContent(Guid? source, ResourceType imageType, double width, double height)
		{
			if (!source.HasValue)
			{
				return string.Empty;
			}
			switch (imageType)
			{
				case ResourceType.Image:
					{
						var bmp = _contentService.GetBitmapContent(source.Value);
						bmp.Freeze();
						var encoder = new PngBitmapEncoder();
						encoder.Frames.Add(BitmapFrame.Create(bmp));
						using (MemoryStream ms = new MemoryStream())
						{
							encoder.Save(ms);
							var data = ms.ToArray();
							return Convert.ToBase64String(data);
						}
					}
				case ResourceType.Drawing:
					{
						var drawing = _contentService.GetDrawing(source.Value);
						drawing.Freeze();
						return InternalConverterOld.XamlDrawingToPngBase64String(width, height, drawing);
					}
				case ResourceType.Visual:
					{
						var canvas = _contentService.GetObject<Canvas>(source.Value);
						return canvas == null ? string.Empty : InternalConverter.XamlCanvasToPngBase64(canvas, width, height);
					}
				default:
					{
						return string.Empty;
					}
			}
		}


	}

		#endregion
}
