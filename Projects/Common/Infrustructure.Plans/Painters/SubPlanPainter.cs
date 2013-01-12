using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class SubPlanPainter : RectanglePainter
	{
		protected override Brush GetBrush(ElementBase element)
		{
			return PainterCache.GetTransparentBrush(element.BackgroundColor, element.BackgroundPixels);
		}
	}
}
