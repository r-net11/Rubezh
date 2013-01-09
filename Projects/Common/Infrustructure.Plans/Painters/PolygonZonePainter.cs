using System.Windows;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;
using System.Windows.Media;

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
