using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class PolygonZonePainter : PolygonPainter
	{
		public PolygonZonePainter(ElementBase element)
			: base(element)
		{
		}

		protected override Brush GetBrush()
		{
			return PainterCache.GetTransparentBrush(Element);
		}
		protected override Pen GetPen()
		{
			return PainterCache.ZonePen;
		}
	}
}
