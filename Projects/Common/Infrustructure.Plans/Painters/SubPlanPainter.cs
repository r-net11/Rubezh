using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class SubPlanPainter : RectanglePainter
	{
		protected override ImageBrush CreateImageBrush(ElementBase element, Rect rect)
		{
			return null;
		}
		protected override SolidColorBrush CreateSolidColorBrush(ElementBase element, Rect rect)
		{
			var brush = new SolidColorBrush(element.BackgroundColor);
			brush.Opacity = 0.6;
			brush.Freeze();
			return brush;
		}
	}
}
