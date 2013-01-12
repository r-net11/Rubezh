using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class PolygonZonePainter : PolygonPainter
	{
		protected override Brush GetBrush(ElementBase element)
		{
			return PainterCache.GetTransparentBrush(element.BackgroundColor, element.BackgroundPixels);
		}
	}
}
