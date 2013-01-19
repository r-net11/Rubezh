using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class DefaultPainter : RectanglePainter
	{
		protected override Brush CreateBrush(ElementBase element, Rect rect)
		{
			var brush = new SolidColorBrush(Colors.Black);
			brush.Freeze();
			return brush;
		}
		protected override Pen CreatePen(ElementBase element, Rect rect)
		{
			return null;
		}
		protected override void UpdateBrush(ElementBase element, Rect rect)
		{
			
		}
	}
}