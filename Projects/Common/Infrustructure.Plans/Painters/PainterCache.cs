using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Security.Cryptography;
using System;
using Common;
using Infrustructure.Plans.Elements;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace Infrustructure.Plans.Painters
{
	public static class PainterCache
	{
		private static ImageBrush _transparentBackgroundBrush;
		private static Converter<Guid, BitmapImage> _imageFactory;
		private static Dictionary<Color, Brush> _brushes = new Dictionary<Color, Brush>();
		private static Dictionary<Brush, Brush> _transparentBrushes = new Dictionary<Brush, Brush>();
		private static Dictionary<Guid, Brush> _pictureBrushes = new Dictionary<Guid, Brush>();
		private static Dictionary<Color, Dictionary<double, Pen>> _pens = new Dictionary<Color, Dictionary<double, Pen>>();

		public static Pen ZonePen { get; private set; }
		public static Brush BlackBrush { get; private set; }
		public static RectangleGeometry PointGeometry { get; private set; }
		public static double Zoom { get; private set; }
		public static double PointZoom { get; private set; }

		static PainterCache()
		{
			BlackBrush = new SolidColorBrush(Colors.Black);
			BlackBrush.Freeze();
			ZonePen = new Pen(BlackBrush, 1);
			PointGeometry = new RectangleGeometry(new Rect(-15, -15, 30, 30));
			_transparentBackgroundBrush = CreateTransparentBackgroundBrush();
		}
		public static void Initialize(Converter<Guid, BitmapImage> imageFactory)
		{
			_imageFactory = imageFactory;
		}
		public static void Dispose()
		{
			_brushes.Clear();
			_pens.Clear();
			_pictureBrushes.Clear();
		}

		public static void UpdateZoom(double zoom, double pointZoom)
		{
			Zoom = zoom;
			PointZoom = pointZoom;
			ZonePen.Thickness = 1 / Zoom;
			PointGeometry.Rect = new Rect(-pointZoom / 2, -pointZoom / 2, pointZoom, pointZoom);
			_transparentBackgroundBrush.Viewport = new Rect(0, 0, 16 / zoom, 16 / zoom);
		}

		public static void CacheBrush(IElementBackground element)
		{
			if (element.BackgroundImageSource.HasValue && !_pictureBrushes.ContainsKey(element.BackgroundImageSource.Value))
			{
				var brush = GetBrush(element.BackgroundImageSource.Value);
				_pictureBrushes.Add(element.BackgroundImageSource.Value, brush);
			}
		}
		public static Brush GetBrush(IElementBackground element)
		{
			if (element.AllowTransparent && element.BackgroundColor == Colors.Transparent)
				return _transparentBackgroundBrush;
			else if (element.BackgroundImageSource.HasValue)
			{
				CacheBrush(element);
				return _pictureBrushes[element.BackgroundImageSource.Value];
				//return GetBrush(element.BackgroundImageSource.Value);
			}
			else
				return GetBrush(element.BackgroundColor);
		}
		public static Brush GetTransparentBrush(IElementBackground element)
		{
			var brush = GetBrush(element);
			if (!_transparentBrushes.ContainsKey(brush))
			{
				var transparent = brush.CloneCurrentValue();
				transparent.Opacity = 0.5;
				transparent.Freeze();
				_transparentBrushes.Add(brush, transparent);
			}
			return _transparentBrushes[brush];
		}
		public static Brush GetBrush(Color color)
		{
			if (!_brushes.ContainsKey(color))
			{
				var brush = new SolidColorBrush(color);
				brush.Freeze();
				_brushes.Add(color, brush);
			}
			return _brushes[color];
		}
		public static Brush GetTransparentBrush(Color color)
		{
			var brush = GetBrush(color);
			if (!_transparentBrushes.ContainsKey(brush))
			{
				var transparent = brush.CloneCurrentValue();
				transparent.Opacity = 0.5;
				transparent.Freeze();
				_transparentBrushes.Add(brush, transparent);
			}
			return _transparentBrushes[brush];
		}

		public static Pen GetPen(Color color, double thickness)
		{
			if (!_pens.ContainsKey(color))
				_pens.Add(color, new Dictionary<double, Pen>());
			if (!_pens[color].ContainsKey(thickness))
			{
				var brush = GetBrush(color);
				var pen = new Pen(brush, thickness);
				pen.Freeze();
				_pens[color].Add(thickness, pen);
			}
			return _pens[color][thickness];
		}

		private static Brush GetBrush(Guid guid)
		{
			var bitmap = _imageFactory(guid);
			var brush = new ImageBrush(bitmap);
			PainterHelper.FreezeBrush(brush);
			return brush;
		}
		private static ImageBrush CreateTransparentBackgroundBrush()
		{
			BitmapImage bi = new BitmapImage();
			bi.BeginInit();
			bi.UriSource = new System.Uri(string.Format("pack://application:,,,/{0};component/Resources/Transparent.bmp", Assembly.GetExecutingAssembly()));
			bi.EndInit();
			var brush = new ImageBrush(bi)
			{
				TileMode = TileMode.Tile,
				ViewportUnits = BrushMappingMode.Absolute,
				Viewport = new Rect(0, 0, 16, 16)
			};
			return brush;
		}
	}
}
