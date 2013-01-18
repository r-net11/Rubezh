using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class DefaultPainter : RectanglePainter
	{
		protected override ImageBrush CreateImageBrush(ElementBase element, Rect rect)
		{
			return null;
		}
		protected override SolidColorBrush CreateSolidColorBrush(ElementBase element, Rect rect)
		{
			var brush = new SolidColorBrush(Colors.Black);
			brush.Freeze();
			return brush;
		}
		protected override Pen CreatePen(ElementBase element, Rect rect)
		{
			return null;
		}
	}
}