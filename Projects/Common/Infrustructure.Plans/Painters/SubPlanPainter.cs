using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class SubPlanPainter : RectanglePainter
	{
		protected override void InitializeBrushes(ElementBase element, System.Windows.Rect rect)
		{
			base.InitializeBrushes(element, rect);
			SolidColorBrush.Opacity = 0.5;
		}
	}
}
