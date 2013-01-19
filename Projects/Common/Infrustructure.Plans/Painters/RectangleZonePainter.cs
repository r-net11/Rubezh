using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class RectangleZonePainter : RectanglePainter
	{
		protected override Brush CreateBrush(ElementBase element, Rect rect)
		{
			var brush = new SolidColorBrush();
			brush.Opacity = 0.5;
			return brush;
		}
		protected override Pen CreatePen(ElementBase element, Rect rect)
		{
			return PainterCache.ZonePen;
		}
		protected override void UpdatePen(ElementBase element, Rect rect)
		{
		}
	}
}
