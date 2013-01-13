using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public static class PainterCache
	{
		private static Dictionary<Color, Brush> _brushes = new Dictionary<Color, Brush>();
		private static Dictionary<Brush, Brush> _transparentBrushes = new Dictionary<Brush, Brush>();
		private static Dictionary<byte[], Brush> _pictureBrushes = new Dictionary<byte[], Brush>();
		private static Dictionary<Color, Dictionary<double, Pen>> _pens = new Dictionary<Color, Dictionary<double, Pen>>();

		public static Brush GetTransparentBrush(Color color, byte[] backgroundPixels)
		{
			var brush = GetBrush(color, backgroundPixels);
			if (!_transparentBrushes.ContainsKey(brush))
			{
				var transparent = brush.Clone();
				transparent.Opacity = 0.5;
				transparent.Freeze();
				_transparentBrushes.Add(brush, transparent);
			}
			return _transparentBrushes[brush];
		}
		public static Brush GetBrush(Color color, byte[] backgroundPixels)
		{
			if (backgroundPixels == null)
				return GetBrush(color);
			if (!_pictureBrushes.ContainsKey(backgroundPixels))
				_pictureBrushes.Add(backgroundPixels, PainterHelper.CreateBrush(backgroundPixels));
			return _pictureBrushes[backgroundPixels];
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
		public static double Zoom { get; internal set; }
		public static double PointZoom { get; internal set; }
	}
}
