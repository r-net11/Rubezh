using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class PolygonZonePainter : PolygonPainter
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
