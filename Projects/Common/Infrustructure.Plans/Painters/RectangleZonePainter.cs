using System.Windows;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class RectangleZonePainter : RectanglePainter
	{
		public override bool RedrawOnZoom
		{
			get { return true; }
		}
		protected override Brush GetBrush(ElementBase element)
		{
			return PainterCache.GetTransparentBrush(element.BackgroundColor, element.BackgroundPixels);
		}
		protected override Pen GetPen(ElementBase element)
		{
			return PainterCache.GetPen(element.BorderColor, element.BorderThickness / PainterCache.Zoom);
		}
	}
}
