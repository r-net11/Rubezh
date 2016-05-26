using Common;
using StrazhAPI.Plans;
using StrazhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = StrazhAPI.Color;
using Colors = StrazhAPI.Colors;
using WindowsColor = System.Windows.Media.Color;
using WindowsColors = System.Windows.Media.Colors;

namespace Infrustructure.Plans.Painters
{
	public class PainterCache
	{
		private static Converter<Guid, BitmapImage> _imageFactory;
		private static Converter<Guid, Drawing> _drawingFactory;
		private static Converter<Guid, Visual> _visualFactory;
		private static Dictionary<Color, Brush> _brushes = new Dictionary<Color, Brush>();
		private static Dictionary<Brush, Brush> _transparentBrushes = new Dictionary<Brush, Brush>();
		private static Dictionary<Guid, Brush> _pictureBrushes = new Dictionary<Guid, Brush>();
		private static Dictionary<Color, Dictionary<double, Pen>> _pens = new Dictionary<Color, Dictionary<double, Pen>>();

		public static double DefaultPointSize { get; private set; }

		public static Brush GridLineBrush { get; private set; }

		public static Brush BlackBrush { get; private set; }

		public static Brush WhiteBrush { get; private set; }

		public static Brush TransparentBrush { get; private set; }

		public static bool UseTransparentImage { get; set; }

		static PainterCache()
		{
			UseTransparentImage = true;
			try
			{
				TransparentBrush = new SolidColorBrush(WindowsColors.Transparent);
				TransparentBrush.Freeze();
				BlackBrush = new SolidColorBrush(WindowsColors.Black);
				BlackBrush.Freeze();
				WhiteBrush = new SolidColorBrush(WindowsColors.White);
				WhiteBrush.Freeze();
				GridLineBrush = new SolidColorBrush(WindowsColors.Orange);
				GridLineBrush.Freeze();
			}
			catch (Exception e)
			{
				Logger.Error(e, "PainterCache.PainterCache()");
			}
		}

		public static void Initialize(Converter<Guid, BitmapImage> imageFactory, Converter<Guid, Drawing> drawingFactory, Converter<Guid, Visual> visualFactory)
		{
			_imageFactory = imageFactory;
			_drawingFactory = drawingFactory;
			_visualFactory = visualFactory;
		}

		public static void Dispose()
		{
			_brushes.Clear();
			_pens.Clear();
			_pictureBrushes.Clear();
		}

		public static void CacheBrush(IElementBackground element)
		{
			if (element.BackgroundImageSource.HasValue && !_pictureBrushes.ContainsKey(element.BackgroundImageSource.Value))
			{
				var brush = GetBrush(element.BackgroundImageSource.Value, element.ImageType);
				_pictureBrushes.Add(element.BackgroundImageSource.Value, brush);
			}
		}

		public static Brush GetBrush(Color color)
		{
			if (!_brushes.ContainsKey(color))
			{
				var brush = new SolidColorBrush(color.ToWindowsColor());
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

		private static Brush GetBrush(Guid guid, ResourceType imageType)
		{
			Brush brush = null;
			switch (imageType)
			{
				case ResourceType.Drawing:
					var drawing = _drawingFactory(guid);
					if (drawing != null)
					{
						drawing.Freeze();
						brush = new DrawingBrush(drawing);
					}
					else
						brush = new SolidColorBrush(WindowsColor.FromRgb(255, 255, 255));
					break;

				case ResourceType.Visual:
					var visual = _visualFactory(guid);
					if (visual != null)
						brush = new VisualBrush(visual);
					else
						brush = new SolidColorBrush(WindowsColor.FromRgb(255, 255, 255));
					break;

				case ResourceType.Image:
					var bitmap = _imageFactory(guid);
					brush = new ImageBrush(bitmap);
					break;
			}
			if (brush != null)
				PainterHelper.FreezeBrush(brush);
			return brush;
		}

		private static ImageBrush CreateTransparentBackgroundBrush()
		{
			var bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new System.Uri(string.Format("pack://application:,,,/{0};component/Resources/Transparent.bmp", Assembly.GetExecutingAssembly()));
			bitmapImage.EndInit();
			var brush = new ImageBrush(bitmapImage)
			{
				TileMode = TileMode.Tile,
				ViewportUnits = BrushMappingMode.Absolute,
				Viewport = new Rect(0, 0, 16, 16)
			};
			return brush;
		}

		private ImageBrush _transparentBackgroundBrush;

		public Pen ZonePen { get; private set; }

		public Pen GridLinePen { get; private set; }

		public RectangleGeometry PointGeometry { get; private set; }

		public double Zoom { get; private set; }

		public double PointZoom { get; private set; }

		public PainterCache()
		{
			ZonePen = new Pen(BlackBrush, 1);
			GridLinePen = new Pen(GridLineBrush, 1);
			GridLinePen.EndLineCap = PenLineCap.Square;
			GridLinePen.StartLineCap = PenLineCap.Square;
			GridLinePen.DashStyle = DashStyles.Dash;
			PointGeometry = new RectangleGeometry(new Rect(-15, -15, 30, 30));
			_transparentBackgroundBrush = CreateTransparentBackgroundBrush();
		}

		public void UpdateZoom(double zoom, double pointZoom)
		{
			Zoom = zoom;
			PointZoom = pointZoom;
			ZonePen.Thickness = 1 / Zoom;
			GridLinePen.Thickness = 1 / Zoom;
			PointGeometry.Rect = new Rect(-pointZoom / 2, -pointZoom / 2, pointZoom, pointZoom);
			_transparentBackgroundBrush.Viewport = new Rect(0, 0, 16 / zoom, 16 / zoom);
			DefaultPointSize = PointZoom * Zoom;
		}

		public Brush GetBrush(IElementBackground element)
		{
			if (element.BackgroundImageSource.HasValue)
			{
				CacheBrush(element);
				return _pictureBrushes[element.BackgroundImageSource.Value];
				//return GetBrush(element.BackgroundImageSource.Value);
			}
			else if (element.AllowTransparent && element.BackgroundColor == Colors.Transparent)
				return UseTransparentImage ? _transparentBackgroundBrush : TransparentBrush;
			else
				return GetBrush(element.BackgroundColor);
		}

		public Brush GetTransparentBrush(IElementBackground element)
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

		public Pen GetPen(IElementBorder element)
		{
			return GetPen(element.BorderColor, element.BorderThickness);
		}
	}
}