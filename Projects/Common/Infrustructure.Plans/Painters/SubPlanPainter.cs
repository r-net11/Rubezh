using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class SubPlanPainter : RectanglePainter
	{
		protected override Brush CreateBrush(ElementBase element, Rect rect)
		{
			var brush = new SolidColorBrush(element.BackgroundColor);
			brush.Opacity = 0.6;
			brush.Freeze();
			return brush;
		}
		protected override void UpdateBrush(ElementBase element, Rect rect)
		{
		}
	}
}
