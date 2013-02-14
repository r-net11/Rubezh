using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class RectangleZonePainter : RectanglePainter
	{
		public RectangleZonePainter(ElementBase element)
			: base(element)
		{
		}

		protected override Brush GetBrush()
		{
			return PainterCache.GetTransparentBrush(Element.BackgroundColor, Element.BackgroundPixels);
		}
		protected override Pen GetPen()
		{
			return PainterCache.ZonePen;
		}
	}
}
